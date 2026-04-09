using System.Threading.Tasks;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[RequireComponent(typeof(UIDocument))]
public class LoadingScreen : NonPersistentSingleton<LoadingScreen>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement LoadingScreenElement { get; set; }
    [field: SerializeField] public Label StageNumber { get; set; }
    [field: SerializeField] public Slider LoadingBar { get; protected set; }
    [field: SerializeField, Min(0f)] public float CurrentProgress { get; protected set; }
    [field: SerializeField, Min(0f)] public float MaxProgress { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        Root = GetComponent<UIDocument>().rootVisualElement;
        LoadingScreenElement = Root.Q<VisualElement>("LoadingScreen");
        LoadingScreenElement.AddToClassList("hide");
        LoadingScreenElement.style.display = DisplayStyle.None;

        StageNumber = LoadingScreenElement.Q<Label>("StageNumber");
        LoadingBar = LoadingScreenElement.Q<Slider>("LoadingBar");
    }

    public async Task SetLevelInfo(LevelContext context)
    {
        if(context.StageNumber == 0)
        {
            StageNumber.AddToClassList("hide");
        } 
        else
        {
            StageNumber.RemoveFromClassList("hide");
        }

        CurrentProgress = 0f;
        MaxProgress = context.TotalWeight;

        LoadingBar.value = 0f;

        await Task.CompletedTask;
    }

    public async Task ShowLoadingScreen()
    {
        Debug.Log("Show Loading Screen");

        LoadingScreenElement.style.display = DisplayStyle.Flex;
        await Task.Yield();
        LoadingScreenElement.RemoveFromClassList("hide");

        await Task.Delay(400);
    }

    public async Task HideLoadingScreen()
    {
        Debug.Log("Hide Loading Screen");
        
        LoadingScreenElement.AddToClassList("hide");
        await Task.Delay(400);
        LoadingScreenElement.style.display = DisplayStyle.None;
    }

    public void UpdateLoadingProgress(float weight)
    {
        CurrentProgress += weight;
        LoadingBar.value = (CurrentProgress / MaxProgress) * 100;
    }
}
