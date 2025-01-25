using UnityEngine.UIElements;

namespace Editor.View
{
    /// <summary>
    /// 分割视图，继承自分割视图类
    /// </summary>
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

        public SplitView()
        {
            
        }
    }
}
