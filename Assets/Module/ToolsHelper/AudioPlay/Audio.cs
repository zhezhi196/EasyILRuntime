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

        public ObjectPool pool { get; set; }
        public AudioPlayType playType { get; set; }
        public bool autoCollection = false;
        private Coroutine playCoroutine;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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
        }

        private void OnDisable()
        {
            AudioPlay.onMusicVolumeChanged -= OnMusicVolumeChanged;
            AudioPlay.onAudioVolumeChanged -= OnAudioVolumeChanged;
            AudioPlay.onMuteChanged -= OnMuteChanged;
        }

        private void OnMuteChanged(bool obj)
        {
            if (audioSource != null)
            {
                audioSource.mute = obj;
            }
        }

        private void OnAudioVolumeChanged(float obj)
        {
            if (audioSource != null && playType == AudioPlayType.Audio)
            {
                audioSource.volume = obj;
            }
        }

        private void OnMusicVolumeChanged(float obj)
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
                pool.ReturnObject(this);
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

        public void Play(AudioClip clip, bool oneShot, AudioPlay play, bool autoStop = true)
        {
            if (!gameObject.activeInHierarchy) return;
            if (playType == AudioPlayType.Music)
            {
                audioSource.volume = AudioPlay.MusicVolume;
                if (playCoroutine != null) StopCoroutine(playCoroutine);
            }
            else if (playType == AudioPlayType.Audio)
            {
                audioSource.volume = AudioPlay.AudioVolume;
            }
            
             audioSource.mute = AudioPlay.Mute;
            
             if (oneShot)
             {
                 audioSource.PlayOneShot(clip);
             }
             else
             {
                 audioSource.clip = clip;
                 audioSource.Play();
             }

             if (autoStop)
             {
                 playCoroutine = StartCoroutine(PlayCoroutine(play));
             }
        }

        private IEnumerator PlayCoroutine(AudioPlay play)
        {
            yield return play;
            playCoroutine = null;
            play.Stop();
        }

        public void StopPlay()
        {
            audioSource.Stop();
            if (playCoroutine != null) StopCoroutine(playCoroutine);
        }

[Button]
        public void TestPlay()
        {
            audioSource.Play();
        }
    }
}