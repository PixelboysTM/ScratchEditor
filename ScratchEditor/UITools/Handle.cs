using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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
        
        public Handle(HandleType handleType, HandleDataType handleDataType, Node node)
        {
            _handleType = handleType;
            _handleDataType = handleDataType;
            parentNode = node;
        }

        public void BeginDrag()
        {
            _dragging = true;
        }

        public void EndDrag(Handle connTo)
        {
            _dragging = false;
            connectTo = connTo;
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
                    return center.Y + 10;
            }

            hover = false;
            return 0;

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
            SolidColorBrush brush = new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.TilteText));

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

            switch (PropertyManager.LineType)
            {
                case LineType.Line:
                    var p = new Pen(new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.TilteText)), 2);
                    context.DrawLine(p, lastCenter, lastCenter + new Vector(20,0));
                    context.DrawLine(p, lastCenter + new Vector(20,0), connectTo.lastCenter - new Vector(20,0));
                    context.DrawLine(p, connectTo.lastCenter - new Vector(20,0), connectTo.lastCenter);
                    break;
                case LineType.Bezier:
                    double abs = Math.Abs(lastCenter.X - connectTo.lastCenter.X);
                    double standartLength = 200.0; /*TODO:EXPOSE TO PROPERTY*/
                    context.DrawGeometry(null, new Pen(new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.TilteText)), 1), 
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
            

        public void ConnectTo(Handle conn)
        {
            connectTo = conn;
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
        
        public HandleType HandleType;
        public HandleDataType HandleDataType;
    }
}