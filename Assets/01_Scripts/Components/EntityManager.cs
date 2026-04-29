using EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntityManager : MonoBehaviour
    {
        [field: SerializeField] public IInputHandler InputHandler { get; protected set; }
        [field: SerializeField] public Movement Movement { get; protected set; }
        [field: SerializeField] public EntityAnimator Anim { get; protected set; }

        [field: SerializeField] public NodeScript StartNode { get; set; }
        [field: SerializeField] public EventChannel OnHit { get; private set; }

        public virtual void OnGameStateUpdated(GameState gameState)
        {
            Movement.OnGameStateUpdated(gameState);
            InputHandler.OnGameStateUpdated(gameState);
        }

        public virtual void OnLevelStateUpdated(LevelState levelState)
        {
            Movement.OnLevelStateUpdated(levelState);
            InputHandler.OnLevelStateUpdated(levelState);

            if (levelState.Equals(LevelState.Resetting))
            {
                _ = ResetPosition();
            }
        }

        protected virtual async Task ResetPosition()
        {

        }

        public virtual void OnHitEvent()
        {
            OnHit.Invoke(new Empty());
        }

        protected virtual void OnDeathEvent()
        {
        }
    }
}