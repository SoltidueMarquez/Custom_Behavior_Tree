using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 只需要存在一个单例，用于获取并加载视图检查器部分的数据
    /// </summary>
    public class InspectorViewData : SerializedScriptableObject
    {
        public HashSet<BtNodeBase> DataView = new HashSet<BtNodeBase>();
    }
}