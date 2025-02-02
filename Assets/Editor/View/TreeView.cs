using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorTree;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.View
{
    /// <summary>
    /// 树视图，继承自图形化视图
    /// </summary>
    public class TreeView : GraphView
    {
        public Dictionary<string, NodeView> NodeViews = new Dictionary<string, NodeView>();//以节点id为索引的节点存储字典，方便查找

        public List<BtNodeBase> copyNodes = new List<BtNodeBase>();
        public new class UxmlFactory : UxmlFactory<TreeView,UxmlTraits> { }
        public TreeView()
        {
            Insert(0,new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            //应用统一样式
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/View/BehaviorTreeWindow.uss"));
            GraphViewMenu();//将菜单放入

            graphViewChanged += OnGraphViewChanged;//添加连线委托
            RegisterCallback<MouseEnterEvent>(MouseEnterControl);//为鼠标点击注册器添加订阅事件
            RegisterCallback<KeyDownEvent>(KeyDownEventCallback);//键盘事件回调订阅事件
        }

        private void KeyDownEventCallback(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Tab)
            {
                evt.StopPropagation();
            }

            if (!evt.ctrlKey) return;
            switch (evt.keyCode)
            {
                case KeyCode.S:
                    BehaviorTreeWindow.windowRoot.Save();
                    evt.StopPropagation();//按键信息停止向下传送，避免多次调用
                    break;
                case KeyCode.E:
                    //OnStartMove();
                    evt.StopPropagation();
                    break;
                case KeyCode.X:
                    //Cut(null);
                    evt.StopPropagation();
                    break;
                case KeyCode.C:
                    Copy();
                    evt.StopPropagation();
                    break;
                case KeyCode.V:
                    Paste();
                    evt.StopPropagation();
                    break;
            }
        }


        /// <summary>
        /// 复制方法，使用序列化实现
        /// </summary>
        private void Copy() => copyNodes = selection.OfType<NodeView>()
            .Select(n => n.NodeData)
            .ToList()
            .CloneData();

        /// <summary>
        /// 黏贴方法，使用反序列化实现
        /// </summary>
        private void Paste()
        {
            var nodePaste = new List<NodeView>();
            for (int i = 0; i < copyNodes.Count; i++) //先生成
            {
                var nodeView = new NodeView(copyNodes[i]);
                nodeView.SetPosition(new Rect(copyNodes[i].Position, Vector2.one));
                this.AddElement(nodeView);
                nodePaste.Add(nodeView);
                NodeViews.Add(copyNodes[i].Guid, nodeView); //添加进字典
            }
            nodePaste.ForEach(n => n.LinkLine());//生成完毕后对所有节点重新连线
            copyNodes = copyNodes.CloneData();//重新克隆待复制的节点，避免数据重复
        }

        /// <summary>
        /// 每次鼠标点击就会触发这个方法
        /// </summary>
        /// <param name="evt"></param>
        private void MouseEnterControl(MouseEnterEvent evt)
        {
            BehaviorTreeWindow.windowRoot.inspectorView.UpdateViewData();
        }

        /// <summary>
        /// 视图控制回调函数，当视图改变时
        /// </summary>
        /// <param name="gvc"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange gvc)
        {
            if (gvc.edgesToCreate != null)//如果要创建线条的话
            {
                gvc.edgesToCreate.ForEach(edge =>
                {
                    edge.LinkLineAddData();
                });
            }

            if (gvc.elementsToRemove != null)//如果要断开线的话
            {
                gvc.elementsToRemove.ForEach(ele =>
                {
                    if (ele is Edge edge)
                    {
                        edge.UnLinkLineDeleteData();
                    }
                });
            }

            return gvc;
        }

        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            //添加创建菜单
            //evt.menu.AppendAction("Create Node",CreateNode);
        }

        /// <summary>
        /// 创建节点方法
        /// </summary>
        /// <param name="obj"></param>
        private void CreateNode(Type type, Vector2 position)
        {
            BtNodeBase nodeData = Activator.CreateInstance(type) as BtNodeBase;//使用反射创建对应节点单例
            nodeData.Guid = System.Guid.NewGuid().ToString();//生成一个ID随机值
            nodeData.NodeName = type.Name;
            var node = new NodeView(nodeData);
            node.SetPosition(new Rect(position, Vector2.one));
            NodeViews.Add(nodeData.Guid, node);//添加进字典
            this.AddElement(node);
        }
        
        /// <summary>
        /// 显示菜单图形界面
        /// </summary>
        public void GraphViewMenu()
        {
            var menuWindowProvider = ScriptableObject.CreateInstance<RightClickMenu>();//将BehaviorTreeWindow中的菜单实例化
            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;//传递对应事件的委托
        
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            };
        }
        
        /// <summary>
        /// 覆写GetCompatiblePorts 定义链接规则
        /// 筛选条件是两个端口类型不同，且不是自身
        /// </summary>
        /// <param name="startAnchor"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns>返回所有能连接的端口</returns>
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPorts => 
                    endPorts.direction != startAnchor.direction && endPorts.node != startAnchor.node)
                .ToList();
        }
        
        /// <summary>
        /// 点击菜单时菜单创建对应类型的Node
        /// </summary>
        /// <param name="searchTreeEntry"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = BehaviorTreeWindow.windowRoot;//获取窗口位置
            var rootVisualElement = windowRoot.rootVisualElement;
            var windowMousePosition = rootVisualElement.ChangeCoordinatesTo(rootVisualElement.parent, 
                context.screenMousePosition - windowRoot.position.position);//获取鼠标点击的坐标
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);//通过窗口转换鼠标的位置

            CreateNode((Type)searchTreeEntry.userData, graphMousePosition);//创建节点
            return true;
        }
    }
}