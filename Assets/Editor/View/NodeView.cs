using BehaviorTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.View
{
    public class NodeView : Node
    {
        public BtNodeBase NodeData;
        
        public Port InputPort;
        public Port OutputPort;
        
        public NodeView(BtNodeBase nodeData)
        {
            this.NodeData = nodeData;
            InitNodeView(nodeData);//初始化创建的节点
        }

        public void InitNodeView(BtNodeBase nodeData)
        { 
            title = nodeData.NodeName;//赋值标题
            //对应三种节点创建节点端口
            InputPort = +this;
            inputContainer.Add(InputPort);

            if (nodeData is not BtActionNode)
            {
                OutputPort = this - (nodeData is BtPrecondition);
                outputContainer.Add(OutputPort);
            }
        }

        /// <summary>
        /// 设置节点的位置
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.Position = new Vector2(newPos.xMin, newPos.yMin);
        }

        #region 创建连线端口
        public static Port operator +(NodeView view)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input,
                Port.Capacity.Single, typeof(NodeView));
            return port;
        }

        public static Port operator -(NodeView view, bool isSingle)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output,
                isSingle ? Port.Capacity.Single : Port.Capacity.Multi, typeof(NodeView));
            return port;
        }
        #endregion
    }

    public enum PortType
    {
        无,Input,OutPut
    }
}
