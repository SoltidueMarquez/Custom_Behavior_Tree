using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BehaviorTree
{
    public class TestBT : SerializedMonoBehaviour, IGetBt
    {
        [OdinSerialize] public BehaviorTreeData TreeData = new BehaviorTreeData();

        private void Update()
        {
            TreeData.rootNode?.Tick();
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 打开行为树视图
        /// </summary>
        [Button]
        public void OpenView()
        {
            BTSetting.GetSetting().TreeID = GetInstanceID();//获取物体的序列化ID(每次重新序列化时都会变)
            UnityEditor.EditorApplication.ExecuteMenuItem("Tools/BehaviorTreeWindow");//打开对应菜单地址下的按钮
        }
#endif
        public BehaviorTreeData GetTree() => TreeData;
        public void SetRoot(BtNodeBase rootData) => TreeData.rootNode = rootData;

    }
    
    /// <summary>
    /// 行为树节点信息获取接口
    /// </summary>
    public interface IGetBt
    {
        BehaviorTreeData GetTree();//获取根节点
        void SetRoot(BtNodeBase rootData);//设置根节点
    }
}