using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        }
    }
}