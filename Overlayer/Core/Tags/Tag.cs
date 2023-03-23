using System;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using static Overlayer.Core.Tags.Replacer;

namespace Overlayer.Core.Tags
{
    public class Tag : IDisposable
    {
        public readonly string Name;
        public readonly TagConfig Config;
        List<Thread> Threads;
        public bool Built { get; private set; }
        public MethodInfo Getter { get; private set; }
        public Delegate GetterDelegate { get; private set; }
        public MethodInfo OptionConverter { get; private set; }
        public MethodInfo ReturnConverter { get; private set; }
        public Func<object> FastInvoker { get; private set; }
        public Func<object, object> FastInvokerOpt { get; private set; }
        public bool HasOption { get; private set; }
        public bool Referenced => ReferencedCount > 0;
        internal int ReferencedCount = 0;
        // For CustomTag
        public string SourcePath = null;
        public Tag(string name, TagConfig config = null)
        {
            Name = name;
            Config = config ?? TagConfig.DefaultNormal;
            OptionConverter = null;
            Threads = new List<Thread>();
        }
        public Tag SetGetter(Func<string, string> getter)
        {
            if (Built) return null;
            Getter = Wrapper.Wrap(getter);
            GetterDelegate = getter;
            HasOption = true;
            return this;
        }
        public Tag SetGetter(Delegate getter)
        {
            if (Built) return null;
            var invoke = getter.GetType().GetMethod("Invoke");
            if (!CheckGetterSig(invoke))
                throw new InvalidOperationException($"Parameter's Length Must Be Less Than 2. ({getter})");
            var prms = invoke.GetParameters();
            HasOption = prms.Length == 1;
            if (HasOption)
            {
                Type pType = invoke.GetParameters()[0].ParameterType;
                if (pType != typeof(string))
                {
                    OptionConverter = StringConverter.GetToConverter(pType);
                    if (OptionConverter == null)
                        throw new NotSupportedException($"Option Type:{pType} Is Not Supported.");
                }
            }
            if (invoke.ReturnType != typeof(string))
                ReturnConverter = StringConverter.GetFromConverter(invoke.ReturnType);
            Getter = Wrapper.Wrap(getter);
            GetterDelegate = getter;
            return this;
        }
        public Tag SetGetter(MethodInfo getter)
        {
            if (Built) return null;
            if (!CheckGetterSig(getter))
                throw new InvalidOperationException($"Parameter's Length Must Be Less Than 2. ({getter})");
            var prms = getter.GetParameters();
            HasOption = prms.Length == 1;
            if (HasOption)
            {
                Type pType = getter.GetParameters()[0].ParameterType;
                if (pType != typeof(string))
                {
                    OptionConverter = StringConverter.GetToConverter(pType);
                    if (OptionConverter == null)
                        throw new NotSupportedException($"Option Type:{pType} Is Not Supported.");
                }
            }
            if (getter.ReturnType != typeof(string))
                ReturnConverter = StringConverter.GetFromConverter(getter.ReturnType);
            Getter = getter;
            if (HasOption)
                GetterDelegate = getter.CreateDelegate(Expression.GetFuncType(prms[0].ParameterType, getter.ReturnType));
            else GetterDelegate = getter.CreateDelegate(Expression.GetFuncType(getter.ReturnType));
            return this;
        }
        public Tag AddThread(ThreadStart thread)
        {
            if (Built) return null;
            Threads.Add(new Thread(thread));
            return this;
        }
        public Tag AddThread(MethodInfo thread)
        {
            if (Built) return null;
            if (!CheckThreadSig(thread))
                throw new InvalidOperationException("ReturnType Must Be Void And Parameter's Length Must Be 0.");
            Threads.Add(new Thread((ThreadStart)thread.CreateDelegate(typeof(ThreadStart))));
            return this;
        }
        public void Build()
        {
            if (Built) return;
            if (Getter == null)
                throw new InvalidOperationException("Cannot Build Without Set Getter!");
            Threads.ForEach(t => t.Start());
            if (HasOption)
            {
                FastInvoker = null;
                FastInvokerOpt = Wrapper.WrapFastOpt(Getter);
            }
            else
            {
                FastInvoker = Wrapper.WrapFast(Getter);
                FastInvokerOpt = null;
            }
            Built = true;
        }
        public void Dispose()
        {
            Getter = null;
            GetterDelegate = null;
            OptionConverter = null;
            ReturnConverter = null;
            FastInvoker = null;
            FastInvokerOpt = null;
            Threads.ForEach(t => t.Abort());
            Threads = null;
            Built = false;
        }
        public Tag Copy()
        {
            Tag tag = new Tag(Name, Config);
            tag.Getter = Getter;
            tag.GetterDelegate = GetterDelegate;
            tag.OptionConverter = OptionConverter;
            tag.ReturnConverter = ReturnConverter;
            tag.FastInvoker = FastInvoker;
            tag.FastInvokerOpt = FastInvokerOpt;
            Threads.ForEach(tag.Threads.Add);
            return tag;
        }
        public override string ToString() => $"{Config.Open}{Name}{Config.Close}";
        public static bool CheckGetterSig(MethodInfo method) => method.GetParameters().Length < 2;
        public static bool CheckThreadSig(MethodInfo method)
        {
            if (method.ReturnType != typeof(void))
                return false;
            var prms = method.GetParameters();
            if (prms.Length < 1) return true;
            else return false;
        }
    }
}
