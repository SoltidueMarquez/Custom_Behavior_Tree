using UnityEngine.UIElements;

namespace Editor.View
{
    /// <summary>
    /// 检查器视图，继承自VisualElement
    /// </summary>
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView,UxmlTraits> { }

        public InspectorView()
        {
            
        }
    }
}