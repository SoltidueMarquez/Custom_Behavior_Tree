using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BehaviorTree
{
    public class TestBT : SerializedMonoBehaviour, IGetBt
    {
        [OdinSerialize] public BtNodeBase rootNode;

        private void Update()
        {
            rootNode?.Tick();
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
        public BtNodeBase GetRoot() => rootNode;
    }
    
    public interface IGetBt
    {
        BtNodeBase GetRoot();
    }
}