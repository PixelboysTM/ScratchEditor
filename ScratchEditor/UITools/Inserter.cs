using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScratchEditor.misc;
using ScratchEditor.ThemeHandling;
using ScratchEditor.UI;

namespace ScratchEditor.UITools
{
    public class Inserter
    {
        private Size _size;
        private Point _pos;
        private readonly List<Tuple<string, Type>> _nodeDict = new List<Tuple<string, Type>>();
        private string _searchString = "";
        private readonly ImageSource _searchIcon;
        private List<Tuple<Rect,bool, string, Type>> _rects = new List<Tuple<Rect,bool, string, Type>>();
        private string _path = "";
        private List<string> _drawnFolders;
        private int _scroll;
        
        public Inserter(Size size)
        {
            _size = size;
            _searchIcon = ((Image) Application.Current.Resources["search"]).Source;
        }

        public void AddNodeElement<T>(string location) where T : Node
        {
            _nodeDict.Add(new Tuple<string, Type>(location, typeof(T)));
        }

        public void SetNextSpot(Point point, Object filter) //TODO: Add ability for filter for connections;
        {
            _pos = point;
        }

        public void PerformKeyPress(Key k)
        {
            
        }

        public void PerformScroll(int amount)
        {
            _scroll += (amount / 100);
            Console.WriteLine(_scroll);
        }

        public Tuple<bool, Node> PerformClick(Point p)
        {
            string sel = string.Empty;
            bool folder = true;
            Type t = null;
            foreach (var rect in _rects)
            {
                if (rect.Item1.Contains(p))
                {
                    //If Folder
                    if (rect.Item3 == "\nReverse")
                    {
                        _path = _path.RemoveLastPart('/');
                        return new Tuple<bool, Node>(false, null);
                    }
                    
                    sel = rect.Item3;
                    folder = rect.Item2;
                    t = rect.Item4;
                }
            }

            if (sel == string.Empty)
            {
                return new Tuple<bool, Node>(true, null);
            }

            if (folder)
            {
                _path += sel + "/";
                return new Tuple<bool, Node>(false, null);
            }
            return new Tuple<bool, Node>(true,(Node) Activator.CreateInstance(t, _pos.X, _pos.Y ));
            
        }
        
