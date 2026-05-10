using CoreSystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Utilities;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioCollection))]
    [RequireComponent(typeof(MusicManager))]
    public class AudioManager : NonPersistentSingleton<AudioManager>
    {
        [field: SerializeField] public MusicManager MusicManager { get; private set; }
        [field: SerializeField] public AudioCollection AudioCollection { get; private set; }

        IObjectPool<AudioEmitter> AudioEmitterPool;
        readonly List<AudioEmitter> activeAudioEmitters = new();
        public readonly LinkedList<AudioEmitter> FrequentAudioEmitters = new();

        [SerializeField] AudioEmitter audioEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;

        public void Initialise()
        {
            InitializePool();
            MusicManager = GetComponent<MusicManager>();
            AudioCollection = GetComponent<AudioCollection>();

            MusicManager.Initialise();
            AudioCollection.Initialise();
        }

        public AudioBuilder CreateAudioBuilder() => new(this);

        public bool CanPlaySound(AudioData data)
        {
            if (!data.frequentSound) return true;

            if (FrequentAudioEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentAudioEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("AudioEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public AudioEmitter Get()
        {
            return AudioEmitterPool.Get();
        }

        public void ReturnToPool(AudioEmitter soundEmitter)
        {
            soundEmitter.transform.parent = transform;
            AudioEmitterPool.Release(soundEmitter);
        }

        public void StopAll()
        {
            foreach (var soundEmitter in activeAudioEmitters)
            {
                soundEmitter.Stop();
            }

            FrequentAudioEmitters.Clear();
        }

        void InitializePool()
        {
            AudioEmitterPool = new ObjectPool<AudioEmitter>(
                CreateAudioEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }

        AudioEmitter CreateAudioEmitter()
        {
            var soundEmitter = Instantiate(audioEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(AudioEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeAudioEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(AudioEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentAudioEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeAudioEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(AudioEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        public async Task ResetComponent()
        {
            StopAll();
            AudioEmitterPool.Clear();
            await Task.CompletedTask;
        }
    }
}