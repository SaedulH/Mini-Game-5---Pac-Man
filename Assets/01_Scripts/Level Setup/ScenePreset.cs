using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "ScenePreset", menuName = "Presets/ScenePreset")]
    public class ScenePreset : ScriptableObject
    {
        [field: SerializeField] public string SceneName { get; set; }
        [field: SerializeField] public LoadSceneMode LoadMode { get; set; } = LoadSceneMode.Additive;
        [field: SerializeField] public LevelInfo TrackInfo { get; set; }
        [field: SerializeField] public LevelContext LevelContext { get; set; }

        private void OnValidate()
        {
            if (TrackInfo == null || LevelContext == null) return;

            LevelContext.TotalWeight = (int)TrackInfo.StepOrder.Sum(s => s.Weight);
            //if(LevelContext.VehicleOne.VehicleName != Vehicles.AvailableVehicles[(int)PlayerOneVehicle].VehicleName)
            //{
            //    LevelContext.VehicleOne = Vehicles.AvailableVehicles[(int)PlayerOneVehicle];
            //}

            //if(LevelContext.VehicleTwo.VehicleName != Vehicles.AvailableVehicles[(int)PlayerTwoVehicle].VehicleName)
            //{
            //    LevelContext.VehicleTwo = Vehicles.AvailableVehicles[(int)PlayerTwoVehicle];
            //}
        }
    }
}