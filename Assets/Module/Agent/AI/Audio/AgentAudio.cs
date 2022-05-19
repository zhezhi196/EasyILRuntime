using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module
{
    public class AgentAudio : Audio
    {
        [SerializeField]
        public string key;

        public IAgentAudioObject owner;

        public void Refresh()
        {
            OnAudioVolumeChanged(AudioPlay.AudioVolume);
            OnAudioVolumeChanged(AudioPlay.MusicVolume);
        }

        public override void Play(AudioClip clip, AudioPlay play, bool oneShot, AudioPlay.AudioFlag flag)
        {
            if (!owner.GetAudioActive(this))
            {
                return;
            }

            base.Play(clip, play, oneShot, flag);
        }

        protected override void OnAudioVolumeChanged(float obj)
        {
            if (audioSource != null && playType == AudioPlayType.Audio)
            {
                audioSource.volume = obj * owner.GetAudioVolume(this);
            }
        }

        protected override void OnMusicVolumeChanged(float obj)
        {
            if (audioSource != null && (playType == AudioPlayType.Music))
            {
                audioSource.volume = obj * owner.GetAudioVolume(this);
            }
        }
    }
}