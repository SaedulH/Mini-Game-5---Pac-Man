using AudioSystem;
using CoreSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public abstract class SettingsTab : MonoBehaviour
    {
        [field: SerializeField] public Tab TabElement { get; set; }

        public virtual IEnumerator ShowSettingsTab(float transitionTime)
        {
            GetSettings();

            yield return new WaitForSeconds(transitionTime);
            TabElement.SetEnabled(true);
            TabElement.RemoveFromClassList("hide");
        }

        protected virtual void GetSettings()
        {

        }

        public virtual bool OnBackClicked(SettingsType settingsType)
        {
            return true;
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
