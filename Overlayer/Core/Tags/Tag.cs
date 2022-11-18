using System;
using System.IO;
using System.Reflection;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace Overlayer.Core
{
    public class Tag
    {
        public string Name;
        public string Description => description == null ? Main.Language[Name] : description;
        private string description;
        public bool Referenced => ReferencedCount > 0;
        public int ReferencedCount;
        public bool IsString;
        public bool IsStringOpt;
        public bool IsOpt;
        public string DefOptStr;
        public double? DefOptNum;
        public bool Hidden;
        public MethodInfo Raw;
        public Func<string> Str;
        public Func<double> Num;
        public Func<string, string> StrOptStr;
        public Func<string, double> StrOptNum;
        public Func<double, string> NumOptStr;
        public Func<double, double> NumOptNum;
        public Thread[] Threads;
        internal Tag(string name, MethodInfo raw, bool hidden)
        {
            Name = name;
            Raw = raw;
            Type retType = Raw.ReturnType;
            var @params = Raw.GetParameters();
            var paramLen = @params.Length;
            if (paramLen == 1)
            {
                IsOpt = true;
                var optParam = @params[0];
                var optType = optParam.ParameterType;
                if (retType == typeof(string))
                {
                    IsString = true;
                    if (optType == typeof(string))
                    {
                        IsStringOpt = true;
                        DefOptStr = optParam.DefaultValue as string;
                        StrOptStr = (Func<string, string>)Raw.CreateDelegate(typeof(Func<string, string>));
                    }
                    else if (optType == typeof(double))
                    {
                        DefOptNum = optParam.DefaultValue is double f ? f : null;
                        NumOptStr = (Func<double, string>)Raw.CreateDelegate(typeof(Func<double, string>));
                    }
                    else throw new InvalidOperationException($"Tag's ValueGetter's Option Type Cannot Be {retType}. Only Supports double(System.Single) Or string(System.String)");
                }
                else if (retType == typeof(double))
                {
                    if (optType == typeof(string))
                    {
                        IsStringOpt = true;
                        DefOptStr = optParam.DefaultValue as string;
                        StrOptNum = (Func<string, double>)Raw.CreateDelegate(typeof(Func<string, double>));
                    }
                    else if (optType == typeof(double))
                    {
                        DefOptNum = optParam.DefaultValue is double f ? f : null;
                        NumOptNum = (Func<double, double>)Raw.CreateDelegate(typeof(Func<double, double>));
                    }
                    else throw new InvalidOperationException($"Tag's ValueGetter's Option Type Cannot Be {retType}. Only Supports double(System.Single) Or string(System.String)");
                }
                else throw new InvalidOperationException($"Tag's ValueGetter's Return Type Cannot Be {retType}. Only Supports double(System.Single) Or string(System.String)");
            }
            else if (paramLen >= 2)
                throw new InvalidOperationException($"Tag's ValueGetter's Parameters Cannot Be Greater Than 1. Only Supports 1 Parameter.");
            else
            {
                if (retType == typeof(string))
                {
                    IsString = true;
                    Str = (Func<string>)Raw.CreateDelegate(typeof(Func<string>));
                }
                else if (retType == typeof(double))
                    Num = (Func<double>)Raw.CreateDelegate(typeof(Func<double>));
                else throw new InvalidOperationException($"Tag's ValueGetter's Return Type Cannot Be {retType}. Only Supports double(System.Single) Or string(System.String)");
            }
            Hidden = hidden;
        }
        public bool IsDynamic = false;
        public Func<object> Dyn;
        internal Tag(string name, string description, Func<object> del)
        {
            Name = name;
            this.description = description;
            IsOpt = false;
            IsDynamic = true;
            Dyn = del;
        }
        public string DynamicString() => Dyn().ToString();
        internal Tag(string name, string description, Func<double, double> raw, Func<string> rawStr)
        {
            Name = name;
            this.description = description;
            if (raw != null)
            {
                NumOptNum = raw;
                IsOpt = true;
                DefOptNum = -1;
            }
            if (rawStr != null)
            {
                IsString = true;
                Str = rawStr;
                IsOpt = false;
                DefOptStr = "";
            }
        }
        public override string ToString() => $"{{{Name}}}: {Description}";
        public void Start()
        {
            if (Threads != null)
            {
                for (int i = 0; i < Threads.Length; i++)
                {
                    Thread thread = Threads[i];
                    if (thread.ThreadState == ThreadState.Unstarted ||
                        thread.ThreadState == ThreadState.Stopped)
                        thread.Start();
                }
            }
        }
        public void Stop()
        {
            if (Threads != null)
            {
                for (int i = 0; i < Threads.Length; i++)
                {
                    Thread thread = Threads[i];
                    if (thread.ThreadState == ThreadState.Running ||
                        thread.ThreadState == ThreadState.Background)
                        thread.Abort();
                }
            }
        }
        public string StrValue()
        {
            if (IsDynamic)
                return Dyn().ToString();
            if (IsString)
                return Str();
            return Num().ToString();
        }
        public string Value
        {
            get
            {
                if (IsDynamic)
                    return (string)Dyn();
                if (IsString)
                    return Str();
                return Num().ToString();
            }
        }
        public double FloatValue() => ValueFloat;
        public string StringValue() => Value;
        public string OptValue(double opt)
        {
            if (IsString)
                return NumOptStr(opt);
            return NumOptNum(opt).ToString();
        }
        public string OptValue(string opt)
        {
            if (IsString)
                return StrOptStr(opt);
            return StrOptNum(opt).ToString();
        }

        public double OptValueFloat(double opt)
        {
            if (IsString) return 0;
            else return NumOptNum(opt);
        }
        public double OptValueFloat(string opt)
        {
            if (IsString) return 0;
            else return StrOptNum(opt);
        }
        public double ValueFloat
        {
            get
            {
                if (IsDynamic)
                    return (double)Dyn();
                if (IsString) return 0;
                else return Num();
            }
        }
    }
}
