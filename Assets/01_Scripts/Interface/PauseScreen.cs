using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    [RequireComponent(typeof(VisualElement))]
    public class PauseScreen : MonoBehaviour
    {
        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<VisualElement>();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}