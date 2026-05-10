using AudioSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem
{
    public class CoreSceneInitialiser : MonoBehaviour
    {
        [field: Header("Managers")]
        [field: SerializeField] public GameManager GameManager { get; private set; }
        [field: SerializeField] public AudioManager AudioManager { get; private set; }
        [field: SerializeField] public UIManager UIManager { get; private set; }

        [field: Header("Components")]
        [field: SerializeField] public GameObject MainCamera { get; private set; }
        [field: SerializeField] public GameObject EventSystem { get; private set; }

        [field: SerializeField] public PlayerInputActions InputActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();
        }

        private async void Start()
        {
            BindComponents();

            await InitialiseComponents();

            await CreateObjects();
        }

        private void BindComponents()
        {
            GameManager = Instantiate(GameManager);
            GameManager.name = "GameManager";
            AudioManager = Instantiate(AudioManager);
            AudioManager.name = "AudioManager";
            UIManager = Instantiate(UIManager);
            UIManager.name = "UIManager";

            MainCamera = Instantiate(MainCamera);
            MainCamera.name = "MainCamera";
            EventSystem = Instantiate(EventSystem);
            EventSystem.name = "EventSystem";
        }

        private async Task InitialiseComponents()
        {
            GameManager.Initialise(InputActions);
            AudioManager.Initialise();
            UIManager.Initialise(InputActions);
        }

        private async Task CreateObjects()
        {
            GameManager.InitialiseMenu();
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }
    }
}
