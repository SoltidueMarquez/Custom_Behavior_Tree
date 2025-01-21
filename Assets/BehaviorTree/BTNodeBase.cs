using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
        [FoldoutGroup("基础数据"), LabelText("名称")] public string NodeName;
        
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

    /// <summary>
    /// 顺序节点
    /// </summary>
    public class Sequence : BtComposite
    {
        private int m_Index;//这一帧下一个执行的子节点索引
        
        /// <summary>
        /// 返回运行状态
        /// </summary>
        /// <returns></returns>
        public override BehaviourState Tick()
        {
            var state = ChildNodes[m_Index].Tick();
            switch (state)
            {
                case BehaviourState.成功:                //完成了就按顺序往下执行
                    m_Index++;
                    if (m_Index >= ChildNodes.Count) {  //全部执行完了，重置并返回成功
                        m_Index = 0;
                        return BehaviourState.成功;
                    }else {                              //没执行完就返回执行中
                        return BehaviourState.执行中;
                    }
                case BehaviourState.失败:              //失败了就重置，重新开始
                    m_Index = 0;
                    return BehaviourState.失败;
                case BehaviourState.执行中:            //子节点未执行完，返回执行中
                    return state;
            }

            return BehaviourState.未执行;
        }
    }
    
    /// <summary>
    /// 延时执行节点
    /// 使用计时器，计时器超过时返回成功
    /// </summary>
    public class Delay : BtPrecondition
    {
        [LabelText("延时"), SerializeField] private float timer;

        private float _currentTimer;
        public override BehaviourState Tick()
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer>=timer)
            {
                _currentTimer = 0f;
                ChildNode.Tick();
                return BehaviourState.成功;
            }
            return BehaviourState.执行中;
        }
    }
    
    /// <summary>
    /// 对物体执行禁用/启用 节点
    /// </summary>
    public class SetObjectActive : BtActionNode
    {
        [LabelText("是否启用"), SerializeField, FoldoutGroup("@NodeName")]
        private bool m_IsActive;

        [LabelText("启用对象"), SerializeField, FoldoutGroup("@NodeName")]
        private GameObject m_Particle;
        
        public override BehaviourState Tick()
        {
            m_Particle.SetActive(m_IsActive);
            Debug.Log($"{NodeName}节点 {(m_IsActive ? "启用了" : "禁用了")}");
            return BehaviourState.成功;
        }
    }
}
