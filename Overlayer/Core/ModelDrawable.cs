using Overlayer.Core.Interfaces;

namespace Overlayer.Core
{
    public abstract class ModelDrawable<T> : IDrawable where T : IModel
    {
        public T model;
        public string Name { get; protected set; }
        public ModelDrawable(T model, string name)
        {
            this.model = model;
            Name = name;
        }
        public abstract void Draw();
        protected static string L(string translationKey, params object[] formatArgs)
        {
            if (formatArgs.Length == 0)
                return Main.Lang[translationKey];
            return string.Format(Main.Lang[translationKey], formatArgs);
        }
    }
}
