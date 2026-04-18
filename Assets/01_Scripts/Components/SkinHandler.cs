using UnityEngine;

namespace CoreSystem
{
    public class SkinHandler : MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer BodyRenderer { get; private set; }
        [field: SerializeField] public int SkinIndex { get; private set; }

        private void Awake()
        {
            BodyRenderer = GetComponent<MeshRenderer>();    
        }

        public void AssignSkin(int skinIndex)
        {
            SkinIndex = skinIndex;
            // Implement skin assignment logic here
        }
    }
}