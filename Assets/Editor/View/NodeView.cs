using BehaviorTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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

        #region 连线方法
        public void LinkLine()
        {
            var graphView = BehaviorTreeWindow.windowRoot.treeView;
            switch (NodeData)
            {
                case BtComposite composite: //复合节点将所有子节点都连线
                    composite.ChildNodes.ForEach(n =>
                    {
                        graphView.AddElement(PortLink(OutputPort, graphView.NodeViews[n.Guid].InputPort));
                    });
                    break;
                case BtPrecondition precondition://条件节点于自己唯一的子节点连线
                    graphView.AddElement(PortLink(OutputPort, graphView.NodeViews[precondition.ChildNode.Guid].InputPort));
                    break;
            }
        }

        public static Edge PortLink(Port p1, Port p2)
        {
            var tempEdge = new Edge()
            {
                output = p1,
                input = p2
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            return tempEdge;
        }
        #endregion

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

        /// <summary>
        /// 在右键菜单时会调用的
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Set Root", SetRoot);//添加设置某个节点为根节点的方法
        }

        /// <summary>
        /// 节点被选择时调用的接口
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            BehaviorTreeWindow.windowRoot.inspectorView.UpdateViewData();//更新Inspector视图的数据
        }

        private void SetRoot(DropdownMenuAction obj) => BTSetting.GetSetting().SetRoot(NodeData);

        /// <summary>
        /// 设置节点的位置
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            NodeData.Position = new Vector2(newPos.xMin, newPos.yMin);
        }

    }

    public enum PortType
    {
        无,Input,OutPut
    }
}
