using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GameSceneInitialiser : NonPersistentSingleton<GameSceneInitialiser>
    {
        [field: SerializeField] public MazeGenerator MazeGenerator { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (MazeGenerator == null)
            {
                MazeGenerator = GetComponentInChildren<MazeGenerator>();
            }
        }

        public void Initialise(LevelContext levelContext)
        {
            MazeGenerator.Initialise(levelContext);
        }
    }
}
