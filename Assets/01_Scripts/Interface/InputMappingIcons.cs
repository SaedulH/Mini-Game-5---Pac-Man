using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "InputMappingIcons", menuName = "Mapping/InputMappingIcons")]
    public class InputMappingIcons : ScriptableObject
    {
        [field: SerializeField] public InputKeyIconMap[] InputKeyIconMap { get; private set; }

        public InputKeyIconMap GetInputMapForInputKey(string inputKey)
        {
            //Debug.Log($"Searching for icon with input key: {inputKey}");
            foreach (var mapping in InputKeyIconMap)
            {
                if (mapping.InputKey == inputKey)
                {
                    return mapping;
                }
            }
            return null;
        }
    }
}
