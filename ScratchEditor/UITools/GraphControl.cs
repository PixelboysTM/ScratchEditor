using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScratchEditor.misc;
using ScratchEditor.ThemeHandling;
using ScratchEditor.UITools;

namespace ScratchEditor.UI
{
    public class GraphControl : Control
    {
        private SolidColorBrush brush;
        private List<Node> nodes;
        private List<Node> selctedNodes;
        private Point mousePos;
        private bool leftMouseDown;
        private bool controlDown;
        private Point offset;
        private Point dragFrom;
        private Inserter _inserter;
        private bool InserterOpen = false;
        private Node nodeForAction = null;
        private Node draggingTo = null;
        private bool _draggingConnection = false;
        
        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl), new FrameworkPropertyMetadata(typeof(GraphControl)));
            
        }

        public GraphControl()
        {
         brush = new SolidColorBrush();   
         nodes = new List<Node>()
         {
             new Node(50,50,150,100, "SampleNode", new HandleConstruct(HandleType.Input, HandleDataType.Path), new HandleConstruct(HandleType.Output, HandleDataType.Path)),
             new Node(500,50,150,100, "SampleNode", new HandleConstruct(HandleType.Input, HandleDataType.Path), new HandleConstruct(HandleType.Output, HandleDataType.Path)),
         };
         
         mousePos = new Point(0,0);
         
         _inserter = new Inserter(new Size(150,250));
         _inserter.AddNodeElement<Node>("Base Node");
         _inserter.AddNodeElement<Node>("Folder/Node");
         _inserter.AddNodeElement<Node>("Folder/Folder2/Sample Node");
         _inserter.AddNodeElement<Node>("1");
         _inserter.AddNodeElement<Node>("2");
         _inserter.AddNodeElement<Node>("3");
         _inserter.AddNodeElement<Node>("4");
         _inserter.AddNodeElement<Node>("5");
         _inserter.AddNodeElement<Node>("6");
         _inserter.AddNodeElement<Node>("7");
         _inserter.AddNodeElement<Node>("8");
         _inserter.AddNodeElement<Node>("9");
         _inserter.AddNodeElement<Node>("10");
         _inserter.AddNodeElement<Node>("11");
         _inserter.AddNodeElement<Node>("12");
         _inserter.AddNodeElement<Node>("13");
         _inserter.AddNodeElement<Node>("14");
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            //Drawing BG;
            brush.Color = PropertyManager.getColor(ColorIdentifier.Background);
            drawingContext.DrawRectangle(brush, null, new Rect(0,0,ActualWidth, ActualHeight));
            
            //Draw Grid;
            SolidColorBrush b = new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.Line1));
            Pen pen = new Pen(b, /*TODO:Expose To Property*/0.2);
            int gridSize = 20;
            DrawGrid(drawingContext, pen, gridSize, ActualWidth, ActualHeight);
            
            gridSize *= 2;
            pen.Thickness = 0.8;
            b.Color = PropertyManager.getColor(ColorIdentifier.Line2);
            DrawGrid(drawingContext, pen, gridSize, ActualWidth, ActualHeight);
            
            
            //DrawContent
                //Draw Connections
            foreach (var node in nodes)
            {
                node.DrawConnections(drawingContext);
            }
                //Draw Nodes
            if (!_draggingConnection)
                nodeForAction = null;
            else
                draggingTo = null;
            foreach (var node in nodes)
            {
                if (node.Draw(drawingContext, this, mousePos, offset,
                    selctedNodes?.Contains(node) ?? false))
                {
                    if (!_draggingConnection)
                        nodeForAction = node;
                    else
                        draggingTo = node;
                }
            }

            
            //Draw Inserter
            if (InserterOpen)
            {
                if (_inserter.Perform(drawingContext, this, mousePos))
                    InserterOpen = false;
            }
            
            //Draw Selection
            if (controlDown && leftMouseDown && dragFrom != null)
            {
                SolidColorBrush s = new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.Bounding));
                var p = new Pen(s, 3);
                p.DashStyle = DashStyles.Dash;
                p.DashCap = PenLineCap.Square;

                drawingContext.DrawRectangle(null, p, new Rect(dragFrom, mousePos ));
            }
            
            //Drawing Bounding;
            drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(PropertyManager.getColor(ColorIdentifier.Bounding)), /* Expose to Property*/ 5), new Rect(0,0, ActualWidth, ActualHeight) );
        }
        
        
        private void DrawGrid(DrawingContext drawingContext, Pen pen, int gridSize, double width, double height )
        {
            for (int x = 0; x <= width / gridSize; x++)
            {
                drawingContext.DrawLine(pen, new Point(x * gridSize + getRawOffset(gridSize, offset.X), 0), new Point(x * gridSize + getRawOffset(gridSize, offset.X), height));
            }
            for (int y = 0; y <= height / gridSize; y++)
            {
                drawingContext.DrawLine( pen, new Point(0, y * gridSize + getRawOffset(gridSize, offset.Y)), new Point(width, y * gridSize + getRawOffset(gridSize, offset.Y)));
            }
        }

        private double getRawOffset(double gridSize, double set)
        {
            double raw = set;
            if (set < 0 )
            {
                while (raw < -gridSize)
                {
                    raw += gridSize;
                }

                return raw;
            }
            
            while (raw > gridSize)
            {
                raw -= gridSize;
            }

            return raw;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point newMouse = e.GetPosition(this);

            if (InserterOpen)
            {
                mousePos = newMouse;
                InvalidateVisual();
                return;
            }
            
            if (leftMouseDown)
            {
                if (controlDown)
                {
                    selctedNodes = new List<Node>();
                    foreach (var node in nodes)
                    {
                        if (new Rect(dragFrom, mousePos ).OverlapRect(node.getRect(offset)))
                        {
                            selctedNodes.Add(node);
                        }
                    }

                    mousePos = newMouse;
                    InvalidateVisual();
                    return;
                }
                
                if (selctedNodes.Count > 0)
                {
                    foreach (var node in selctedNodes)
                    {
                        node.Move(mousePos - newMouse);
                    }
                }
                else
                {
                    offset -= mousePos - newMouse;
                }
            }
            mousePos = newMouse;
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {//TODO: Only reset selected if Not over node.
            base.OnMouseLeftButtonDown(e);
            if (InserterOpen)
            {
                var r = _inserter.PerformClick(e.GetPosition(this));
                if (r.Item1)
                {
                    if (r.Item2 != null)
                    {
                        nodes.Add(r.Item2);
                    }
                    InserterOpen = false;
                }

                
                InvalidateVisual();
                return;
            } //Inserter Handling

            if (nodeForAction != null)
            {
                _draggingConnection = true;
                nodeForAction.BeginDrag();
                InvalidateVisual();
                return;
            }
            
            leftMouseDown = true;
            selctedNodes = new List<Node>();
            if (controlDown)
            {
                dragFrom = e.GetPosition(this);
                return;
            }
            foreach (var node in nodes)
            {
                if (node.getRect(offset).Contains(e.MouseDevice.GetPosition(this)))
                {
                    selctedNodes.Add(node);
                    break;
                }
            }
            
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);


            if (_draggingConnection)
            {
                _draggingConnection = false;
                nodeForAction.EndDrag(draggingTo);
                InvalidateVisual();
                return;
            }
            
            
            leftMouseDown = false;
            selctedNodes = new List<Node>();
            if (controlDown)
            {
                foreach (var node in nodes)
                {
                    if (new Rect(dragFrom, mousePos ).Contains(e.MouseDevice.GetPosition(this)))
                    {
                        selctedNodes.Add(node);
                    }
                }
            }
        }
        private void OpenInserter(Point pos)
        {
            _inserter.SetNextSpot(pos, null);
            InserterOpen = true;
            InvalidateVisual();
            
        }
        
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            OpenInserter(e.GetPosition(this));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (InserterOpen)
            {
                _inserter.PerformKeyPress(e.Key);
                InvalidateVisual();
                return;
            }
            
            base.OnKeyDown(e);
            if (e.Key == Key.LeftCtrl)
            {
                controlDown = true;
            }else if (e.Key == Key.Space)
            {
                OpenInserter(mousePos);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.LeftCtrl)
            {
                controlDown = false;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (InserterOpen)
            {
                _inserter.PerformScroll(e.Delta);
                InvalidateVisual();
            }
        }
    }
}