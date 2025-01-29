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
    }
}