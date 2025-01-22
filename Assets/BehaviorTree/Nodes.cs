using Sirenix.OdinInspector;
using UnityEngine;

namespace BehaviorTree
{
    public class Nodes
    {
        
    }
    
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
    /// 选择节点
    /// </summary>
    public class Selector : BtComposite
    {
        [LabelText("选择的index"), FoldoutGroup("@NodeName")]
        private int m_SelectIndex;
        
        public override BehaviourState Tick()
        {
            var state = ChildNodes[m_SelectIndex].Tick();//运行当前的选择节点
            switch (state)
            {
                case BehaviourState.成功:
                    m_SelectIndex = 0;
                    return state;
                case BehaviourState.失败:
                    m_SelectIndex++;                        //继续执行下一个节点
                    if (m_SelectIndex >= ChildNodes.Count)  //如果所有的节点都失败则返回失败
                    {
                        m_SelectIndex = 0;                  //从头开始
                        return BehaviourState.失败;
                    }
                    break;
                default:
                    return state;
            }
            return BehaviourState.失败;
        }
    }
    
    /// <summary>
    /// 延时执行节点
    /// 使用计时器，计时器超过时返回成功
    /// </summary>
    public class Delay : BtPrecondition
    {
        [LabelText("延时"), SerializeField, FoldoutGroup("@NodeName")]
        private float m_Timer;

        private float m_CurrentTimer;
        public override BehaviourState Tick()
        {
            m_CurrentTimer += Time.deltaTime;
            if (m_CurrentTimer >= m_Timer) 
            {
                m_CurrentTimer = 0f;
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

    /// <summary>
    /// 判定节点
    /// </summary>
    public class So : BtPrecondition
    {
        [LabelText("是否Active"), SerializeField, FoldoutGroup("@NodeName")]
        private bool m_IsActive;
        public override BehaviourState Tick()
        {
            if (m_IsActive)//如果为真则执行子节点
            {
                return ChildNode.Tick();
            }
            return BehaviourState.失败;
        }
    }
}