using System;
using System.Reflection;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace Overlayer.Tags
{
    public class Tag
    {
        public string Name;
        public string Description;
        public string NumberFormat;
        public bool IsString;
        public bool IsStringOpt;
        public bool IsOpt;
        public string DefOptStr;
        public float? DefOptNum;
        public MethodInfo Raw;
        public Func<string> Str;
        public Func<float> Num;
        public Func<string, string> StrOptStr;
        public Func<string, float> StrOptNum;
        public Func<float, string> NumOptStr;
        public Func<float, float> NumOptNum;
        public Thread[] Threads;
        internal Tag(string name, string description, MethodInfo raw, string format)
        {
            Name = name;
            Description = description;
            NumberFormat = format;
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
                    else if (optType == typeof(float))
                    {
                        DefOptNum = optParam.DefaultValue is float f ? f : null;
                        NumOptStr = (Func<float, string>)Raw.CreateDelegate(typeof(Func<float, string>));
                    }
                    else throw new InvalidOperationException($"Tag's ValueGetter's Option Type Cannot Be {retType}. Only Supports float(System.Single) Or string(System.String)");
                }
                else if (retType == typeof(float))
                {
                    if (optType == typeof(string))
                    {
                        IsStringOpt = true;
                        DefOptStr = optParam.DefaultValue as string;
                        StrOptNum = (Func<string, float>)Raw.CreateDelegate(typeof(Func<string, float>));
                    }
                    else if (optType == typeof(float))
                    {
                        DefOptNum = optParam.DefaultValue is float f ? f : null;
                        NumOptNum = (Func<float, float>)Raw.CreateDelegate(typeof(Func<float, float>));
                    }
                    else throw new InvalidOperationException($"Tag's ValueGetter's Option Type Cannot Be {retType}. Only Supports float(System.Single) Or string(System.String)");
                }
                else throw new InvalidOperationException($"Tag's ValueGetter's Return Type Cannot Be {retType}. Only Supports float(System.Single) Or string(System.String)");
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
                else if (retType == typeof(float))
                    Num = (Func<float>)Raw.CreateDelegate(typeof(Func<float>));
                else throw new InvalidOperationException($"Tag's ValueGetter's Return Type Cannot Be {retType}. Only Supports float(System.Single) Or string(System.String)");
            }
        }
        internal Tag(string name, string description, Func<float, float> raw)
        {
            Name = name;
            Description = description;
            NumOptNum = raw;
            IsOpt = true;
            DefOptNum = -1;
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
        public string Value
        {
            get
            {
                if (IsString)
                    return Str();
                else if (NumberFormat == null)
                    return Num().ToString();
                else return Num().ToString(NumberFormat);
            }
        }
        public float FloatValue() => Num();
        public string StringValue() => Str();
        public string OptValue(float opt)
        {
            if (IsString)
                return NumOptStr(opt);
            if (NumberFormat == null)
                return NumOptNum(opt).ToString();
            else return NumOptNum(opt).ToString(NumberFormat);
        }
        public string OptValue(string opt)
        {
            if (IsString)
                return StrOptStr(opt);
            if (NumberFormat == null)
                return StrOptNum(opt).ToString();
            else return StrOptNum(opt).ToString(NumberFormat);
        }

        public float OptValueFloat(float opt)
        {
            if (IsString) return 0;
            else return NumOptNum(opt);
        }
        public float OptValueFloat(string opt)
        {
            if (IsString) return 0;
            else return StrOptNum(opt);
        }
        public float ValueFloat
        {
            get
            {
                if (IsString) return 0;
                else return Num();
            }
        }
    }
}
