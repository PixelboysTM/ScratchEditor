using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScratchEditor.misc;
using ScratchEditor.ThemeHandling;
using ScratchEditor.UI;
using Node = ScratchEditor.UI.Node;

namespace ScratchEditor.UITools
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
        public ID_Data guid;

        public void setId(ID_Data id)
        {
            guid = IdManager.setId(guid, id);
        }
        
        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl), new FrameworkPropertyMetadata(typeof(GraphControl)));
            
        }

        public GraphControl()
        {
            guid = IdManager.getGuid();

            this.Focusable = true;
            Keyboard.Focus(this);
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Keyboard.AddKeyUpHandler(this, OnKeyUp);
            
         brush = new SolidColorBrush();   
         nodes = new List<Node>()
         {
             new Node(50,50,150,100, "SampleNode", new HandleConstruct(HandleType.Input, HandleDataType.Path), new HandleConstruct(HandleType.Input, HandleDataType.Bool), new HandleConstruct(HandleType.Output, HandleDataType.Path), new HandleConstruct(HandleType.Output, HandleDataType.Bool)),
             new Node(100,50,150,100, "SampleNode", new HandleConstruct(HandleType.Input, HandleDataType.Path), new HandleConstruct(HandleType.Input, HandleDataType.Bool), new HandleConstruct(HandleType.Output, HandleDataType.Path), new HandleConstruct(HandleType.Output, HandleDataType.Bool)),
             new Node(150,50,150,100, "SampleNode", new HandleConstruct(HandleType.Input, HandleDataType.Path), new HandleConstruct(HandleType.Input, HandleDataType.Bool), new HandleConstruct(HandleType.Output, HandleDataType.Path), new HandleConstruct(HandleType.Output, HandleDataType.Bool)),
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

         // var y = Serialisation.SerialzeGraph(this);
         // Console.WriteLine(y);
         
          Serialisation.DeserializeGraph(
              "{\"id\":15658816,\"offset\":\"0,0\",\"nodes\":[{\"id\":1484882,\"pos\":\"50,50\",\"size\":\"150,100\",\"handles\":[{\"id\":51717130,\"HandleType\":0,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":76566748,\"HandleType\":0,\"HandleDataType\":3,\"connectedTo\":-1},{\"id\":62435764,\"HandleType\":1,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":50246420,\"HandleType\":1,\"HandleDataType\":3,\"connectedTo\":-1}]},{\"id\":46016738,\"pos\":\"100,50\",\"size\":\"150,100\",\"handles\":[{\"id\":26722482,\"HandleType\":0,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":55123457,\"HandleType\":0,\"HandleDataType\":3,\"connectedTo\":-1},{\"id\":43477822,\"HandleType\":1,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":45107328,\"HandleType\":1,\"HandleDataType\":3,\"connectedTo\":-1}]},{\"id\":84178410,\"pos\":\"150,50\",\"size\":\"150,100\",\"handles\":[{\"id\":10713050,\"HandleType\":0,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":1532512,\"HandleType\":0,\"HandleDataType\":3,\"connectedTo\":-1},{\"id\":34662742,\"HandleType\":1,\"HandleDataType\":0,\"connectedTo\":-1},{\"id\":22373670,\"HandleType\":1,\"HandleDataType\":3,\"connectedTo\":-1}]}]}"
              , this);
        }


        public Point getOffset() => offset;
        public Node[] getNodes() => nodes.ToArray();
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            //SortingNodes
            if (selctedNodes != null && selctedNodes.Count > 0)
            {
                foreach (var node in selctedNodes)
                {
                    nodes = nodes.MoveToFront(node);
                }
            }
            
            //Drawing BG;
            brush.Color = ColorIdentifier.Background.get();
            drawingContext.DrawRectangle(brush, null, new Rect(0,0,ActualWidth, ActualHeight));
            
            //Draw Grid;
            SolidColorBrush b = (SolidColorBrush) ColorIdentifier.Line1.get().toBrush();
            Pen pen = new Pen(b, DoubleIdentifier.GridThickness.get());
            int gridSize = 20;
            DrawGrid(drawingContext, pen, gridSize, ActualWidth, ActualHeight);
            
            gridSize *= 2;
            pen.Thickness = 0.8;
            b.Color = ColorIdentifier.Line2.get();
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
                Brush s = ColorIdentifier.Bounding.get().toBrush();
                var p = new Pen(s, 3);
                p.DashStyle = DashStyles.Dash;
                p.DashCap = PenLineCap.Square;

                drawingContext.DrawRectangle(null, p, new Rect(dragFrom, mousePos ));
            }
            //Drawing Bounding;
            drawingContext.DrawRectangle(null, new Pen(ColorIdentifier.Bounding.get().toBrush(), DoubleIdentifier.WindowBoundingThickness.get()), new Rect(0,0, ActualWidth, ActualHeight) );
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

            if (nodeForAction != null && nodeForAction.HoveredHandle.HandleType == HandleType.Output)
            {
                _draggingConnection = true;
                nodeForAction.BeginDrag();
                InvalidateVisual();
                return;
            }
            
            leftMouseDown = true;
            selctedNodes = new List<Node>();
            Node tempNode = null;
            if (controlDown)
            {
                dragFrom = e.GetPosition(this);
                
                return;
            }
            foreach (var node in nodes)
            {
                if (node.getRect(offset).Contains(e.MouseDevice.GetPosition(this)))
                {
                    tempNode = node;
                }
            }

            if (tempNode != null)
                selctedNodes.Add(tempNode);
            
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);


            if (_draggingConnection)
            {
                if (nodeForAction == draggingTo )
                {
                    nodeForAction.EndDrag(null);
                    InvalidateVisual();
                    return;
                }
                _draggingConnection = false;
                if (!nodeForAction.EndDrag(draggingTo))
                {
                    //TODO: OpenInserter with filter;
                }

                
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

        private void OnKeyDown(Object o,KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (InserterOpen)
            {
                _inserter.PerformKeyPress(e.Key);
                InvalidateVisual();
                return;
            }
            
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    controlDown = true;
                    break;
                case Key.Space:
                    OpenInserter(mousePos);
                    break;
            }
        }

        private void OnKeyUp( Object o, KeyEventArgs e)
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

        public void setOffset(Point jsonObjOffset)
        {
            offset = jsonObjOffset;
        }

        public void setNodes(List<Node> list)
        {
            nodes = list;
        }
    }
}