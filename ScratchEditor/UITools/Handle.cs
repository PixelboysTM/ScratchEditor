using System;
using System.Windows;
using System.Windows.Media;
using ScratchEditor.misc;
using ScratchEditor.misc.math;
using ScratchEditor.ThemeHandling;

namespace ScratchEditor.UI
{
    public class Handle
    {

        private HandleType _handleType;
        public HandleType HandleType
        {
            get => _handleType;
        }

        private HandleDataType _handleDataType;

        public HandleDataType HandleDataType
        {
            get => _handleDataType;
        }

        private Node parentNode;
        private Handle connectTo = null;
        private bool _dragging = false;
        private Point lastCenter;
        public readonly ID_Data guid;

        public Handle getConnected() => connectTo;
        
        public Handle(HandleType handleType, HandleDataType handleDataType, Node node)
        {
            guid = IdManager.getGuid();
            _handleType = handleType;
            _handleDataType = handleDataType;
            parentNode = node;
        }

        public void BeginDrag()
        {
            _dragging = true;
        }

        public bool EndDrag(Handle connTo)
        {
            _dragging = false;
            if (connTo != null)
            {
                connectTo = connTo;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draws this handle
        /// </summary>
        /// <param name="context">The canvas to draw on.</param>
        /// <param name="mousePos">The position of the mouse.</param>
        /// <param name="center">The handles center position</param>
        /// <param name="hover">Out: If this handle is hovered</param>
        /// <returns>The new y Position.</returns>
        public double Draw(DrawingContext context, Point mousePos, Point center, out bool hover)
        {
            lastCenter = center;
            
            switch (_handleDataType)
            {
                case HandleDataType.Path:
                    hover = PathDraw(context, mousePos, center);
                    return center.Y + 15;
                case HandleDataType.Bool:
                    hover = BoolDraw(context, mousePos, center);
                    return center.Y + 15;
            }

            hover = false;
            return 0;

        }

        private bool BoolDraw(DrawingContext context, Point mousePos, Point center)
        {
            center -= new Vector(1,0);
            Brush brush = ColorIdentifier.BoolType.get().toBrush();
            if (connectTo != null)
            {
                context.DrawEllipse(brush, null, center, 4,4 );
                return false;
            }
            else if (_dragging || Collisions.PointCircle(center, mousePos, 4) )
            {
                context.DrawEllipse(brush, null, center, 4,4 );
                if (_dragging)
                {
                    context.DrawLine(new Pen(brush, 1), center, mousePos );
                    return false;
                }
                return true;
            }
            else
            {
                context.DrawEllipse(null, new Pen(brush, 1), center, 4,4 );
                return false;
            }
        }

        /// <summary>
        /// Draws the Handle for the path Handle Type;
        /// </summary>
        /// <param name="context">The canvas to draw on.</param>
        /// <param name="mousePos">The mouse Position</param>
        /// <param name="center">The center point of the Handle</param>
        /// <returns>whether this handle is hovered if dragging from this handle always returns false;</returns>
        private bool PathDraw(DrawingContext context, Point mousePos, Point center)
        {
            Brush brush = ColorIdentifier.TilteText.get().toBrush();

            var points = new Point[]
            {
                new Point(center.X - 5, center.Y - 5),
                new Point(center.X, center.Y - 5),
                new Point(center.X + 5, center.Y), 
                new Point(center.X, center.Y + 5), 
                new Point(center.X - 5, center.Y + 5),
                new Point(center.X - 5, center.Y - 5)
            };
            
            var g = new PathGeometry(
                new[]
                {
                    new PathFigure(points[0],
                        new PathSegment[]
                        {
                            new LineSegment(points[1], true),
                            new LineSegment(points[2], true),
                            new LineSegment(points[3], true),
                            new LineSegment(points[4], true),
                            new LineSegment(points[5], true),
                        }, true
                    )
                }
            );
            if (connectTo != null)
            {
                context.DrawGeometry(brush, null, g);
                return false;
            }
            else if (_dragging || Collisions.GeometryPoint(points, mousePos.X, mousePos.Y)  )
            {
                context.DrawGeometry(brush, null, g);
                if (_dragging)
                {
                    context.DrawLine(new Pen(brush, 1), center, mousePos );
                    return false;
                }
                return true;
            }
            else
            {
                context.DrawGeometry(null, new Pen(brush, 1), g);
                return false;
            }
            
            
        }

        public void DrawConnection(DrawingContext context) 
        {
            if (connectTo == null)
                return;

            Color c = getColorFromType();
            
            switch (PropertyManager.LineType)
            {
                case LineType.Line:
                    var p = new Pen(new SolidColorBrush(c), DoubleIdentifier.ConnectionLineThickness.get());
                    var vector = new Vector(DoubleIdentifier.ConnectionLineStraight.get(), 0);
                    context.DrawLine(p, lastCenter, lastCenter + vector);
                    context.DrawLine(p, lastCenter + vector, connectTo.lastCenter - vector);
                    context.DrawLine(p, connectTo.lastCenter - vector, connectTo.lastCenter);
                    break;
                case LineType.Bezier:
                    double abs = Math.Abs(lastCenter.X - connectTo.lastCenter.X);
                    double standartLength = DoubleIdentifier.BezierStandartLenght.get();
                    context.DrawGeometry(null, new Pen(new SolidColorBrush(c), DoubleIdentifier.ConnectionBezierThickness.get()), 
                        new PathGeometry(new PathFigure[]
                        {
                            new PathFigure(
                                lastCenter, new PathSegment[]
                                {
                                    new PolyBezierSegment(new []{
                                        //lastCenter, 
                                        new Point(lastCenter.X +  standartLength.InRange(0, abs), lastCenter.Y),
                                        new Point(connectTo.lastCenter.X - standartLength.InRange(0, abs), connectTo.lastCenter.Y), 
                                        connectTo.lastCenter
                                        
                                    }, true), 
                                }, false
                                ), 
                        })
                        );
                    break;
            }
        }

        private Color getColorFromType()
        {
            switch (HandleDataType)
            {
                case HandleDataType.Path:
                    return ColorIdentifier.TilteText.get();
                case HandleDataType.Bool:
                    return ColorIdentifier.BoolType.get();
                default:
                    return Colors.Purple;
            }
        }
        
        public bool ConnectTo(Handle conn)
        {
            if (conn.HandleDataType == HandleDataType)
            {
                connectTo = conn;
                return true;
            }

            return false;
        }
    }

    public enum HandleType
    {
        Input, Output
    }

    public enum HandleDataType
    {
        Path, Text, Number, Bool
    }

    public struct HandleConstruct
    {
        public HandleConstruct(HandleType type, HandleDataType dataType)
        {
            HandleType = type;
            HandleDataType = dataType;
        }
        
        public readonly HandleType HandleType;
        public readonly HandleDataType HandleDataType;
    }
}