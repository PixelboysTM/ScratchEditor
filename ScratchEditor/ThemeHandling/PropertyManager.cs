using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ScratchEditor.ThemeHandling
{
    public static class PropertyManager
    {
#region Themes

        private static Theme _theme = Theme.Dark;

        public static void setTheme(Theme theme)
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
            {ColorIdentifier.InserterOutline, Color.FromRgb(100,255,50)},
            {ColorIdentifier.BoolType, Color.FromRgb(255,100,100)},
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

#region IntDicts

        private static Dictionary<IntIdentifier, int> _int_ThemeDark = new Dictionary<IntIdentifier, int>()
        {
            
        };
        private static Dictionary<IntIdentifier, int> _int_ThemeLight = new Dictionary<IntIdentifier, int>()
        {
            
        };
        private static Dictionary<IntIdentifier, int> _int_ThemeContrast = new Dictionary<IntIdentifier, int>()
        {
            
        };

#endregion

#region IntLogic
        public static int getInt(IntIdentifier identifier)
        {
            switch (_theme)
            {
                case Theme.Dark:
                    if (_int_ThemeDark.ContainsKey(identifier))
                        return _int_ThemeDark[identifier];
                    break;
                case Theme.Light:
                    if (_int_ThemeLight.ContainsKey(identifier))
                        return _int_ThemeLight[identifier];
                    break;
                case Theme.Contrast:
                    if (_int_ThemeContrast.ContainsKey(identifier))
                        return _int_ThemeContrast[identifier];
                    break;
            }

            if (_int_ThemeDark.ContainsKey(identifier))
                return _int_ThemeDark[identifier];

            return 1;
        }
        

#endregion
        
#region DoubleDicts

        private static Dictionary<DoubleIdentifier, double> _double_ThemeDark =
            new Dictionary<DoubleIdentifier, double>()
        {
            {DoubleIdentifier.GridThickness, 0.2},
            {DoubleIdentifier.BezierStandartLenght, 200.0},
            {DoubleIdentifier.ConnectionLineThickness, 2},
            {DoubleIdentifier.ConnectionBezierThickness, 1},
            {DoubleIdentifier.ConnectionLineStraight, 20},
            {DoubleIdentifier.InserterOutlineThickness, 0.9},
            {DoubleIdentifier.InserterSeperatorThickness, 1.1},
            {DoubleIdentifier.InserterCornerRadius, 8},
            {DoubleIdentifier.WindowBoundingThickness, 4.6}
        };
        private static Dictionary<DoubleIdentifier, double> _double_ThemeLight = new Dictionary<DoubleIdentifier, double>()
        {
            
        };
        private static Dictionary<DoubleIdentifier, double> _double_ThemeContrast = new Dictionary<DoubleIdentifier, double>()
        {
            
        };

#endregion

#region DoubleLogic
        public static double getDouble(DoubleIdentifier identifier)
        {
            switch (_theme)
            {
                case Theme.Dark:
                    if (_double_ThemeDark.ContainsKey(identifier))
                        return _double_ThemeDark[identifier];
                    break;
                case Theme.Light:
                    if (_double_ThemeLight.ContainsKey(identifier))
                        return _double_ThemeLight[identifier];
                    break;
                case Theme.Contrast:
                    if (_double_ThemeContrast.ContainsKey(identifier))
                        return _double_ThemeContrast[identifier];
                    break;
            }

            if (_double_ThemeDark.ContainsKey(identifier))
                return _double_ThemeDark[identifier];

            return 1;
        }
        

#endregion
        
#region Singlevalues

        public static LineType LineType
        {
            get
            {
                switch (_theme)
                {
                    case Theme.Dark:
                        return LineType.Line;
                    case Theme.Light:
                        return LineType.Bezier;
                    case Theme.Contrast:
                        return LineType.Line;
                    default:
                        return LineType.Bezier;
                }
            }
            
        }


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
        InserterOutline,
        BoolType
    }

    public enum IntIdentifier
    {
        
    }

    public enum DoubleIdentifier
    {
        WindowBoundingThickness,
        GridThickness,
        BezierStandartLenght,
        ConnectionLineThickness,
        ConnectionBezierThickness,
        ConnectionLineStraight,
        InserterOutlineThickness,
        InserterSeperatorThickness,
        InserterCornerRadius
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