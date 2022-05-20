using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using BenchmarkDotNet.Attributes;
using S;

namespace OverlayerEx
{
    public class Tag
    {
        public class Setting
        {
            public string name;
            public string op;
            public string[] values;
        }
        static Tag()
        {
            Tags = new List<Tag>()
            {
                new Tag("{Year}", "Year Of System Time").SetValueGetter(() => DateTime.Now.Year),
                new Tag("{Month}", "Month Of System Time").SetValueGetter(() => DateTime.Now.Month),
                new Tag("{Day}", "Day Of System Time").SetValueGetter(() => DateTime.Now.Day),
                new Tag("{Hour}", "Hour Of System Time").SetValueGetter(() => DateTime.Now.Hour),
                new Tag("{Minute}", "Minute Of System Time").SetValueGetter(() => DateTime.Now.Minute),
                new Tag("{Second}", "Second Of System Time").SetValueGetter(() => DateTime.Now.Second),
                new Tag("{MilliSecond}", "MilliSecond Of System Time").SetValueGetter(() => DateTime.Now.Millisecond),
            };
            NameTags = new Dictionary<string, Tag>();
            CustomSettings = new List<Setting>();
            for (int i = 0; i < Tags.Count; i++)
                NameTags.Add(Tags[i].Name, Tags[i]);
            NPTags = new List<Tag>
            {
                NameTags["{Year}"],
                NameTags["{Month}"],
                NameTags["{Day}"],
                NameTags["{Hour}"],
                NameTags["{Minute}"],
                NameTags["{Second}"],
                NameTags["{MilliSecond}"],
            };
        }
        public Tag SetValueGetter(Func<object> getter)
        {
            ValueGetter = getter;
            return this;
        }
        public Func<object> ValueGetter = () => "fuck you";
        Tag(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public readonly string Name;
        public readonly string Description;
        public string Value => ValueGetter().ToString();
        public static readonly List<Tag> Tags;
        public static readonly List<Tag> NPTags;
        public static List<Setting> CustomSettings;
        public static readonly Dictionary<string, Tag> NameTags;
    }
    public static class Program
    {
        public static void Main(string[] args)
        {
            Method method = Method.GetMethod(typeof(Program), "WriteLine");
            method.AddPrefix((Action<string>)(fuck =>
            {
                Console.WriteLine($"FUck{fuck}");
            }));
            method.Attach();
            WriteLine("FKF");
        }
        public static string ToString(byte[] bytes)
        {
            int length = bytes.Length;
            StringBuilder sb = new StringBuilder();
            sb.Append($"new byte[{length}] {{");
            for (int i = 0; i < length; i++)
                sb.Append($"{bytes[i]}, ");
            return sb.Append("}}").ToString();
        }
        public static void WriteLine(string fuck)
        {
            Console.WriteLine(fuck);
        }
    }
    public class TagConverter
    {
        public static readonly int maxTagLength = Tag.NameTags.Keys.Select(t => t.Length).Max();
        private readonly StringBuilder sb;
        private readonly IReadOnlyList<Func<object>> getters;
        public TagConverter(string source)
        {
            sb = new StringBuilder(source.Length);
            StringBuilder tagSb = new StringBuilder(maxTagLength);
            List<Func<object>> getters = new List<Func<object>>();
            bool tagMode = false;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (tagMode)
                {
                    if (c == '{')
                    {
                        sb.Append(tagSb);
                        tagSb.Clear();
                        tagSb.Append(c);
                    }
                    else if (c == '}')
                    {
                        tagSb.Append(c);
                        Tag tag = Tag.NameTags[tagSb.ToString()];
                        if (tag != null)
                        {
                            string beforeTagString = sb.ToString();
                            getters.Add(() => beforeTagString);
                            sb.Clear();
                            getters.Add(tag.ValueGetter);
                        }
                        else
                        {
                            sb.Append(tagSb);
                        }
                        tagSb.Clear();
                        tagMode = false;
                    }
                    else
                    {
                        tagSb.Append(c);
                    }
                }
                else
                {
                    if (c == '{')
                    {
                        tagMode = true;
                        tagSb.Append(c);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            if (sb.Length > 0)
            {
                string restString = sb.ToString();
                getters.Add(() => restString);
            }
            this.getters = getters.AsReadOnly();
        }
        public string Convert()
        {
            sb.Clear();
            for (int i = 0; i < getters.Count; i++)
                sb.Append(getters[i]());
            return sb.ToString();
        }
    }
    public class BenchmarkClass
    {
        public static readonly List<Func<object>> list;
        public static readonly TagConverter tagConverter;
        public static readonly string source;
        static BenchmarkClass()
        {
            source = "test {MilliSecond} asdfasfd {Year} {Minu {Hour}";
            tagConverter = new TagConverter(source);
            list = new List<Func<object>>();
            for (int i = 0; i < 100; i++)
                list.Add(() => "asdfjkl;");
        }
        public static string s = string.Empty;
        public static StringBuilder sb = new StringBuilder();
        [Benchmark]
        public void TagConvRepl()
        {
            tagConverter.Convert();
        }
        [Benchmark]
        public void StringRepl()
        {
            string text = source;
            for (int i = 0; i < Tag.Tags.Count; i++)
            {
                Tag tag = Tag.Tags[i];
                if (text.Contains(tag.Name))
                    text = text.Replace(tag.Name, tag.Value);
            }
        }
        //[Benchmark]
        public void String()
        {
            s = string.Empty;
            for (int i = 0; i < 100; i++)
                s += "asdfjkl;";
        }
        //[Benchmark]
        public void StringBuilder()
        {
            sb.Clear();
            for (int i = 0; i < 100; i++)
                sb.Append(list[i]());
        }
    }
}


namespace S
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    public class Method
    {
        static readonly FieldInfo paramTypes;
        public static readonly bool IsMonoRuntime;
        public static readonly MethodInfo MonoCreateDM;
        public static readonly FieldInfo MonoHandleDM;
        public static readonly MethodInfo NetCreateDM;
        public static readonly OpCode[] OneByteOpCodes;
        public static readonly OpCode[] TwoBytesOpCodes;
        public const ExceptionBlockType
                           endBlock = ExceptionBlockType.End | ExceptionBlockType.BigBlock,
                           beginBlock = ExceptionBlockType.Begin | ExceptionBlockType.BigBlock,
                           beginFilter = ExceptionBlockType.Begin | ExceptionBlockType.FilterBlock,
                           beginFinally = ExceptionBlockType.Begin | ExceptionBlockType.FinallyBlock,
                           beginCatch = ExceptionBlockType.Begin | ExceptionBlockType.CatchBlock,
                           beginFault = ExceptionBlockType.Begin | ExceptionBlockType.FaultBlock,
                           flagValue = (ExceptionBlockType)0x3F;
        static Method()
        {
            IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
            MonoCreateDM = typeof(DynamicMethod).GetMethod("CreateDynMethod", (BindingFlags)15420);
            MonoHandleDM = typeof(DynamicMethod).GetField("mhandle", (BindingFlags)15420);
            NetCreateDM = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", (BindingFlags)15420);
            OneByteOpCodes = new OpCode[0xe1];
            TwoBytesOpCodes = new OpCode[0x1f];
            foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var opcode = (OpCode)field.GetValue(null);
                if (opcode.OpCodeType == OpCodeType.Nternal)
                    continue;
                if (opcode.Size == 1) OneByteOpCodes[opcode.Value] = opcode;
                else TwoBytesOpCodes[opcode.Value & 0xff] = opcode;
            }
            if (IsMonoRuntime)
                paramTypes = typeof(DynamicMethod).GetField("parameters", (BindingFlags)15420);
            else paramTypes = typeof(DynamicMethod).GetField("m_parameterTypes", (BindingFlags)15420);
        }
        public readonly MethodBase Base;

