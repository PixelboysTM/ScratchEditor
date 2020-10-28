using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using ScratchEditor.UI;
using ScratchEditor.UITools;

namespace ScratchEditor.misc
{

    public class Serialisation
    {


        public static string SerialzeGraph(GraphControl control)
        {
            GraphData data = new GraphData()
            {
                id = control.guid.id,
                offset = control.getOffset(),
                nodes = compileNodes(control.getNodes())
            };
            return JsonConvert.SerializeObject(data, Formatting.None);
        }

        public static void DeserializeGraph(string json, GraphControl self)
        {
            var jsonObj = JsonConvert.DeserializeObject<GraphData>(json);
            self.setId(id: new ID_Data(jsonObj.id));
            self.setOffset(jsonObj.offset);
            List<Node> nodes = new List<Node>();
            foreach (NodeData nodeData in jsonObj.nodes)
            {
                nodes.Add(new Node(nodeData.pos, new ID_Data(nodeData.id))); //TODO: Restore Handles and Node Overload type
            }

            self.setNodes(nodes);
            self.InvalidateVisual();
        }

        private static NodeData[] compileNodes(Node[] nodes)
        {
            var n = new List<NodeData>();
            foreach (Node node in nodes)
            {
                n.Add(new NodeData()
                {
                    id = node.guid.id,
                    pos = node.getPos(),
                    size = node.getSize(),
                    handles = compileHandles(node.getHandles())
                });
            }

            return n.ToArray();
        }

        private static HandleData[] compileHandles(Handle[] handles)
        {
            var h = new List<HandleData>();
            foreach (Handle handle in handles)
            {
                var i = handle.getConnected();
                long id = -1;
                if (i != null)
                {
                    id = i.guid.id;
                }
                
                h.Add(new HandleData()
                {
                    id = handle.guid.id,
                    HandleType = handle.HandleType,
                    HandleDataType = handle.HandleDataType,
                    connectedTo = id
                    
                });
            }

            return h.ToArray();
        }
        
        private struct GraphData
        {
            public long id;
            public Point offset;
            public NodeData[] nodes;
        }
        private struct NodeData
        {
            public long id;
            public Point pos;
            public Point size;
            public HandleData[] handles;
        }
        
        private struct HandleData
        {
            public long id;
            public HandleType HandleType;
            public HandleDataType HandleDataType;
            public long connectedTo;
        }
    }

   
}