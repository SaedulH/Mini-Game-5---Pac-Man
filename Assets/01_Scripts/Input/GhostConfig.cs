using UnityEngine;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "Ghost Config", menuName = "Ghosts/GhostConfig")]
    public class GhostConfig : ScriptableObject
    {
        [field: SerializeField] public Vector3 Corner { get; set; }
        [field: SerializeField] public int StartPellets { get; set; }
        [field: SerializeField] public float StartTimer { get; set; }
        [field: SerializeField] public int EndPellets { get; set; }
        [field: SerializeField] public float EndTimer { get; set; }
        [field: SerializeField] public bool CanExitImmediately { get; set; }
        [field: SerializeField] public ControlInput InitialInput { get; set; }
        [field: SerializeField] public Material Material { get; set; }
    }
}