        public bool Perform( DrawingContext context,GraphControl self, Point mousePos)
        {
            //DrawBG;
            Brush brush = ColorIdentifier.Background.get().toBrush();
            context.DrawRoundedRectangle(brush, 
                new Pen(ColorIdentifier.InserterOutline.get().toBrush(), 
                    DoubleIdentifier.InserterOutlineThickness.get()), 
                new Rect(_pos, _size), DoubleIdentifier.InserterCornerRadius.get(), DoubleIdentifier.InserterCornerRadius.get());
            
            //Draw heading
            var f = new FormattedText(
                "Insert Node",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"), 12, ColorIdentifier.TilteText.get().toBrush(), VisualTreeHelper.GetDpi(self).PixelsPerDip);
            context.DrawText(
                f, 
                new Point(_pos.X + (_size.Width - f.Width) / 2 , _pos.Y + 2) 
                );
            context.DrawLine(
                new Pen(
                    ColorIdentifier.InserterOutline.get().toBrush(), 1),
                new Point(_pos.X, _pos.Y + f.Height + 4),
                new Point(_pos.X + _size.Width, _pos.Y + f.Height + 4));
            
            //Draw SearchBox
            context.DrawImage(_searchIcon, new Rect(_pos.X + 4, _pos.Y + f.Height + 8, f.Height, f.Height ));
            
            //TODO: DrawSearch String
            
            context.DrawLine(
                    new Pen( 
                        ColorIdentifier.InserterOutline.get().toBrush(), 
                        DoubleIdentifier.InserterSeperatorThickness.get()), 
                    new Point( _pos.X,_pos.Y + f.Height + 12 + f .Height),
                    new Point(_pos.X + _size.Width,_pos.Y + f.Height + 12 + f .Height )
                );
            //Draw Path
            double curY = _pos.Y + f.Height + 14 + f.Height;
            _rects = new List<Tuple<Rect, bool, string, Type>>();
            if (_path.Length > 0)
            {
                var sFormattedText = new FormattedText(
                    _path,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 12, Brushes.White, VisualTreeHelper.GetDpi(self).PixelsPerDip) {MaxTextWidth = _size.Width - 5};

                if (new Rect(new Point(_pos.X,curY ), new Size(_size.Width, sFormattedText.Height)).Contains(mousePos) )
                {
                    context.DrawRectangle(ColorIdentifier.Line2.get().toBrush(),null,new Rect(new Point(_pos.X,curY ), new Size(_size.Width, sFormattedText.Height)));
                }
                
                context.DrawText(
                    sFormattedText, 
                    new Point(_pos.X + 3 , curY + 1) 
                );
                _rects.Add(new Tuple<Rect, bool, string, Type>(
                    new Rect(new Point(_pos.X,curY ), new Size(_size.Width, sFormattedText.Height)),true, "\nReverse", null 
                    ));
                curY += sFormattedText.Height + 4;
            }
            
            //Draw Items
            double toY = _pos.Y + _size.Height - 8;
            
            List<Tuple<string, Type>> items = new List<Tuple<string, Type>>(
                _nodeDict.Where(x => x.Item1.StartsWith(_path))
                );
            _drawnFolders = new List<string>();
            
            //apply scroll
            int tScroll = _scroll;
            tScroll  = tScroll.InRange(0, items.Count - 1);
            
            for (int i = tScroll; i < items.Count; i++)
            {
                if (curY < toY)
                {
                    DrawElement(context, self,items[i].Item1,mousePos, items[i].Item2,ref curY);
                    
                }
            }
            
            return false;
        }

        private void DrawElement(DrawingContext context,GraphControl self, string text, Point mouse, Type type, ref double y)
        {


            string nt = text.Remove(0, _path.Length);
                bool folder = false;
            if (nt.Contains("/"))
            {
                nt = nt.Split('/')[0];
                folder = true;
                if (_drawnFolders.Contains(nt))
                {
                    return;
                }
            }

            var t = new FormattedText(
                nt, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White,
                VisualTreeHelper.GetDpi(self).PixelsPerDip
            ) {MaxTextWidth = folder ? _size.Width - 20 - 2 : _size.Width - 2};


            if (new Rect(_pos.X, y , _size.Width, t.Height).Contains(mouse))
            {
                context.DrawRectangle(ColorIdentifier.Line1.get().toBrush(),null,new Rect(_pos.X, y , _size.Width, t.Height));
            }
            context.DrawText(t, new Point(_pos.X + 2, y ));
            
            //Draw Folder
            if (folder)
            {
                var l = new PathGeometry(new[]
                {
                    new PathFigure(new Point(_pos.X + _size.Width - 15, y + 2), new PathSegment[]
                    {
                        new LineSegment(new Point(_pos.X + _size.Width - 5, y + t.Height / 2 ),true ), 
                        new LineSegment(new Point(_pos.X + _size.Width - 15, y + t.Height - 1), true), 
                    }, false ), 
                });
               _drawnFolders.Add(nt);
                context.DrawGeometry(null, new Pen(ColorIdentifier.TilteText.get().toBrush(), 1), l  );
            }
            
            _rects.Add(new Tuple<Rect, bool, string, Type>(
                new Rect(_pos.X, y , _size.Width, t.Height), folder, nt, type));
            y += t.Height + 2;
            context.DrawLine(new Pen(ColorIdentifier.Line1.get().toBrush(),0.5 ),new Point(_pos.X + 3, y -1 ), new Point(_pos.X + _size.Width - 3, y) );
        }
    }
}