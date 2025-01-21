using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BehaviorTree
{
    /// <summary>
    /// 节点状态枚举
    /// </summary>
    public enum BehaviourState
    {
        未执行,成功,失败,执行中
    }
    
    /// <summary>
    /// 行为树节点基类
    /// </summary>
    public abstract class BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("名称")] public string NodeName;
        
        /// <summary>
        /// 传递信号方法
        /// </summary>
        /// <returns></returns>
        public abstract BehaviourState Tick();
    }
    
    /// <summary>
    /// 组合节点
    /// </summary>
    public abstract class BtComposite : BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("子节点")]
        public List<BtNodeBase> ChildNodes = new List<BtNodeBase>();
    }

    /// <summary>
    /// 条件节点
    /// </summary>
    public abstract class BtPrecondition : BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("子节点")]
        public BtNodeBase ChildNode;
    }

    /// <summary>
    /// 行为节点
    /// </summary>
    public abstract class BtActionNode : BtNodeBase { }
    
}
