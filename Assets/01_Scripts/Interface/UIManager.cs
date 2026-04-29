using CoreSystem;
using SettingsSystem;
using System.Threading.Tasks;
using UnityEngine;
using UserInterface;
using Utilities;

public class UIManager : NonPersistentSingleton<UIManager>
{
    [field: SerializeField] public UIState CurrentUIState { get; private set; }
    [field: SerializeField] public HUDManager HUDManager { get; private set; }
    [field: SerializeField] public PauseScreen PauseScreen { get; private set; }
    [field: SerializeField] public StartScreen StartScreen { get; private set; }
    [field: SerializeField] public SettingsManager SettingsManager { get; private set; }
    [field: SerializeField] public ResultsScreen ResultsScreen { get; private set; }
    [field: SerializeField] public LoadingScreen LoadingScreen { get; private set; }
    private UIState _previousUIState;

    protected override void Awake()
    {
        base.Awake();
        HUDManager = GetComponentInChildren<HUDManager>();
        PauseScreen = GetComponentInChildren<PauseScreen>();
        StartScreen = GetComponentInChildren<StartScreen>();
        SettingsManager = GetComponentInChildren<SettingsManager>();
        ResultsScreen = GetComponentInChildren<ResultsScreen>();
        LoadingScreen = GetComponentInChildren<LoadingScreen>();
    }

    public void OnGameStateUpdated(GameState gameState)
    {
        HUDManager.OnGameStateUpdated(gameState);
        PauseScreen.OnGameStateUpdated(gameState);
        //StartScreen.OnGameStateUpdated(gameState);
        //ResultsScreen.OnGameStateUpdate(gameState);
        //LoadingScreen.OnGameStateUpdate(gameState);
    }


    public void OnUIStateChanged(UIState newUIState)
    {
        _previousUIState = CurrentUIState;
        Debug.Log($"UI State changed from [{CurrentUIState}] to [{newUIState}]");
        CurrentUIState = newUIState;
    }

    public async Task SetupHUD(LevelContext context)
    {
        await HUDManager.SetupHUD(context);
    }

    public async Task BeginCountdown(float duration)
    {
        await HUDManager.BeginCountdown(duration);
    }
}
