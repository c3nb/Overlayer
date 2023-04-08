namespace Overlayer.Scripting
{
    public abstract class Impl
    {
        public abstract ScriptType ScriptType { get; }
        public abstract string Generate();
    }
}
