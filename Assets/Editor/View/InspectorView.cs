using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BehaviorTree;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.View
{
    /// <summary>
    /// 检查器视图，继承自VisualElement
    /// </summary>
    public class InspectorView : VisualElement
    {
        public IMGUIContainer inspectorBar;
        public InspectorViewData ViewData;
        
        public new class UxmlFactory : UxmlFactory<InspectorView,UxmlTraits> { }

        public InspectorView()
        {
            inspectorBar = new IMGUIContainer() { name = "inspectorBar" };//创建一个GUI框
            inspectorBar.style.flexGrow = 1;//拉伸占满
            CreateInspectorView();
            Add(inspectorBar);
        }
        
        private async void CreateInspectorView()
        {
            ViewData = Resources.Load<InspectorViewData>("InspectorViewData");
            await Task.Delay(100);//等待资源加载
            var odinEditor = UnityEditor.Editor.CreateEditor(ViewData);
            //将SO的Inspector信息搬到视图上
            inspectorBar.onGUIHandler += () =>
            {
                odinEditor.OnInspectorGUI();
            };
        }

        /// <summary>
        /// 更新视图方法
        /// </summary>
        public void UpdateViewData()
        {
            var nodes = BehaviorTreeWindow.windowRoot.treeView.selection
                .OfType<NodeView>()
                .Select(nodeView => nodeView.NodeData)
                .ToHashSet();//从树的视图获取当前选择的节点的信息，从而更新Inspector视图
            ViewData.DataView = nodes;
            /*ViewData.DataView.Clear();//直接赋值的话会导致对象在销毁后不更新，那就不对了
            foreach (var node in nodes)
            {
                ViewData.DataView.Add(node);
            }*/
        }
    }
}