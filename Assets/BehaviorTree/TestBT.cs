using BehaviorTree;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class TestBt : SerializedMonoBehaviour
{
    [OdinSerialize] public BtNodeBase rootNode;//奥丁的序列化可以序列化抽象类与泛型，但需要是奥丁的MonoBehaviour
    
}
