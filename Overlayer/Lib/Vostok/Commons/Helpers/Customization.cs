using System;
using JetBrains.Annotations;

// ReSharper disable ParameterHidesMember

namespace Vostok.Commons.Helpers
{
    [PublicAPI]
    internal class Customization<T>
    {
        private Func<T, T> customization;

        public void AddCustomization(Action<T> customization)
        {
            AddCustomization(
                settings =>
                {
                    customization(settings);
                    return settings;
                });
        }

        public void AddCustomization(Func<T, T> customization)
        {
            var previousCustomization = this.customization;

            this.customization =
                settings =>
                    customization(
                        previousCustomization == null
                            ? settings
                            : previousCustomization(settings));
        }

        public T Customize(T settings)
        {
            if (customization != null)
                settings = customization(settings);
            return settings;
        }
    }
}