using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.View
{
    public class BehaviorTreeWindow : EditorWindow
    {
        /// <summary>
        /// 将ShowExample方法放在菜单栏地址下,
        /// _%#&表示快捷键，%为ctrl，#为shift，&为alt
        /// </summary>
        [MenuItem("Tools/BehaviorTreeWindow _#&i")]
        public static void ShowExample()
        {
            BehaviorTreeWindow wnd = GetWindow<BehaviorTreeWindow>("BehaviorTreeWindow");   //自带的创建窗口的方法
        }

        /// <summary>
        /// 创建完窗口后会创建GUI
        /// </summary>
        public void CreateGUI()
        {
            var root = rootVisualElement;
        
            //使用组件添加树视图，并克隆到视窗中
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/View/BehaviorTreeWindow.uxml");
            visualTree.CloneTree(root);
            
            //新建一个Label组件并附加样式代码
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/View/BehaviorTreeWindow.uss");
        
        }

   
    }
}