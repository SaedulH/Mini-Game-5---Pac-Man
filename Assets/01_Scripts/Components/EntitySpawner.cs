using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class EntitySpawner : NonPersistentSingleton<GameManager>
    {
        [field: SerializeField] public GameObject PacManPrefab { get; private set; }
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clive")] public GhostConfig[] GhostConfigs { get; private set; }

        public (PlayerManager pacman, GhostManager[] ghosts) SetupEntities(PlayerInputActions inputActions, int levelNumber)
        {
            PlayerManager playerManager = SetupPlayer(inputActions, levelNumber);

            GhostManager[] ghosts = new GhostManager[4];

            // Setup AI
            GhostManager blinky = SetupGhost(ghosts, GhostType.Blinky, levelNumber);
            SetupGhost(ghosts, GhostType.Inky, levelNumber);
            SetupGhost(ghosts, GhostType.Pinky, levelNumber);
            SetupGhost(ghosts, GhostType.Clive, levelNumber);

            BindEntities(playerManager, ghosts, blinky.transform);

            return (playerManager, ghosts);
        }

        public async Task SetupEntitySpawnpoints(PlayerManager pacman, Vector3 pacManSpawnPosition, Quaternion playerOneSpawnRotation,
            GhostManager[] ghosts, Vector3[] ghostSpawnPosition, Quaternion[] ghostSpawnRotation)
        {
            pacman.SetSpawnpoint(pacManSpawnPosition, playerOneSpawnRotation);
            if (ghosts != null && ghosts.Length > 0)
            {
                for (int i = 0; i < ghosts.Length; i++)
                {
                    ghosts[i].SetSpawnpoint(ghostSpawnPosition[i], ghostSpawnRotation[i]);
                }
            }
            ghosts[0].InputHandler.RespawnNode = ghosts[2].StartNode;
        }

        public PlayerManager SetupPlayer(PlayerInputActions inputActions, int levelNumber)
        {
            PlayerManager pacman = Instantiate(PacManPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            pacman.name = "PacMan";
            pacman.InitialisePlayer(inputActions, levelNumber);

            return pacman;
        }

        public GhostManager SetupGhost(GhostManager[] ghosts, GhostType ghostType, int levelNumber)
        {
            GhostManager ghost = Instantiate(GhostPrefab, Vector3.zero, Quaternion.identity).GetComponent<GhostManager>();
            ghost.name = ghostType.ToString();

            int index = GetGhostIndex(ghostType);
            ghosts[index] = ghost;
            GhostConfig config = GhostConfigs[index];

            var renderer = ghost.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = config.Material;
            }
            ghost.InitialiseGhost(ghostType, config, levelNumber);

            return ghost;
        }

        public void BindEntities(PlayerManager pacman, GhostManager[] ghosts, Transform blinky)
        {
            if (ghosts == null || ghosts.Length != 4)
            {
                Debug.LogError("Ghost references are not properly set up. Cannot assign target transforms.");
                return;
            }
            ghosts[0].SetTargets(pacman.transform, pacman);
            ghosts[1].SetTargets(blinky, pacman);
            ghosts[2].SetTargets(pacman.transform, pacman);
            ghosts[3].SetTargets(pacman.transform, pacman);
        }

        private int GetGhostIndex(GhostType type)
        {
            return type switch
            {
                GhostType.Blinky => 0,
                GhostType.Inky => 1,
                GhostType.Pinky => 2,
                GhostType.Clive => 3,
                _ => 0
            };
        }
    }
}