        public readonly MethodBody Body;
        public readonly Module Module;
        public readonly Type[] TypeGenerics;
        public readonly Type[] MethodGenerics;
        public readonly ByteBuffer Buffer;
        public readonly IList<LocalVariableInfo> LocalVars;
        public readonly List<Instruction> Instructions;
        public readonly IList<ExceptionHandlingClause> ExceptionHandlers;
        public Method(MethodBase method)
        {
            Base = method;
            Body = method.GetMethodBody();
            if (Body == null)
                throw new ArgumentException("Method has no body");
            var bytes = Body.GetILAsByteArray();
            if (bytes == null)
                throw new ArgumentException("Can not get the body of the method");
            if (!(method is ConstructorInfo))
                MethodGenerics = method.GetGenericArguments();
            if (method.DeclaringType != null)
                TypeGenerics = method.DeclaringType.GetGenericArguments();
            LocalVars = Body.LocalVariables;
            Module = method.Module;
            Buffer = new ByteBuffer(bytes);
            Instructions = new List<Instruction>((bytes.Length + 1) / 2);
            ExceptionHandlers = Body.ExceptionHandlingClauses;
            Instruction previous = null;
            while (Buffer.Position < Buffer.buffer.Length)
            {
                var instruction = new Instruction(Buffer.Position, ReadOpCode());
                ReadOperand(instruction);
                if (previous != null)
                {
                    instruction.Previous = previous;
                    previous.Next = instruction;
                }
                Instructions.Add(instruction);
                previous = instruction;
            }
            foreach (ExceptionHandlingClause ex in ExceptionHandlers)
            {
                Instruction.FindInstruction(Instructions, ex.TryOffset)
                    .Block = new ExceptionBlock(beginBlock);
                Instruction.FindInstruction(Instructions, ex.TryLength + ex.HandlerLength - 1).Next
                    .Block = new ExceptionBlock(endBlock);
                switch (ex.Flags)
                {
                    case ExceptionHandlingClauseOptions.Filter:
                        Instruction.FindInstruction(Instructions, ex.FilterOffset)
                            .Block = new ExceptionBlock(beginFilter);
                        break;
                    case ExceptionHandlingClauseOptions.Finally:
                        Instruction.FindInstruction(Instructions, ex.HandlerOffset)
                            .Block = new ExceptionBlock(beginFinally);
                        break;
                    case ExceptionHandlingClauseOptions.Clause:
                        Instruction.FindInstruction(Instructions, ex.HandlerOffset)
                            .Block = new ExceptionBlock(beginCatch, ex.CatchType);
                        break;
                    case ExceptionHandlingClauseOptions.Fault:
                        Instruction.FindInstruction(Instructions, ex.HandlerOffset)
                            .Block = new ExceptionBlock(beginFault);
                        break;
                    default:
                        break;
                }
            }
        }
        public List<MethodInfo> prefixes = new List<MethodInfo>();
        public List<MethodInfo> postfixes = new List<MethodInfo>();
        public void Copy(ILGenerator il)
        {
            PrepareLabels(il);
            PrepareLocals(il);
            var insts = Instructions;
            foreach (var inst in insts)
            {
                var block = inst.Block;
                var label = inst.Label;
                if (block.IsValid)
                    MarkExceptionBlock(il, block);
                if (label.HasValue)
                    il.MarkLabel((Label)label);
                var code = inst.OpCode;
                var op = inst.Operand;
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        op = Instruction.FindInstruction(insts, (int)op).Label;
                        break;
                }
                EmitAuto(il, code, op);
            }
        }
        public void AddPrefix(MethodInfo prefix) => prefixes.Add(prefix);
        public MethodInfo AddPrefix(Delegate prefix)
        {
            MethodInfo wrapper;
            prefixes.Add(wrapper = MakeStaticWrapper(prefix));
            return wrapper;
        }
        public void AddPostfix(MethodInfo postfix) => postfixes.Add(postfix);
        public MethodInfo AddPostfix(Delegate postfix)
        {
            MethodInfo wrapper;
            postfixes.Add(wrapper = MakeStaticWrapper(postfix));
            return wrapper;
        }
        public void RemovePrefix(MethodInfo prefix) => prefixes.Remove(prefix);
        public void RemovePostfix(MethodInfo postfix) => postfixes.Remove(postfix);
        public void Attach(FixOption option = null)
        {
            Replace(Base, MakeFixedMethod(option));
            Attached = true;
        }
        public void Detach()
        {
            Recover(Base);
            Attached = false;
        }
        public void UpdateWrapper(FixOption option = null)
        {
            if (Attached) Detach();
            Attach(option);
        }
        static void EmitAuto(ILGenerator il, OpCode opcode, object operand)
        {
            switch (operand)
            {
                case string i:
                    il.Emit(opcode, i);
                    return;
                case FieldInfo i:
                    il.Emit(opcode, i);
                    return;
                case Label[] i:
                    il.Emit(opcode, i);
                    return;
                case Label i:
                    il.Emit(opcode, i);
                    return;
                case LocalBuilder i:
                    il.Emit(opcode, i);
                    return;
                case float i:
                    il.Emit(opcode, i);
                    return;
                case byte i:
                    il.Emit(opcode, i);
                    return;
                case sbyte i:
                    il.Emit(opcode, i);
                    return;
                case short i:
                    il.Emit(opcode, i);
                    return;
                case double i:
                    il.Emit(opcode, i);
                    return;
                case MethodInfo i:
                    il.Emit(opcode, i);
                    return;
                case int i:
                    il.Emit(opcode, i);
                    return;
                case long i:
                    il.Emit(opcode, i);
                    return;
                case Type i:
                    il.Emit(opcode, i);
                    return;
                case SignatureHelper i:
                    il.Emit(opcode, i);
                    return;
                case ConstructorInfo i:
                    il.Emit(opcode, i);
                    return;
                default:
                    il.Emit(opcode);
                    return;
            }
        }
        static void MarkExceptionBlock(ILGenerator il, ExceptionBlock block)
        {
            ExceptionBlockType btype = block.blockType;
            if ((btype & ExceptionBlockType.End) != 0)
                il.EndExceptionBlock();
            else
                switch (btype & (ExceptionBlockType)0x3F)
                {
                    case ExceptionBlockType.BigBlock:
                        il.BeginExceptionBlock();
                        return;
                    case ExceptionBlockType.FilterBlock:
                        il.BeginExceptFilterBlock();
                        return;
                    case ExceptionBlockType.FinallyBlock:
                        il.BeginFinallyBlock();
                        return;
                    case ExceptionBlockType.CatchBlock:
                        il.BeginCatchBlock(block.catchType);
                        return;
                    case ExceptionBlockType.FaultBlock:
                        il.BeginFaultBlock();
                        return;
                    default:
                        return;
                }
        }
        private IntPtr MakeFixedMethod(FixOption option)
        {
            option = option ?? FixOption.Default;
            var orig = Base;
            var retType = orig is MethodInfo m ? m.ReturnType : typeof(void);
            var parameters = orig.GetParameters();
            var paramTypes = parameters.Select(p => p.ParameterType).ToList();
            var offset = 0;
            var method = new DynamicMethod($"Fix{(FixedMethodCount++ != 0 ? FixedMethodCount.ToString() : "")}", retType, paramTypes.ToArray(), true);
            var decType = orig.DeclaringType;
            if (!orig.IsStatic)
            {
                if (IsStruct(decType))
                    paramTypes.Insert(offset++, decType.MakeByRefType());
                else paramTypes.Insert(offset++, decType);
                Method.paramTypes.SetValue(method, paramTypes.ToArray());
                method.DefineParameter(1, ParameterAttributes.None, "__instance");
            }
            var il = method.GetILGenerator();
            var instructions = Instructions.ToList();
            var hasReturn = retType != typeof(void);
            var retLoc = hasReturn ? il.DeclareLocal(retType) : null;
            var lastInst = instructions.Last();
            var canSkip = prefixes.Any(fix => fix.ReturnType == typeof(bool));
            var skipOrig = canSkip ? il.DeclareLocal(typeof(bool)) : null;
            if (canSkip)
            {
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Stloc, skipOrig);
            }
            var endOfOriginal = il.DefineLabel();
            var body = orig.GetMethodBody();
            var locVars = body.LocalVariables;
            var locCount = locVars.Count;
            var fixes = prefixes.Union(postfixes);
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var pb = method.DefineParameter(i + 1 + offset, param.Attributes, param.Name);
                if (param.HasDefaultValue)
                    pb.SetConstant(param.DefaultValue);
            }
            PrepareLabels(il, instructions, i => i.OpCode != OpCodes.Ret);
            PrepareLocals(il, locVars);
            PrepareArgumentArray(il, orig, parameters, fixes, option, out LocalBuilder argumentArray);
            prefixes.ForEach(fix => EmitFix(orig, fix, il, retLoc, skipOrig, argumentArray, option));
            if (canSkip)
            {
                il.Emit(OpCodes.Ldloc, skipOrig);
                il.Emit(OpCodes.Brfalse, endOfOriginal);
            }
            foreach (var inst in instructions)
            {
                var block = inst.Block;
                var label = inst.Label;
                if (block.IsValid)
                    MarkExceptionBlock(il, block);
                if (label.HasValue)
                    il.MarkLabel((Label)label);
                var code = inst.OpCode;
                var op = inst.Operand;
                if (code == OpCodes.Ret)
                {
                    if (hasReturn)
                    {
                        code = OpCodes.Stloc;
                        op = retLoc;
                    }
                    else code = OpCodes.Nop;
                    if (!ReferenceEquals(lastInst, inst))
                        il.Emit(OpCodes.Br, endOfOriginal);
                }
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        if (!(op is Label))
                            op = Instruction.FindInstruction(instructions, (int)op).Label;
                        break;
                }
                EmitAuto(il, code, op);
            }
            il.MarkLabel(endOfOriginal);
            postfixes.ForEach(fix => EmitFix(orig, fix, il, retLoc, skipOrig, argumentArray, option));
            if (hasReturn)
                il.Emit(OpCodes.Ldloc, retLoc);
            il.Emit(OpCodes.Ret);
            return Compile(method).GetFunctionPointer();
        }
        private void EmitFix(MethodBase orig, MethodInfo fix, ILGenerator il, LocalBuilder retLoc, LocalBuilder skipOrig, LocalBuilder argumentArray, FixOption option = null)
        {
            var isSkippable = fix.ReturnType == typeof(bool);
            var origParams = orig.GetParameters();
            var origDecType = orig.DeclaringType;
            var fixParams = fix.GetParameters();
            var privVars = new Dictionary<string, int>();
            bool isStatic;
            var offset = (isStatic = orig.IsStatic) ? 0 : 1;
            for (int i = 0; i < origParams.Length; i++)
            {
                var param = origParams[i];
                privVars.Add(param.Name, i + offset);
            }
            for (int i = 0; i < fixParams.Length; i++)
            {
                var param = fixParams[i];
                var paramType = param.ParameterType;
                var name = param.Name;
                if (privVars.TryGetValue(name, out int index))
                {
                    if (paramType.IsByRef)
                        il.Emit(OpCodes.Ldarga, index);
                    else
                        il.Emit(OpCodes.Ldarg, index);
                }
                else if (name == option.Instance)
                {
                    if (isStatic)
                    {
                        il.Emit(OpCodes.Ldnull);
                        continue;
                    }
                    if (paramType.IsByRef)
                        il.Emit(OpCodes.Ldarga, 0);
                    else il.Emit(OpCodes.Ldarg_0);
                    BoxIfNeeded(origDecType, paramType);
                }
                else if (name == option.Result)
                {
                    if (paramType.IsByRef)
                        il.Emit(OpCodes.Ldloca, retLoc);
                    else il.Emit(OpCodes.Ldloc, retLoc);
                    BoxIfNeeded(retLoc.LocalType, paramType);
                }
                else if (name == option.RunOriginal)
                {
                    if (skipOrig != null)
                        il.Emit(OpCodes.Ldloc, skipOrig);
                    else il.Emit(OpCodes.Ldc_I4_0);
                    BoxIfNeeded(skipOrig.LocalType, paramType);
                }
                else if (name == option.OriginalMethod)
                {
                    if (orig is MethodInfo method)
                        il.Emit(OpCodes.Ldtoken, method);
                    else if (orig is ConstructorInfo constructor)
                        il.Emit(OpCodes.Ldtoken, constructor);
                    else il.Emit(OpCodes.Ldnull);
                    var type = orig.ReflectedType;
                    if (type.IsGenericType) il.Emit(OpCodes.Ldtoken, type);
                    il.Emit(OpCodes.Call, type.IsGenericType ? gmfhGeneric : gmfh);
                    BoxIfNeeded(typeof(MethodBase), paramType);
                }
                else if (name == option.Args)
                {
                    if (argumentArray != null)
                        il.Emit(OpCodes.Ldloc, argumentArray);
                    else il.Emit(OpCodes.Ldnull);
                }
                else if (name.StartsWith(option.FieldPrefix))
                {
                    var fldName = name.Substring(option.FieldPrefix.Length);
                    var fld = origDecType.GetField(fldName, (BindingFlags)15420);
                    if (fld == null) throw new NullReferenceException($"Cannot Find Field {fldName}!");
                    if (fld.IsStatic)
                    {
                        if (paramType.IsByRef)
                            il.Emit(OpCodes.Ldsflda, fld);
                        else il.Emit(OpCodes.Ldsfld, fld);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        if (paramType.IsByRef)
                            il.Emit(OpCodes.Ldflda, fld);
                        else il.Emit(OpCodes.Ldfld, fld);
                    }
                    BoxIfNeeded(fld.FieldType, paramType);
                }
                else throw new ArgumentException($"Invalid Argument ({name})");
            }
            il.Emit(OpCodes.Call, fix);
            if (orig.GetParameters().Any(p => p.Name == option.Args))
                RestoreArgumentArray(argumentArray, orig.GetParameters(), orig, il);
            if (isSkippable)
                il.Emit(OpCodes.Stloc, skipOrig);
            void BoxIfNeeded(Type origType, Type fixType)
            {
                if (origType != fixType && fixType.IsAssignableFrom(origType))
                {
                    if (origType.IsClass)
                        il.Emit(OpCodes.Castclass, fixType);
                    else il.Emit(OpCodes.Box, fixType);
                }
            }
        }
        public bool Attached;
        static readonly AssemblyBuilder delWrapper = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("delWrapper"), AssemblyBuilderAccess.Run);
        static readonly ModuleBuilder mod = delWrapper.DefineDynamicModule("delWrapper");
        public static MethodInfo MakeStaticWrapper(Delegate del)
        {
            var delType = del.GetType();
            var invoke = delType.GetMethod("Invoke");
            var method = del.Method;
            var parameters = method.GetParameters();
            var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            var returnType = invoke.ReturnType;
            var t = mod.DefineType($"StaticWrapper{WrapperCount++}");
            var fld = t.DefineField("del", delType, FieldAttributes.Public | FieldAttributes.Static);
            var dm = t.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, returnType, paramTypes);
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                dm.DefineParameter(i + 1, param.Attributes, param.Name);
            }
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, fld);
            for (int i = 0; i < paramTypes.Length; i++) il.Emit(OpCodes.Ldarg, i);
            il.Emit(OpCodes.Call, invoke);
            il.Emit(OpCodes.Ret);
            var ct = t.CreateType();
            var result = ct.GetMethod("Wrapper");
            ct.GetField("del").SetValue(null, del);
            return result;
        }
        static void PrepareArgumentArray(ILGenerator il, MethodBase orig, ParameterInfo[] parameters, IEnumerable<MethodInfo> fixes, FixOption option, out LocalBuilder argumentArray)
        {
            if (fixes.Any(f => f.GetParameters().Any(p => p.Name == option.Args)))
            {
                var i = 0;
                foreach (var pInfo in parameters)
                {
                    var argIndex = i++ + (orig.IsStatic ? 0 : 1);
                    if (pInfo.IsOut || pInfo.IsRetval)
                        InitializeOutParameter(il, argIndex, pInfo.ParameterType);
                }
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                i = 0;
                var arrayIdx = 0;
                foreach (var pInfo in parameters)
                {
                    var argIndex = i++ + (orig.IsStatic ? 0 : 1);
                    var pType = pInfo.ParameterType;
                    var paramByRef = pType.IsByRef;
                    if (paramByRef) pType = pType.GetElementType();
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4, arrayIdx++);
                    il.Emit(OpCodes.Ldarg, argIndex);
                    if (paramByRef)
                    {
                        if (IsStruct(pType))
                            il.Emit(OpCodes.Ldobj, pType);
                        else
                            il.Emit(LoadIndOpCodeFor(pType));
                    }
                    if (pType.IsValueType)
                        il.Emit(OpCodes.Box, pType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
                argumentArray = il.DeclareLocal(typeof(object[]));
                il.Emit(OpCodes.Stloc, argumentArray);
            }
            else argumentArray = null;
        }
        static void RestoreArgumentArray(LocalBuilder argumentArray, ParameterInfo[] parameters, MethodBase orig, ILGenerator il)
        {
            var i = 0;
            var arrayIdx = 0;
            foreach (var pInfo in parameters)
            {
                var argIndex = i++ + (orig.IsStatic ? 0 : 1);
                var pType = pInfo.ParameterType;
                if (pType.IsByRef)
                {
                    pType = pType.GetElementType();
                    il.Emit(OpCodes.Ldarg, argIndex);
                    il.Emit(OpCodes.Ldloc, argumentArray);
                    il.Emit(OpCodes.Ldc_I4, arrayIdx);
                    il.Emit(OpCodes.Ldelem_Ref);
                    if (pType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, pType);
                        if (IsStruct(pType))
                            il.Emit(OpCodes.Stobj, pType);
                        else
                            il.Emit(StoreIndOpCodeFor(pType));
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, pType);
                        il.Emit(OpCodes.Stind_Ref);
                    }
                }
                arrayIdx++;
            }
        }
        static void InitializeOutParameter(ILGenerator il, int argIndex, Type type)
        {
            if (type.IsByRef) type = type.GetElementType();
            il.Emit(OpCodes.Ldarg, argIndex);
            if (IsStruct(type))
            {
                il.Emit(OpCodes.Initobj, type);
                return;
            }
            if (IsValue(type))
            {
                if (type == typeof(float))
                {
                    il.Emit(OpCodes.Ldc_R4, (float)0);
                    il.Emit(OpCodes.Stind_R4);
                    return;
                }
                else if (type == typeof(double))
                {
                    il.Emit(OpCodes.Ldc_R8, (double)0);
                    il.Emit(OpCodes.Stind_R8);
                    return;
                }
                else if (type == typeof(long))
                {
                    il.Emit(OpCodes.Ldc_I8, (long)0);
                    il.Emit(OpCodes.Stind_I8);
                    return;
                }
                else
                {
                    il.Emit(OpCodes.Ldc_I4, 0);
                    il.Emit(OpCodes.Stind_I4);
                    return;
                }
            }
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Stind_Ref);
        }
        static int FixedMethodCount;
        static int WrapperCount;
        static bool IsStruct(Type type) => type.IsValueType && !(type.IsPrimitive || type.IsEnum) && type != typeof(void);
        static bool IsValue(Type type) => type.IsPrimitive || type.IsEnum;
        private static OpCode LoadIndOpCodeFor(Type type)
        {
            if (type.IsEnum) return OpCodes.Ldind_I4;
            if (type == typeof(float)) return OpCodes.Ldind_R4;
            if (type == typeof(double)) return OpCodes.Ldind_R8;
            if (type == typeof(byte)) return OpCodes.Ldind_U1;
            if (type == typeof(ushort)) return OpCodes.Ldind_U2;
            if (type == typeof(uint)) return OpCodes.Ldind_U4;
            if (type == typeof(ulong)) return OpCodes.Ldind_I8;
            if (type == typeof(sbyte)) return OpCodes.Ldind_I1;
            if (type == typeof(short)) return OpCodes.Ldind_I2;
            if (type == typeof(int)) return OpCodes.Ldind_I4;
            if (type == typeof(long)) return OpCodes.Ldind_I8;
            return OpCodes.Ldind_Ref;
        }
        private static OpCode StoreIndOpCodeFor(Type type)
        {
            if (type.IsEnum) return OpCodes.Stind_I4;
            if (type == typeof(float)) return OpCodes.Stind_R4;
            if (type == typeof(double)) return OpCodes.Stind_R8;
            if (type == typeof(byte)) return OpCodes.Stind_I1;
            if (type == typeof(ushort)) return OpCodes.Stind_I2;
            if (type == typeof(uint)) return OpCodes.Stind_I4;
            if (type == typeof(ulong)) return OpCodes.Stind_I8;
            if (type == typeof(sbyte)) return OpCodes.Stind_I1;
            if (type == typeof(short)) return OpCodes.Stind_I2;
            if (type == typeof(int)) return OpCodes.Stind_I4;
            if (type == typeof(long)) return OpCodes.Stind_I8;
            return OpCodes.Stind_Ref;
        }
        static readonly MethodInfo gmfh = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
        static readonly MethodInfo gmfhGeneric = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });
        void PrepareLocals(ILGenerator il)
                => PrepareLocals(il, LocalVars);
        void PrepareLabels(ILGenerator il)
            => PrepareLabels(il, Instructions);
        static void PrepareLocals(ILGenerator il, IList<LocalVariableInfo> localVars)
        {
            for (int i = 0; i < localVars.Count; i++)
            {
                var localVar = localVars[i];
                il.DeclareLocal(localVar.LocalType, localVar.IsPinned);
            }
        }
        static void PrepareLabels(ILGenerator il, List<Instruction> instructions, Predicate<Instruction> condition)
        {
            for (int x = 0; x < instructions.Count - 1; x++)
            {
                Instruction buf = instructions[x];
                switch (buf.OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        if (condition(buf))
                            Instruction.FindInstruction(instructions, (int)buf.Operand).Label = il.DefineLabel();
                        break;
                    case OperandType.InlineSwitch:
                        foreach (int i in (int[])buf.Operand)
                            if (condition(buf))
                                Instruction.FindInstruction(instructions, i).Label = il.DefineLabel();
                        break;
                    default:
                        break;
                }
            }
        }
        static void PrepareLabels(ILGenerator il, List<Instruction> instructions)
        {
            for (int x = 0; x < instructions.Count - 1; x++)
            {
                Instruction buf = instructions[x];
                switch (buf.OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        Instruction.FindInstruction(instructions, (int)buf.Operand).Label = il.DefineLabel();
                        break;

                    case OperandType.InlineSwitch:
                        foreach (int i in (int[])buf.Operand)
                            Instruction.FindInstruction(instructions, i).Label = il.DefineLabel();
                        break;

                    default:
                        break;
                }
            }
        }
        OpCode ReadOpCode()
        {
            byte op = Buffer.ReadByte();
            return op != 0xfe
                ? OneByteOpCodes[op]
                : TwoBytesOpCodes[Buffer.ReadByte()];
        }
        void ReadOperand(Instruction instruction)
        {
            switch (instruction.OpCode.OperandType)
            {
                case OperandType.InlineNone:
                    break;
                case OperandType.InlineSwitch:
                    int length = Buffer.ReadInt32();
                    int base_offset = Buffer.Position + (4 * length);
                    int[] branches = new int[length];
                    for (int i = 0; i < length; i++)
                        branches[i] = Buffer.ReadInt32() + base_offset;
                    instruction.Operand = branches;
                    break;
                case OperandType.ShortInlineBrTarget:
                    instruction.Operand = ((sbyte)Buffer.ReadByte()) + Buffer.Position;
                    break;
                case OperandType.InlineBrTarget:
                    instruction.Operand = Buffer.ReadInt32() + Buffer.Position;
                    break;
                case OperandType.ShortInlineI:
                    if (instruction.OpCode == OpCodes.Ldc_I4_S)
                        instruction.Operand = (sbyte)Buffer.ReadByte();
                    else
                        instruction.Operand = Buffer.ReadByte();
                    break;
                case OperandType.InlineI:
                    instruction.Operand = Buffer.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    instruction.Operand = Buffer.ReadSingle();
                    break;
                case OperandType.InlineR:
                    instruction.Operand = Buffer.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    instruction.Operand = Buffer.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    instruction.Operand = Buffer.ReadInt32();
                    //instruction.Operand = Module.ResolveSignature(Buffer.ReadInt32());
                    break;
                case OperandType.InlineString:
                    instruction.Operand = Module.ResolveString(Buffer.ReadInt32());
                    break;
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                    instruction.Operand = Module.ResolveMember(Buffer.ReadInt32(), TypeGenerics, MethodGenerics);
                    break;
                case OperandType.ShortInlineVar:
                    instruction.Operand = Buffer.ReadByte();
                    break;
                case OperandType.InlineVar:
                    instruction.Operand = Buffer.ReadInt16();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public object Invoke(object instance, params object[] parameters) => Base.Invoke(instance, parameters);
        public string GetInstructions()
        {
            StringBuilder str = new StringBuilder();
            foreach (var inst in Instructions)
                str.AppendLine(inst.ToString());
            return str.ToString();
        }
        public static Method GetMethod(Type type, string name, bool declaredOnly = false)
            => type.GetMethod(name, declaredOnly ? (BindingFlags)15422 : (BindingFlags)15420);
        public static Method GetMethod(Type type, string name, BindingFlags bindingFlags)
           => type.GetMethod(name, bindingFlags);
        public static Method GetMethod(Type type, string name, Type[] parameterTypes, bool declaredOnly = false)
            => type.GetMethod(name, declaredOnly ? (BindingFlags)15422 : (BindingFlags)15420, null, parameterTypes, null);
        public static Method GetMethod(Type type, string name, Type[] parameterTypes, BindingFlags bindingFlags)
           => type.GetMethod(name, bindingFlags, null, parameterTypes, null);
        public static Method GetConstructor(Type type, params Type[] parameterTypes) => type.GetConstructor(All, null, parameterTypes, null);
        public static Method GetMethod(MethodBase method)
        {
            if (method == null) return null;
            if (CacheM.TryGetValue(method, out Method meth))
                return meth;
            else return CacheM[method] = new Method(method);
        }
        public static readonly BindingFlags All = (BindingFlags)15420;
        public static readonly BindingFlags AllDeclared = (BindingFlags)15422;
        internal static readonly Dictionary<MethodBase, Method> CacheM = new Dictionary<MethodBase, Method>();
        public static implicit operator Method(MethodBase method) => GetMethod(method);
        public static implicit operator MethodBase(Method method) => method.Base;
        public static implicit operator MethodInfo(Method method) => method.Base as MethodInfo;
        public static implicit operator ConstructorInfo(Method method) => method.Base as ConstructorInfo;
        #region Utils
        public static RuntimeMethodHandle Compile(DynamicMethod dynMethod)
        {
            if (IsMonoRuntime)
            {
                MonoCreateDM.Invoke(dynMethod, null);
                return (RuntimeMethodHandle)MonoHandleDM.GetValue(dynMethod);
            }
            return (RuntimeMethodHandle)NetCreateDM.Invoke(dynMethod, null);
        }
        static readonly Dictionary<MethodBase, List<ushort>> Cache = new Dictionary<MethodBase, List<ushort>>();
        public static unsafe void TryNoInlining(MethodBase method)
        {
            if (Type.GetType("Mono.Runtime") != null)
                *((ushort*)method.MethodHandle.Value + 1) |= 8;
        }
        public static unsafe void Replace(MethodBase target, MethodBase method)
        {
            TryNoInlining(target);
            var source = target.MethodHandle.GetFunctionPointer();
            var dest = method.MethodHandle.GetFunctionPointer();
            if (Environment.OSVersion.Platform < PlatformID.Unix)
                VirtualProtect(source, new IntPtr(1), 0x40, out int dummy);
            byte* src = (byte*)source;
            List<ushort> cache = new List<ushort>();
            if (IntPtr.Size == sizeof(long))
            {
                if (*src == 0xE9)
                    src += *(int*)(src + 1) + 5;
                cache.Add(*(ushort*)src);
                src = Write(src, (ushort)0xB848);
                cache.Add(*(ushort*)src);
                src = Write(src, dest.ToInt64());
                cache.Add(*(ushort*)src);
                Write(src, (ushort)0xE0FF);
            }
            else
            {
                cache.Add(*src);
                src = Write(src, (byte)0x68);
                cache.Add(*src);
                src = Write(src, dest.ToInt32());
                cache.Add(*src);
                Write(src, (byte)0xC3);
            }
            if (!Cache.ContainsKey(target))
                Cache.Add(target, cache);
        }
        public static unsafe void Replace(MethodBase target, IntPtr method)
        {
            TryNoInlining(target);
            var source = target.MethodHandle.GetFunctionPointer();
            var dest = method;
            if (Environment.OSVersion.Platform < PlatformID.Unix)
                VirtualProtect(source, new IntPtr(1), 0x40, out int dummy);
            byte* src = (byte*)source;
            List<ushort> cache = new List<ushort>();
            if (IntPtr.Size == sizeof(long))
            {
                if (*src == 0xE9)
                    src += *(int*)(src + 1) + 5;
                cache.Add(*(ushort*)src);
                src = Write(src, (ushort)0xB848);
                cache.Add(*(ushort*)src);
                src = Write(src, dest.ToInt64());
                cache.Add(*(ushort*)src);
                Write(src, (ushort)0xE0FF);
            }
            else
            {
                cache.Add(*src);
                src = Write(src, (byte)0x68);
                cache.Add(*src);
                src = Write(src, dest.ToInt32());
                cache.Add(*src);
                Write(src, (byte)0xC3);
            }
            if (!Cache.ContainsKey(target))
                Cache.Add(target, cache);
        }
        public static unsafe void Recover(MethodBase target)
        {
            var source = target.MethodHandle.GetFunctionPointer();
            byte* src = (byte*)source;
            var cache = Cache[target];
            if (IntPtr.Size == sizeof(long))
            {
                if (*src == 0xE9)
                    src += *(int*)(src + 1) + 5;
                src = Write(src, cache[0]);
                src = Write(src, cache[1]);
                Write(src, cache[2]);
            }
            else
            {
                src = Write(src, (byte)cache[0]);
                src = Write(src, (byte)cache[1]);
                Write(src, (byte)cache[2]);
            }
        }
        public static unsafe byte* Write<T>(byte* ptr, T value) where T : unmanaged
        {
            *(T*)ptr = value;
            return ptr + sizeof(T);
        }
        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, int flNewProtect, out int lpflOldProtect);
        #endregion
    }
    public sealed class ByteBuffer
    {
        internal byte[] buffer;
        public int Position;
        public ByteBuffer(byte[] buffer)
            => this.buffer = buffer;
        public byte ReadByte()
        {
            CheckCanRead(1);
            return buffer[Position++];
        }
        public byte[] ReadBytes(int length)
        {
            CheckCanRead(length);
            var value = new byte[length];
            Buffer.BlockCopy(buffer, Position, value, 0, length);
            Position += length;
            return value;
        }
        public short ReadInt16()
        {
            CheckCanRead(2);
            short value = (short)(buffer[Position]
                | (buffer[Position + 1] << 8));
            Position += 2;
            return value;
        }
        public int ReadInt32()
        {
            CheckCanRead(4);
            int value = buffer[Position]
                | (buffer[Position + 1] << 8)
                | (buffer[Position + 2] << 16)
                | (buffer[Position + 3] << 24);
            Position += 4;
            return value;
        }
        public long ReadInt64()
        {
            CheckCanRead(8);
            uint low = (uint)(buffer[Position]
                | (buffer[Position + 1] << 8)
                | (buffer[Position + 2] << 16)
                | (buffer[Position + 3] << 24));

            uint high = (uint)(buffer[Position + 4]
                | (buffer[Position + 5] << 8)
                | (buffer[Position + 6] << 16)
                | (buffer[Position + 7] << 24));

            long value = (((long)high) << 32) | low;
            Position += 8;
            return value;
        }
        public float ReadSingle()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(4);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }

            CheckCanRead(4);
            float value = BitConverter.ToSingle(buffer, Position);
            Position += 4;
            return value;
        }
        public double ReadDouble()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(8);
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }

            CheckCanRead(8);
            double value = BitConverter.ToDouble(buffer, Position);
            Position += 8;
            return value;
        }
        void CheckCanRead(int count)
        {
            if (Position + count > buffer.Length)
                throw new ArgumentOutOfRangeException();
        }
    }
    public readonly struct ExceptionBlock
    {
        public static readonly ExceptionBlock Empty;
        public readonly ExceptionBlockType blockType;
        public readonly Type catchType;
        public bool IsValid => blockType > 0;
        public ExceptionBlock(ExceptionBlockType blockType, Type catchType)
        {
            this.blockType = blockType;
            this.catchType = catchType;
        }
        public ExceptionBlock(ExceptionBlockType blockType) : this(blockType, null) { }
        public override string ToString()
        {
            if ((blockType & ExceptionBlockType.End) != 0)
                return "end";
            else
                switch (blockType & (ExceptionBlockType)0x3F)
                {
                    case ExceptionBlockType.BigBlock:
                        return "try";
                    case ExceptionBlockType.FilterBlock:
                        return "filter";
                    case ExceptionBlockType.FinallyBlock:
                        return "finally";
                    case ExceptionBlockType.CatchBlock:
                        return $"catch({catchType})";
                    case ExceptionBlockType.FaultBlock:
                        return "fault";
                    default:
                        return "unknown";
                }
        }
    }
    [Flags]
    public enum ExceptionBlockType
    {
        Begin = 0x80,
        End = 0x40,
        None = 0,
        BigBlock = 0x1,
        FilterBlock = 0x2,
        FinallyBlock = 0x4,
        CatchBlock = 0x8,
        FaultBlock = 0x10,
    }
    public sealed class Instruction
    {
        int offset;
        OpCode opcode;
        object operand;
        Instruction previous;
        Instruction next;
        ExceptionBlock block;
        Label? label;
        public int Offset
        {
            get => offset;
            set => offset = value;
        }
        public OpCode OpCode
        {
            get => opcode;
            set => opcode = value;
        }
        public object Operand
        {
            get => operand;
            set => operand = value;
        }
        public ExceptionBlock Block
        {
            get => block;
            set => block = value;
        }
        public Label? Label
        {
            get => label;
            set => label = value;
        }
        public Instruction Previous
        {
            get => previous;
            set => previous = value;
        }
        public Instruction Next
        {
            get => next;
            set => next = value;
        }
        public int Size
        {
            get
            {
                int size = OpCode.Size;

                switch (OpCode.OperandType)
                {
                    case OperandType.InlineSwitch:
                        size += (1 + ((Instruction[])Operand).Length) * 4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        size += 8;
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        size += 4;
                        break;
                    case OperandType.InlineVar:
                        size += 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        size += 1;
                        break;
                }

                return size;
            }
        }
        public Instruction(OpCode opcode, object operand = null)
        {
            this.opcode = opcode;
            this.operand = operand;
            offset = -1;
        }
        public Instruction(int offset, OpCode opcode)
        {
            this.offset = offset;
            this.opcode = opcode;
        }
        public override string ToString()
        {
            if (Operand == null)
                return $"IL_{offset:X4}:{OpCode}";
            switch (OpCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    return $"IL_{offset:X4}:{OpCode} IL_{Operand:X4} ({Operand.GetType()})";
                default:
                    return $"IL_{offset:X4}:{OpCode} {Operand} ({Operand.GetType()})";
            }
        }
        public static Instruction FindInstruction(IReadOnlyList<Instruction> instructions, int offset, bool throwIfNull = false)
        {
            int lastIdx = instructions.Count - 1;
            int min = 0, max = lastIdx;
            while (min <= max)
            {
                int mid = min + (max - min) / 2;
                Instruction current = instructions[mid];
                if (current.offset == offset) return current;
                if (offset < current.offset) max = mid - 1;
                else min = mid + 1;
            }
            if (throwIfNull) throw null;
            else return null;
        }
    }
    public class FixOption
    {
        public string Instance = "__instance";
        public string FieldPrefix = "___";
        public string OriginalMethod = "__originalMethod";
        public string Result = "__result";
        public string RunOriginal = "__runOriginal";
        public string Args = "__args";
        public static readonly FixOption Default = new FixOption();
    }
}
