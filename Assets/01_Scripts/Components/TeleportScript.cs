using UnityEngine;

namespace CoreSystem
{
    public class TeleportScript : MonoBehaviour
    {
        [field: SerializeField] public NodeScript ParentNode { get; private set; }

        public void SetParentNode(NodeScript parentNode)
        {
            ParentNode = parentNode;
        }
    }
}