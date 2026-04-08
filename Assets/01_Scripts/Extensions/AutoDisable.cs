using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class AutoDisable : MonoBehaviour
    {
        [field: SerializeField] private float _disableDelay = 1f;
        [field: SerializeField] private bool _isDisableSelf = false;
        [field: SerializeField] private bool _isDisableFromList = false;
        [field: SerializeField] private GameObject[] _disableList;

        private void Start()
        {
            StartCoroutine(DisableOnDelay());
        }

        private IEnumerator DisableOnDelay()
        {
            yield return new WaitForSeconds(_disableDelay);
            DisableFromList();
            DisableSelf();
        }

        private void DisableSelf()
        {
            if (_isDisableSelf)
            {
                gameObject.SetActive(false);
            }
        }

        private void DisableFromList()
        {
            if (_isDisableFromList)
            {
                foreach (var item in _disableList)
                {
                    item.SetActive(false);
                }
            }
        }
    }
}