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

        private GameState _currentGameState;
        private LevelState _currentLevelState;

        public virtual void OnGameStateUpdated(GameState gameState)
        {
            _currentGameState = gameState;
            SetActiveState(gameState, _currentLevelState);
        }

        public virtual void OnLevelStateUpdated(LevelState levelState)
        {
            _currentLevelState = levelState;
            SetActiveState(_currentGameState, levelState);

            if (levelState.Equals(LevelState.Respawning))
            {
                _ = ResetPosition();
            }
        }

        public virtual void SetActiveState(GameState gameState, LevelState levelState)
        {
            bool isActive = gameState.Equals(GameState.Playing) && levelState.Equals(LevelState.Active);

            Movement.SetActiveState(isActive);
            InputHandler.SetActiveState(isActive);
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