using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class FruitItemScript : ItemScript
    {
        [field: SerializeField] public FruitType FruitType { get; private set; }

        protected override void SetItemScore()
        {
            Score = Constants.GetFruitScore(FruitType);
        }

        public void SetFruitType(int currentLevel)
        {
            FruitType = Constants.GetFruitTypeForLevel(currentLevel);
            Score = Constants.GetFruitScore(FruitType);
            SetFruitBody(FruitType);
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(10f);
            OnCollectedEvent();
        }

        private void SetFruitBody(FruitType fruitType)
        {
            if (Body == null)
            {
                Debug.LogError("Body is not assigned in the inspector.");
                return;
            }

            string targetName = fruitType.ToString();

            foreach (Transform child in Body.GetComponentsInChildren<Transform>(true))
            {
                if (child == Body.transform) continue;

                bool isTargetFruit = child.name.Equals(targetName, StringComparison.OrdinalIgnoreCase);
                child.gameObject.SetActive(isTargetFruit);
            }
        }
    }
}