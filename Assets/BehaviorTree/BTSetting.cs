using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 只需要存在一个单例，用于获取行为树的节点信息以便于在打开时构建视图
    /// </summary>
    public class BTSetting : SerializedScriptableObject
    {
        public int TreeID;

        public static BTSetting GetSetting()
        {
            return Resources.Load<BTSetting>("BTSetting");
        }
#if UNITY_EDITOR
        public IGetBt GetTree() => UnityEditor.EditorUtility.InstanceIDToObject(TreeID) as IGetBt;
        
        public void SetRoot(BtNodeBase rootNode) => GetTree().SetRoot(rootNode);
#endif
    }
}