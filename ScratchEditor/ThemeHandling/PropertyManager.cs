using System.Collections.Generic;
using System.Windows.Media;

namespace ScratchEditor.ThemeHandling
{
    public class PropertyManager
    {
#region Themes

        private static Theme _theme = Theme.Dark;

        public void setTheme(Theme theme)
        {
            _theme = theme;
        }
        

#endregion
        
#region ColorDicts

        private static Dictionary<ColorIdentifier, Color> _colors_ThemeDark = new Dictionary<ColorIdentifier, Color>()
        {
            {ColorIdentifier.Background, Color.FromRgb(40,40,40)},
            {ColorIdentifier.Line1, Color.FromRgb(20,20,20)},
            {ColorIdentifier.Line2, Color.FromRgb(10,10,10)},
            {ColorIdentifier.Bounding, Color.FromRgb(25,25,100)},
            {ColorIdentifier.Node_BG, Color.FromRgb(45,45,45)},
            {ColorIdentifier.TilteText, Color.FromRgb(255,255,255)},
            {ColorIdentifier.InserterOutline, Color.FromRgb(100,255,50)}
        };
        private static Dictionary<ColorIdentifier, Color> _colors_ThemeLight = new Dictionary<ColorIdentifier, Color>()
        {
            
        };
        private static Dictionary<ColorIdentifier, Color> _colors_ThemeContrast = new Dictionary<ColorIdentifier, Color>()
        {
            
        };
#endregion
#region ColorLogic
        public static Color getColor(ColorIdentifier identifier)
        {
            switch (_theme)
            {
                case Theme.Dark:
                    if (_colors_ThemeDark.ContainsKey(identifier))
                        return _colors_ThemeDark[identifier];
                    break;
                case Theme.Light:
                    if (_colors_ThemeLight.ContainsKey(identifier))
                        return _colors_ThemeLight[identifier];
                    break;
                case Theme.Contrast:
                    if (_colors_ThemeContrast.ContainsKey(identifier))
                        return _colors_ThemeContrast[identifier];
                    break;
            }

            if (_colors_ThemeDark.ContainsKey(identifier))
                return _colors_ThemeDark[identifier];

            return Colors.Purple;
        }
#endregion

        
#region Singlevalues

        public static LineType LineType { get; set; } = LineType.Bezier;


#endregion
    }

    public enum ColorIdentifier
    {
        Background,
        Line1,
        Line2,
        Bounding,
        Node_BG,
        TilteText,
        InserterOutline
    }

    public enum Theme
    {
        Dark,
        Light,
        Contrast
    }

    public enum LineType
    {
        Line,
        Bezier
    }
}