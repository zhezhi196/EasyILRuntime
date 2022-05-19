using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module
{
    [RequireComponent(typeof(AudioSource))]
    public class Audio : MonoBehaviour, IPoolObject
    {

        public AudioSource audioSource;
        
        [SerializeField]
        protected AudioPlayType _playType;

        public ObjectPool pool { get; set; }

        public AudioPlayType playType
        {
            get { return _playType; }
            set { _playType = value; }
        }

        public bool autoCollection = false;
        private Coroutine playCoroutine;
        public bool playOnAwake;

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            if (playType == AudioPlayType.Music)
            {
                audioSource.volume = AudioPlay.MusicVolume;
            }
            else if (playType == AudioPlayType.Audio)
            {
                audioSource.volume = AudioPlay.AudioVolume;
            }

            audioSource.mute = AudioPlay.Mute;
            AudioPlay.onMusicVolumeChanged += OnMusicVolumeChanged;
            AudioPlay.onAudioVolumeChanged += OnAudioVolumeChanged;
            AudioPlay.onMuteChanged += OnMuteChanged;
            if (playOnAwake && audioSource != null && audioSource.clip != null)
            {
                AudioPlay.PlayDefault(this);
            }
        }

        private void OnDisable()
        {
            AudioPlay.onMusicVolumeChanged -= OnMusicVolumeChanged;
            AudioPlay.onAudioVolumeChanged -= OnAudioVolumeChanged;
            AudioPlay.onMuteChanged -= OnMuteChanged;
        }

        private void OnDestroy()
        {
            for (int i = AudioPlay._allPlay.Count - 1; i >= 0; i--)
            {
                if (AudioPlay._allPlay[i].audioSource == this)
                {
                    AudioPlay._allPlay.RemoveAt(i);
                }
            }
        }

        protected virtual void OnMuteChanged(bool obj)
        {
            if (audioSource != null)
            {
                audioSource.mute = obj;
            }
        }

        protected virtual void OnAudioVolumeChanged(float obj)
        {
            if (audioSource != null && playType == AudioPlayType.Audio)
            {
                audioSource.volume = obj;
            }
        }

        protected virtual void OnMusicVolumeChanged(float obj)
        {
            if (audioSource != null && (playType == AudioPlayType.Music))
            {
                audioSource.volume = obj;
            }
        }

        public void ReturnToPool()
        {
            if (pool != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
                audioSource.spatialBlend = 0;
                transform.SetParent(ObjectPool.poolRoot);
            }
        }

        public void OnGetObjectFromPool()
        {
        }


        public virtual void Play(AudioClip clip, AudioPlay play, bool onShot, AudioPlay.AudioFlag flag)
        {
            if (!gameObject.activeInHierarchy) return;
            if (playType == AudioPlayType.Music)
            {
                OnMusicVolumeChanged(AudioPlay.MusicVolume);
                if (playCoroutine != null) StopCoroutine(playCoroutine);
            }
            else if (playType == AudioPlayType.Audio)
            {
                OnAudioVolumeChanged(AudioPlay.AudioVolume);
            }

            OnMuteChanged(AudioPlay.Mute);
            
             if (onShot)
             {
                 audioSource.PlayOneShot(clip);
             }
             else
             {
                 if ((flag & AudioPlay.AudioFlag.RestartOnSame) != 0 || audioSource.clip != clip)
                 {
                     audioSource.clip = clip;
                 }
                 audioSource.Play();
             }
             
             playCoroutine = StartCoroutine(PlayCoroutine(play,flag));
        }

        private IEnumerator PlayCoroutine(AudioPlay play,AudioPlay.AudioFlag flat)
        {
            yield return play;
            playCoroutine = null;
            play.Stop(true);
            if ((flat & AudioPlay.AudioFlag.CompleteNotNull) == 0)
            {
                audioSource.clip = null;
            }
        }

        public void StopPlayAndCoroutine()
        {
            audioSource.Stop();
            if (playCoroutine != null) StopCoroutine(playCoroutine);
        }

[Button]
        public void TestPlay()
        {
            if (Application.isPlaying)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }
    }
}