using System.Collections.Generic;

namespace Module
{
    public class AgentAudioCtrl : IAgentCtrl
    {
        private List<object> _pauseList = new List<object>();
        private AudioPlay musicAudio;
        
        public IAgentAudioObject owner { get; }

        public AgentAudioCtrl(IAgentAudioObject owner)
        {
            this.owner = owner;
        }

        public AudioPlay PlayAgentMusic(string name, string audio)
        {
            var ma = owner.GetAudio(name);
            AudioPlay.bgm.audioSource.enabled = false;
            return null;
        }

        public void PlayBackMusic()
        {
            if (musicAudio != null)
            {
                musicAudio.Stop();
            }
            
            AudioPlay.bgm.audioSource.enabled = true;
        }

        public AudioPlay Play(string name, string audio, bool loop)
        {
            Audio tar = owner.GetAudio(audio);
            return AudioPlay.Play(name, tar).SetLoop(loop);
        }

        public AudioPlay PlayOneShot(string name, string audio)
        {
            Audio tar = owner.GetAudio(audio);
            return AudioPlay.PlayOneShot(name, tar);
        }

        public void OnCreat()
        {
            this.owner.onSwitchStation += OnOwnerSwithStation;
        }

        private void OnOwnerSwithStation()
        {
            for (int i = 0; i < owner.allAudio.Length; i++)
            {
                owner.allAudio[i].audioSource.mute = AudioPlay.Mute && owner.AudioIsMute(owner.allAudio[i]);
            }
        }

        public void OnBorn()
        {
        }

        public void OnUpdate()
        {
        }

        public void Pause(object key)
        {
            if (!_pauseList.Contains(key))
            {
                _pauseList.Add(key);
            }

            for (int i = 0; i < owner.allAudio.Length; i++)
            {
                owner.allAudio[i].audioSource.Pause();
            }
        }

        public void Continue(object key)
        {
            if (_pauseList.Contains(key))
            {
                _pauseList.Remove(key);
            }

            if (_pauseList.Count == 0)
            {
                for (int i = 0; i < owner.allAudio.Length; i++)
                {
                    owner.allAudio[i].audioSource.UnPause();
                }
            }
        }

        public void OnAgentDead()
        {
            
        }

        public void OnDestroy()
        {
            this.owner.onSwitchStation -= OnOwnerSwithStation;
        }


        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            return owner.GetAgentCtrl<T>();
        }
    }
}