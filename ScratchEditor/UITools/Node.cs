using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ScratchEditor.ThemeHandling;

namespace ScratchEditor.UI
{
    public class Node
    {
        private double _posX, _posY, _sizeW, _sizeH;
        private string _title;
        private List<Handle> handles;
        private int hoveredHandle;
        private bool dragging = false;
        public Node(double x, double y, double width, double height, string title, params Handle[] handles)
        {
            _posX = x;
            _posY = y;
            _sizeW = width;
            _sizeH = height;
            _title = title;
            this.handles = handles.ToList();
        }
        public Node(double x, double y, double width, double height, string title, params HandleConstruct[] handles)
        {
            _posX = x;
            _posY = y;
            _sizeW = width;
            _sizeH = height;
            _title = title;
            
            List<Handle> hs = new List<Handle>();
            for (int i = 0; i < handles.Length; i++)
            {
                hs.Add(new Handle(handles[i].HandleType, handles[i].HandleDataType, this));
            }

            this.handles = hs.ToList();
        }

        public Node()
        {
            _posX = 10;
            _posY = 10;
            _sizeW = 200;
            _sizeH = 250;
            _title = "Node";
        }
        public Node(double x, double y)
        {
            _posX = x;
            _posY = y;
            _sizeW = 200;
            _sizeH = 250;
            _title = "Node";
        }

        public Handle applyConnection(Handle conn)
        {
            handles[hoveredHandle].ConnectTo(conn);
            return handles[hoveredHandle];
        }
        
        public void EndDrag(Node draggedTo)
        {
            dragging = false;
            
            handles[hoveredHandle].EndDrag(draggedTo.applyConnection(handles[hoveredHandle])); //TODO: Construct to when empty;
        }
        
        public void BeginDrag()
        {
            dragging = true;
            handles[hoveredHandle].BeginDrag();
        }

        public void DrawConnections(DrawingContext context)
        {
            if (handles == null)
                return;
            
            foreach (var h in handles.Where(x => x.HandleType == HandleType.Output))
            {
                h.DrawConnection(context);
            }
        }

        public bool Draw(DrawingContext context,GraphControl self , Point posM, Point offset, bool selected)
        {
            Rect nodeRect = new Rect(_posX + offset.X, _posY + offset.Y, _sizeW, _sizeH);
            
            
            
            var brush = new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.Node_BG));
            var pBrush = new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.Bounding));
            context.DrawRoundedRectangle(brush, new Pen(pBrush, nodeRect.Contains(posM) || selected ? 3 : 1),nodeRect , 10,10  );

            var f = new FormattedText(
                _title,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"), 8, Brushes.White, VisualTreeHelper.GetDpi(self).PixelsPerDip);
            context.DrawText(
                f, 
                new Point(_posX + offset.X + (_sizeW - f.Width) / 2 , _posY + offset.Y + 2) );
            
            context.DrawLine(new Pen(pBrush, 1),new Point(_posX + offset.X, _posY + offset.Y + f.Height + 4), new Point(_posX + _sizeW + offset.X, _posY + offset.Y + f.Height + 4));
            
            
            //Draw Handles
            if (handles == null)
                return false;

            double inY = _posY + offset.Y + f.Height + 14;
            double outY = inY;

            if (!dragging)
                hoveredHandle = -1;
            for (int i = 0; i < handles.Count; i++)//TODO: Implement draw order;
            {
                var handle = handles[i];
                if (handle.HandleType == HandleType.Input)
                {
                    DrawHandleIn(context, ref inY, handle, posM, offset, out bool hovered);
                    if (hovered && !dragging)
                        hoveredHandle = i;
                }
                else
                {
                    DrawHandleOut(context, ref outY, handle, posM, offset, out bool hovered);
                    if (hovered && !dragging)
                        hoveredHandle = i;
                }
            }

            if (dragging)
                return false;
            return hoveredHandle != -1;
        }

        public void DrawHandleIn(DrawingContext context, ref double y, Handle handle, Point posM, Point offset, out bool hover)
        {
           y = handle.Draw(context, posM, new Point(_posX + offset.X + 10, y), out hover );
        }
        public void DrawHandleOut(DrawingContext context, ref double y, Handle handle, Point posM, Point offset, out bool hover)
        {
            y = handle.Draw(context, posM, new Point(_posX + offset.X +  _sizeW - 10, y), out hover );
        }
        
        public Rect getRect(Point offset)
        {
            return new Rect(_posX + offset.X, _posY + offset.Y, _sizeW, _sizeH);
        }

        public void Move(Vector delta)
        {
            _posX -= delta.X;
            _posY -= delta.Y;
        }
    }
}