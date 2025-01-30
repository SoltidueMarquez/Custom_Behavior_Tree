using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu]
    public class BTSetting : ScriptableObject
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