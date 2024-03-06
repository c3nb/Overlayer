using Overlayer.Core.Interfaces;
using System;

namespace Overlayer.Core
{
    public class MethodDrawable : IDrawable
    {
        public string Name { get; set; }
        public Action drawerMethod { get; set; }
        public void Draw() => drawerMethod?.Invoke();
        public MethodDrawable(Action drawer, string name)
        {
            drawerMethod = drawer;
            Name = name;
        }
    }
}
