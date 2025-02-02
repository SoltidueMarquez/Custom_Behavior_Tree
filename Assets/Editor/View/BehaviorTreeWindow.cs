using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorTree;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Editor.View
{
    public class BehaviorTreeWindow : EditorWindow
    {
        public static BehaviorTreeWindow windowRoot;//创建单例用于获取窗口位置信息

        public TreeView treeView;
        public InspectorView inspectorView;

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
        /// 窗口关闭时自动保存
        /// </summary>
        private void OnDestroy()
        {
            Save();
        }

        /// <summary>
        /// 创建完窗口后会创建GUI
        /// </summary>
        public void CreateGUI()
        {
            var id = BTSetting.GetSetting().TreeID;//获取树的ID用于加载树
            var iGetBt = EditorUtility.InstanceIDToObject(id) as IGetBt;
            
            windowRoot = this;
            var root = rootVisualElement;
            //使用组件添加树视图，并克隆到视窗中
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/View/BehaviorTreeWindow.uxml");
            //新建一个Label组件并附加样式代码
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/View/BehaviorTreeWindow.uss");
            visualTree.CloneTree(root);

            inspectorView = root.Q<InspectorView>();//获取Inspector的视图
            treeView = root.Q<TreeView>();//获取树的视图
            if (iGetBt == null) return;//非空判定
            if (iGetBt.GetRoot() == null) return;
            CreateRoot(iGetBt.GetRoot());//创建树的节点
            //在所有节点创建完毕后开始连线，调用所有的节点连接自己的子集
            treeView.nodes.OfType<NodeView>().ForEach(n => n.LinkLine());
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());//直接保存当前活动的场景
        }
        
        /// <summary>
        /// 通过创建根节点创建树
        /// </summary>
        /// <param name="rootNode"></param>
        public void CreateRoot(BtNodeBase rootNode)
        {
            if (rootNode == null) return;
            NodeView nodeView = new NodeView(rootNode);//创建一个节点
            nodeView.SetPosition(new Rect(rootNode.Position, Vector2.one));//设置位置
            treeView.AddElement(nodeView);
            treeView.NodeViews.Add(rootNode.Guid, nodeView);//添加进字典
            switch (rootNode)//向下遍历所有的子节点生成出来
            {
                case BtComposite composite:
                    composite.ChildNodes.ForEach(CreateChild);
                    break;
                case BtPrecondition precondition:
                    CreateChild(precondition.ChildNode);
                    break;
            }
        }

        /// <summary>
        /// 创建子节点视图方法
        /// </summary>
        /// <param name="nodeData"></param>
        public void CreateChild(BtNodeBase nodeData)
        {
            if (nodeData == null) return;
            NodeView nodeView = new NodeView(nodeData);//创建一个节点
            nodeView.SetPosition(new Rect(nodeData.Position, Vector2.one));//设置位置
            treeView.AddElement(nodeView);
            treeView.NodeViews.Add(nodeData.Guid, nodeView);//添加进字典
            switch (nodeData)//向下遍历所有的子节点生成出来
            {
                case BtComposite composite:
                    composite.ChildNodes.ForEach(CreateChild);
                    break;
                case BtPrecondition precondition:
                    CreateChild(precondition.ChildNode);
                    break;
            }
        }
    }

    /// <summary>
    /// 菜单树
    /// </summary>
    public class RightClickMenu : ScriptableObject, ISearchWindowProvider
    {
        //委托，用于处理菜单对应方法的数据发送
        public delegate bool SelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SelectEntryDelegate OnSelectEntryHandler;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));//添加菜单组
            entries = AddNodeType<BtComposite>(entries, "组合节点");
            entries = AddNodeType<BtPrecondition>(entries, "条件节点");
            entries = AddNodeType<BtActionNode>(entries, "行为节点");
            return entries;
        }

        /// <summary>
        /// 通过反射获取对应的菜单数据
        /// </summary>
        public List<SearchTreeEntry> AddNodeType<T>(List<SearchTreeEntry> entries, string pathName)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(pathName)) { level = 1 });
            List<Type> rootNodeTypes = GetDerivedClasses(typeof(T));//寻找继承了该节点类的所有类，也就是之后所有的节点类型
            foreach (var rootType in rootNodeTypes)//遍历并为该节点类型创建二级菜单
            {
                string menuName = rootType.Name;//名称就是对应的Name字段
                entries.Add(new SearchTreeEntry(new GUIContent(menuName)) { level = 2,userData = rootType});//将菜单的数据通过userData传输到对应的点击位置
            }
            return entries;
        }
        
        /// <summary>
        /// 开始选择时
        /// </summary>
        /// <param name="SearchTreeEntry"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null)
            {
                return false;
            }
            return OnSelectEntryHandler(SearchTreeEntry, context);
        }
        
        /// <summary>
        /// 获取所有继承了该类型的类型，包含子类的子类
        /// </summary>
        public static List<Type> GetDerivedClasses(Type type)
        {
            List<Type> derivedClasses = new List<Type>();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                    {
                        derivedClasses.Add(t);
                    }
                }
            }
            return derivedClasses;
        }
    }
}