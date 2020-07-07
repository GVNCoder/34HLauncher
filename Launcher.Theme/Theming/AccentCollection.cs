using System.Collections.Generic;
using System.Windows.Media;

namespace Launcher.XamlThemes.Theming
{
    public static class AccentCollection
    {
        public static IDictionary<AccentEnum, Color> Accents { get; }

        static AccentCollection()
        {
            Accents = new Dictionary<AccentEnum, Color>
            {
                { AccentEnum.OrangeRed, (Color) ColorConverter.ConvertFromString("#FF421A") },
                { AccentEnum.Red, (Color) ColorConverter.ConvertFromString("#F1000C") },
                //{ AccentEnum.Violet, (Color) ColorConverter.ConvertFromString("#9A00B5") }
            };
        }
    }
}