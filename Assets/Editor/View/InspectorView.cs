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
    }
}