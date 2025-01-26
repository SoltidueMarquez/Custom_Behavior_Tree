using System;
using BehaviorTree;
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
            nodeData.NodeName = type.Name;
            
            var node = new NodeView(nodeData);
            node.SetPosition(new Rect(position, Vector2.one));
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