using System;
using System.Collections;
using UnityEngine;

namespace Module
{
    [RequireComponent(typeof(AudioSource))]
    public class Audio : MonoBehaviour, IPoolObject
    {
        public AudioSource audioSource;

        public ObjectPool pool { get; set; }
        public AudioPlayType playType { get; set; }
        public bool autoCollection = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
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
            if (audioSource != null && playType == AudioPlayType.Music)
            {
                audioSource.volume = obj;
            }
        }

        public void ReturnToPool()
        {
            if (autoCollection)
            {
                pool.ReturnObject(this);
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
                audioSource.spatialBlend = 0;
            }
        }

        public void OnGetObjectFromPool()
        {
        }

        public void Play(AudioClip clip, bool oneShot, AudioPlay play)
        {
            if (!gameObject.activeInHierarchy) return;
            if (playType == AudioPlayType.Music)
            {
                audioSource.volume = AudioPlay.MusicVolume;
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

            StartCoroutine(PlayCoroutine(play));
        }

        private IEnumerator PlayCoroutine(AudioPlay play)
        {
            yield return play;
            play.Stop();
        }
    }
}