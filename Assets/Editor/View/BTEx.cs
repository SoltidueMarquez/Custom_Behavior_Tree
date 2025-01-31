
using BehaviorTree;
using Editor.View;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// 扩展方法
/// </summary>
public static class BTEx
{
    /// <summary>
    /// 连线时将对应的数据添加到线条中
    /// </summary>
    public static void LinkLineAddData(this Edge edge)
    {
        //首先获取两个节点视图
        var outputNode = edge.output.node as NodeView;
        var inputNode = edge.input.node as NodeView;
        
        switch (outputNode.NodeData)
        {
            case BtComposite composite://组合节点
                composite.ChildNodes.Add(inputNode.NodeData);
                return;
            case BtPrecondition precondition:
                precondition.ChildNode = inputNode.NodeData;
                return;
        }
    }
    
    /// <summary>
    /// 断开连线时删除数据
    /// </summary>
    public static void UnLinkLineDeleteData(this Edge edge)
    {
        //首先获取两个节点视图
        var outputNode = edge.output.node as NodeView;
        var inputNode = edge.input.node as NodeView;
        
        switch (outputNode.NodeData)
        {
            case BtComposite composite://组合节点
                composite.ChildNodes.Remove(inputNode.NodeData);
                return;
            case BtPrecondition precondition:
                precondition.ChildNode = null;
                return;
        }
    }
}
