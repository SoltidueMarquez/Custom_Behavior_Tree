using System.Collections.Generic;
using System.Linq;
using BehaviorTree;
using Editor.View;
using Sirenix.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
    
    /// <summary>
    /// 用odin序列化去克隆选择的节点
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static List<BtNodeBase> CloneData(this List<BtNodeBase> nodes)
    {
        var nodeBytes= SerializationUtility.SerializeValue(nodes, DataFormat.Binary);//使用奥丁的序列化方法，序列化为字节流
        var toNode = SerializationUtility.DeserializeValue<List<BtNodeBase>>(nodeBytes ,DataFormat.Binary);
            
        //删掉未复制的子数据 并随机新的Guid 位置向右下偏移
        for (int i = 0; i < toNode.Count; i++)
        {
            toNode[i].Guid = System.Guid.NewGuid().ToString();//节点id的唯一标识需要重新随机 
            switch (toNode[i])
            {
                case BtComposite composite:
                    if (composite.ChildNodes .Count==0)break;
                    composite.ChildNodes = composite.ChildNodes.Intersect(toNode).ToList();//只保留当前选择的节点
                    break;
                case BtPrecondition precondition :
                    if (precondition.ChildNode == null)break;
                    if (!toNode.Exists(n => n == precondition.ChildNode))
                    {
                        precondition.ChildNode = null;
                    }
                    break;
            }
            toNode[i].Position += Vector2.one * 30;//将位置偏移一些
        }
            
        return toNode;
    }
}
