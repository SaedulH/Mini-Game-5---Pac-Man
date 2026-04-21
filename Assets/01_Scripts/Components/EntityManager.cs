using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntityManager : MonoBehaviour
    {
        [field: SerializeField] public IInputHandler InputHandler { get; protected set; }
        [field: SerializeField] public Movement Movement { get; protected set; }
        [field: SerializeField] public EntityAnimator Anim { get; protected set; }
        [field: SerializeField] public SkinHandler Skin { get; protected set; }

        [field: SerializeField] public NodeScript StartNode { get; set; }

        public virtual void OnGameStateUpdated(GameState gameState)
        {
            Movement.OnGameStateUpdated(gameState);
            InputHandler.OnGameStateUpdated(gameState);
        }
    }
}