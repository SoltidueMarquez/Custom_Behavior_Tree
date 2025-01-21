using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace BehaviorTree
{
    public class Test : SerializedMonoBehaviour
    {
        [OdinSerialize] public BtNodeBase rootNode;

        private void Update()
        {
            rootNode?.Tick();
        }
    }
}