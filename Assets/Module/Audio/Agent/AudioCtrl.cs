using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Module
{
    public class AudioCtrl : IAgentCtrl
    {
        #region 字段 属性 事件

        private bool _isPause;
        private Dictionary<int, List<AudioPlay>> allPlay = new Dictionary<int, List<AudioPlay>>();
        public IAgentAudioObject owner { get; }
        public bool isPause => _isPause;
        public string logName => owner.logName;
        public bool isLog => owner.isLog;

        #endregion

        /// <summary>
        /// 增加动画层级音效
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="play"></param>
        private void TryAdd(int layer, AudioPlay play)
        {
            List<AudioPlay> temp = null;
            if (!allPlay.TryGetValue(layer, out temp))
            {
                temp = new List<AudioPlay>();
                allPlay.Add(layer, temp);
            }
            
            temp.Add(play);
            play.onStop += re =>
            {
                temp.Remove(play);
            };
        }
        
        public AudioCtrl(IAgentAudioObject owner)
        {
            this.owner = owner;
            // if (!owner.allAudio.IsNullOrEmpty())
            // {
            //     for (int i = 0; i < owner.allAudio.Length; i++)
            //     {
            //         owner.allAudio[i].owner = owner;
            //     }
            // }
            OnOwnerSwithStation();
            this.owner.onSwitchStation += OnOwnerSwithStation;
        }

        private void OnOwnerSwithStation()
        {
            // if (!owner.allAudio.IsNullOrEmpty())
            // {
            //     for (int i = 0; i < owner.allAudio.Length; i++)
            //     {
            //         owner.allAudio[i].Refresh();
            //     }
            // }
        }

        public void OnUpdate()
        {
        }

        public void Pause()
        {
            // for (int i = 0; i < owner.allAudio.Length; i++)
            // {
            //     owner.allAudio[i].audioSource.Pause();
            // }
            //
            // _isPause = true;
        }

        public void Continue()
        {
            // if (_isPause)
            // {
            //     for (int i = 0; i < owner.allAudio.Length; i++)
            //     {
            //         owner.allAudio[i].audioSource.UnPause();
            //     }
            // }
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

        public void EditorInit()
        {
        }

        public void OnDrawGizmos()
        {
        }
    }
}