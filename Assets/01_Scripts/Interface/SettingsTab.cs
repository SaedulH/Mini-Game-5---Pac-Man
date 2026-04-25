using AudioSystem;
using CoreSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace SettingsSystem
{
    public abstract class SettingsTab : MonoBehaviour
    {
        [field: SerializeField] public VisualElement SettingsScreen { get; set; }

        public virtual void InitialiseSettings(VisualElement root)
        {

        }

        public virtual IEnumerator ShowSettingsTab(float transitionTime)
        {
            GetSettings();

            yield return new WaitForSeconds(transitionTime);
            SettingsScreen.SetEnabled(true);
            SettingsScreen.RemoveFromClassList("hide");
        }

        protected virtual void GetSettings()
        {

        }

        public virtual void ResetToDefaults()
        {

        }

        public void PlayResetAudio()
        {
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.5f)
                .Play(AudioCollection.Instance.ResetAudio);
        }
    }
}
