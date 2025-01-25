using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.View
{
    public class BehaviorTreeWindow : EditorWindow
    {
        public static BehaviorTreeWindow windowRoot;//创建单例用于获取窗口位置信息

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
            windowRoot = this;
            var root = rootVisualElement;
            //使用组件添加树视图，并克隆到视窗中
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/View/BehaviorTreeWindow.uxml");
            //新建一个Label组件并附加样式代码
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/View/BehaviorTreeWindow.uss");
            visualTree.CloneTree(root);
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