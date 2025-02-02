using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 视图的位置、旋转、缩放等信息
    /// </summary>
    public class GraphViewTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Matrix4x4 matrix;
    }
    
    [BoxGroup]
    [HideReferenceObjectPicker]
    public class BehaviorTreeData
    {
        public BtNodeBase rootNode;
        
        public GraphViewTransform viewTransform = new GraphViewTransform();
    }
    
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
    [BoxGroup]
    [HideReferenceObjectPicker]
    public abstract class BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("唯一标识")] public string Guid;
        [FoldoutGroup("@NodeName"), LabelText("节点位置")] public Vector2 Position;
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
