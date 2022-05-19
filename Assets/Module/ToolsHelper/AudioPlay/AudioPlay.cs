using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Module
{
    public enum AudioPlayType
    {
        /// <summary>
        /// 音效
        /// </summary>
        Audio,
        /// <summary>
        /// 背景音乐
        /// </summary>
        Music,
    }

    public class AudioPlay : Identify, IProcess
    {
        public static bool playAudio = true;
        [Flags]
        public enum AudioFlag
        {
            RestartOnSame = 1,
            CompleteNotNull=2,
        }
        
        #region AudioArg

        private struct AudioArg<T>
        {
            public T value;
            public bool isChanged;

            public AudioArg(T value)
            {
                this.value = value;
                this.isChanged = false;
            }

            public void ChangeValue(T args)
            {
                if (!this.value.Equals(args))
                {
                    this.value = args;
                    isChanged = true;
                }
            }
        }

        #endregion

        #region Log

        public static bool isLog = false;

        private static void Log(object obj)
        {
            if (isLog)
            {
                GameDebug.Log(obj);
            }
        }

        private static void LogFormat(string obj, params object[] arg)
        {
            if (isLog)
            {
                GameDebug.LogFormat(obj, arg);
            }
        }

        private static void LogError(object obj)
        {
            if (isLog)
            {
                GameDebug.LogError(obj);
            }
        }

        #endregion
        
        #region Static Private

        private static Dictionary<string, AudioInfo> _audioPath = new Dictionary<string, AudioInfo>();
        public static List<AudioPlay> _allPlay = new List<AudioPlay>();
        private static AudioPlay _currentMusic;
        private static List<string> _musicQueue = new List<string>();
        private static AudioListener _listener;
        private static Audio _2dAudioSource;
        private static Audio _bgm;
        private const string _audioPrefabPath = "AudioCompont/AudioPrefab.prefab";
        
        public static void SetAudioSpeed(Predicate<AudioPlay> action, float value)
        {
            for (int i = 0; i < _allPlay.Count; i++)
            {
                if (action.Invoke(_allPlay[i]))
                {
                    _allPlay[i].SetSpeed(value);
                }
            }
        }

        public static void RefreshTimeScaleSpeed(Predicate<AudioPlay> action)
        {
            for (int i = 0; i < _allPlay.Count; i++)
            {
                if (action.Invoke(_allPlay[i]))
                {
                    _allPlay[i].SetSpeed(1);
                }
            }
        }
        //停止BGM
        public static void StopPlayBGM()
        {
            StopAudio(fd => fd.audioSource == bgm);
        }
        //除BGM全部停止
        public static void StopAudio()
        {
            StopAudio(fd => fd.audioSource != bgm);
        }

        public static void MuteAudio(Predicate<AudioPlay> predicate,bool mute)
        {
            if (predicate == null)
            {
                for (int i = 0; i < _allPlay.Count; i++)
                {
                    _allPlay[i].MuteAudioPlay(mute);
                }

                return;
            }

            for (int i = 0; i < _allPlay.Count; i++)
            {
                var temp = _allPlay[i];
                if (temp.audioSource != null)
                {
                    if (predicate.Invoke(temp))
                    {
                        temp.MuteAudioPlay(mute);
                    }
                }
            }
        }

        private static AudioPlay PlayPrivate(string name, Audio audio, Transform parent, bool isOneShot, AudioPlayType playType, Action<AudioPlay, AudioClip, Audio> callback)
        {
            return new AudioPlay();
            AudioPlay play = new AudioPlay {isOneShot = isOneShot, playType = playType};
            if (!playAudio)
            {
                GameDebug.Log($"播放音效{name}");
                return play;
            }

            _allPlay.Add(play);
            
            if (audio != null && !audio.gameObject.activeInHierarchy)
            {
                LogFormat("{0}这个Audio没有激活", audio.gameObject.name);
                return play;
            }
            if (playType == AudioPlayType.Music)
            {
                LogFormat("播放背景音乐: {0}", name);
            }
            else
            {
                LogFormat("尝试播放音效: {0}", name);
            }

            AudioInfo info = default;
            if (!_audioPath.TryGetValue(name, out info))
            {
                AudioData data = SqlData.GetSqlService<AudioData>().Where(fd => fd.name == name);
                if (data == null)
                {
                    LogError("无法找到音频文件: " + name);
                    return play;
                }

                info = new AudioInfo(name, data.path);
                _audioPath.Add(name, info);
            }

            play.info = info;
            AssetLoad.PreloadAsset<AudioClip>(info.path, res =>
            {
                if (res.Result != null)
                {
                    info.clip = res.Result;
                    if (audio == null)
                    {
                        AssetLoad.LoadGameObject<Audio>(_audioPrefabPath, parent, (go, a) =>
                        {
                            play.RecordOldArgs(go.audioSource);
                            play.audioSource = go;
                            callback?.Invoke(play, info.clip, go);
                            play.SetArgs();
                        });
                    }
                    else
                    {

                        
                        play.RecordOldArgs(audio.audioSource);
                        play.audioSource = audio;
                        callback?.Invoke(play, info.clip, audio);
                        play.SetArgs();
                    }
                }
            });


            return play;
        }
        
        #endregion

        #region 静态属性

        public static float MusicVolume { get; set; }
        public static float AudioVolume { get; set; }
        public static bool Mute { get; set; }

        public static AudioPlay currentMusic
        {
            get { return _currentMusic; }
        }

        #endregion
        
        #region Listener


        public static AudioListener defaultListener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = GameObject.Find("GamePlay/Audio").GetComponent<AudioListener>();
                }

                return _listener;
            }
        }

        public static Audio globleAudioSource
        {
            get
            {
                if (_2dAudioSource == null)
                {
                    _2dAudioSource = defaultListener.GetComponent<Audio>();
                    _2dAudioSource.autoCollection = false;
                }

                return _2dAudioSource;
            }
        }

        public static Audio bgm
        {
            get
            {
                if (_bgm == null)
                {
                    _bgm = defaultListener.transform.GetChild(0).GetComponent<Audio>();
                    _bgm.autoCollection = false;
                }
                
                return _bgm;
            }
        }

        #endregion

        #region Event

        public static event Action<float> onMusicVolumeChanged;
        public static event Action<float> onAudioVolumeChanged;
        public static event Action<bool> onMuteChanged;
        
        #endregion

        #region Public Method

        public static AudioPlay PlayDefault(Audio audio, AudioFlag flag = 0)
        {
            AudioPlay play = new AudioPlay {isOneShot = false, playType = AudioPlayType.Audio};
            if (!playAudio)
            {
                GameDebug.Log($"播放音效{audio.audioSource.name}");
                return play;
            }
            _allPlay.Add(play);
            
            if (audio != null && !audio.gameObject.activeInHierarchy)
            {
                LogFormat("{0}这个Audio没有激活", audio.gameObject.name);
                return play;
            }
            LogFormat("尝试播放音效: {0}", audio.audioSource.clip);
            play.RecordOldArgs(audio.audioSource);

            AudioInfo info = new AudioInfo(audio.audioSource.clip);
            play.info = info;
            play.audioSource = audio;
            play.Play(audio.audioSource.clip, flag | AudioFlag.CompleteNotNull);
            play.SetArgs();
            return play;
        }
        public static AudioPlay Play(string name, Audio audio, AudioFlag flag = 0)
        {
            return PlayPrivate(name, audio, null, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip,flag));
        }

        public static AudioPlay Play(string name, Transform parent, Vector3 pos, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, parent, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip, pos,flag));
        }

        public static AudioPlay Play(string name, Vector3 pos, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, null, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip, pos,flag));
        }

        public static AudioPlay Play(string name, Transform parent, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, parent, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip,flag));
        }

        public static AudioPlay Play(string name, AudioFlag flag = 0)
        {
            return PlayPrivate(name, globleAudioSource, null, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip,flag));
        }

        public static AudioPlay PlayBackGroundMusic(string name, AudioFlag flag = 0,bool clearStack = false)
        {
            var tar = PlayPrivate(name, bgm, null, false, AudioPlayType.Music, (play, clip, ago) => play.Play(clip,flag)).SetLoop(true).SetIgnorePause(true);
            _currentMusic = tar;
            if (!clearStack)
            {
                _musicQueue.Add(name);
            }
            else
            {
                if (_musicQueue.Contains(name))
                {
                    for (int i = _musicQueue.Count - 1; i >= 0; i--)
                    {
                        if (_musicQueue[i] == name)
                        {
                            return tar;
                        }
                        else
                        {
                            _musicQueue.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    _musicQueue.Add(name);
                }
            }
            return tar;
        }

        public static void ClearMusicStack()
        {
            _musicQueue.Clear();
        }

        public static AudioPlay PlayBackMusic()
        {
            if (_musicQueue.Count < 2) return null;
            return PlayBackGroundMusic(_musicQueue[_musicQueue.Count - 2], 0, true);
        }

        public static AudioPlay PlayOneShot(string name, Audio audio, AudioFlag flag = 0)
        {
            return PlayPrivate(name, audio, null, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip,flag));
        }

        public static AudioPlay PlayOneShot(string name, Transform parent, Vector3 pos, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, parent, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, pos,flag));
        }

        public static AudioPlay PlayOneShot(string name, Vector3 pos, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, null, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, pos,flag));
        }

        public static AudioPlay PlayOneShot(string name, Transform parent, AudioFlag flag = 0)
        {
            return PlayPrivate(name, null, parent, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, parent.position,flag));
        }

        public static AudioPlay PlayOneShot(string name, AudioFlag flag = 0)
        {
            return PlayPrivate(name, globleAudioSource, globleAudioSource.transform, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip,flag));
        }

        public static void SetMusicVolume(float value)
        {
            MusicVolume = value;
            onMusicVolumeChanged?.Invoke(value);
        }

        public static void SetAudioVolume(float value)
        {
            AudioVolume = value;
            onAudioVolumeChanged?.Invoke(value);
        }

        public static void SetMute(bool mute)
        {
            Mute = mute;
            onMuteChanged?.Invoke(mute);
        }

        public static void PauseAudio(Predicate<AudioPlay> predicate)
        {
            if (predicate == null)
            {
                for (int i = 0; i < _allPlay.Count; i++)
                {
                    _allPlay[i].Pause();
                }
                
                return;
            }
            for (int i = 0; i < _allPlay.Count; i++)
            {
                var temp = _allPlay[i];
                if (predicate.Invoke(temp))
                {
                    temp.Pause();
                }
            }
        }

        public static void ContinueAudio(Predicate<AudioPlay> predicate)
        {
            if (predicate == null)
            {
                for (int i = 0; i < _allPlay.Count; i++)
                {
                    _allPlay[i].Continue();
                }
                
                return;
            }
            for (int i = 0; i < _allPlay.Count; i++)
            {
                var temp = _allPlay[i];
                if (predicate.Invoke(temp))
                {
                    temp.Continue();
                }
            }
        }

        public static void StopAudio(Predicate<AudioPlay> predicate)
        {
            if (predicate == null)
            {
                for (int i = 0; i < _allPlay.Count; i++)
                {
                    _allPlay[i].Stop();
                }
                
                return;
            }
            for (int i = 0; i < _allPlay.Count; i++)
            {
                var temp = _allPlay[i];
                if (predicate.Invoke(temp))
                {
                    temp.Stop();
                }
            }
        }

        public static void GetAudioClip(string name, Action<AudioClip> callback)
        {
            AudioInfo info = default;
            if (!_audioPath.TryGetValue(name, out info))
            {
                AudioData data = SqlData.GetSqlService<AudioData>().Where(fd => fd.name == name);
                if (data == null)
                {
                    LogError("无法找到音频文件: " + name);
                }

                info = new AudioInfo(name, data.path);
                _audioPath.Add(name, info);
            }
            
            AssetLoad.PreloadAsset<AudioClip>(info.path, res =>
            {
                if (res.Result != null)
                {
                    info.clip = res.Result;
                    callback?.Invoke(res.Result);
                }
            });
        }

        #endregion

        #region Private

        private bool isOneShot;
        private AudioArg<bool> _isPause = new AudioArg<bool>(false);
        private AudioArg<float> _3dBlend = new AudioArg<float>(0);
        private AudioArg<float> _3dRange = new AudioArg<float>(15);
        private AudioArg<bool> _isLoop = new AudioArg<bool>(false);
        private AudioArg<float> _speed = new AudioArg<float>(1);
        private AudioArg<bool> _ignorePause = new AudioArg<bool>(false);

        public bool ignorePause
        {
            get { return _ignorePause.value; }
        }

        private void SetArgs()
        {
            if (audioSource == null) return;
            if (this._isLoop.isChanged)
            {
                this.audioSource.audioSource.loop = this._isLoop.value;
                this._isLoop.isChanged = false;
            }

            if (_3dBlend.isChanged)
            {
                this.audioSource.audioSource.spatialBlend = _3dBlend.value;
                this._3dBlend.isChanged = false;
            }

            if (_ignorePause.isChanged)
            {
                if (!_ignorePause.value)
                {
                    if (TimeHelper.isPause)
                    {
                        Pause();
                    }
                    else
                    {
                        Continue();
                    }
                }

                _ignorePause.isChanged = false;
            }

            if (_isPause.isChanged)
            {
                if (_isPause.value)
                {
                    this.audioSource.audioSource.Pause();
                }
                else
                {
                    this.audioSource.audioSource.UnPause();
                }

                this._isPause.isChanged = false;
            }

            if (this._speed.isChanged)
            {
                this.audioSource.audioSource.pitch = _speed.value;
                this._speed.isChanged = false;
            }


            if (_3dRange.isChanged)
            {
                this.audioSource.audioSource.maxDistance = _3dRange.value;
                this._3dRange.isChanged = false;
            }
        }

        private bool CanSet()
        {
            return audioSource != globleAudioSource && audioSource != bgm;
        }

        #endregion

        #region 属性

        public object ID { get; set; }
        public string tag { get; set; }
        public Func<bool> listener { get; set; }
        public Audio audioSource { get; set; }
        public AudioInfo info { get; set; }
        public AudioPlayType playType { get; set; }

        public event Action<bool> onStop;

        public object Current
        {
            get { return audioSource; }
        }

        public bool MoveNext()
        {
            if (playType == AudioPlayType.Music) return true;
            return !isComplete;
        }

        public bool isComplete
        {
            get
            {
                if (audioSource.gameObject.IsNullOrDestroyed() || audioSource.audioSource.gameObject.IsNullOrDestroyed()) return true;
                bool temp = !audioSource.audioSource.isPlaying && !_isPause.value && !_isLoop.value;
                if (listener == null)
                {
                    return temp;
                }
                else
                {
                    return temp || listener.Invoke();
                }
            }
        }

        #endregion

        #region 函数

        private bool oldLoop;
        private float old3dBlend;
        private float old3dRange;
        private float oldspeed;

        private void RecordOldArgs(AudioSource audio)
        {
            _3dBlend = new AudioArg<float>(audio.spatialBlend);
            _3dRange = new AudioArg<float>(audio.maxDistance);
            _isLoop = new AudioArg<bool>(audio.loop);
            _speed = new AudioArg<float>(audio.pitch);
            
            this.oldLoop = audio.loop;
            this.old3dBlend = audio.spatialBlend;
            this.old3dRange = audio.maxDistance;
            this.oldspeed = audio.pitch;
        }

        public void Play(AudioClip clip, Vector3 pos, AudioFlag flag)
        {
            audioSource.playType = playType;
            audioSource.Play(clip, this, false, flag);
            audioSource.transform.position = pos;
        }

        public void Play(AudioClip clip, AudioFlag flag)
        {
            audioSource.playType = playType;
            audioSource.Play(clip, this, false, flag);
        }

        public void PlayOneShot(AudioClip clip, Vector3 pos, AudioFlag flag)
        {
            audioSource.Play(clip, this, true, flag);
            audioSource.transform.position = pos;
        }

        public void PlayOneShot(AudioClip clip, AudioFlag flag)
        {
            audioSource.playType = AudioPlayType.Audio;
            audioSource.Play(clip, this, true, flag);
        }

        public AudioPlay Pause()
        {
            _isPause.ChangeValue(true);
            SetArgs();
            return this;
        }

        public AudioPlay Continue()
        {
            _isPause.ChangeValue(false);
            SetArgs();
            return this;
        }

        public AudioPlay Stop(bool isComplete = false)
        {
            _allPlay.Remove(this);

            if (audioSource != null)
            {
                audioSource.StopPlayAndCoroutine();
                onStop?.Invoke(isComplete);
                if (playType == AudioPlayType.Music)
                {
                    Log("关闭背景音乐");
                }
                audioSource.audioSource.loop = oldLoop;
                audioSource.audioSource.spatialBlend = old3dBlend;
                audioSource.audioSource.maxDistance = old3dRange;
                audioSource.audioSource.pitch = oldspeed;
                
                if (audioSource.autoCollection)
                {
                    audioSource.ReturnToPool();
                    Reset();
                }
            }

            return this;
        }

        #region Set

        public AudioPlay SetTag(string tag)
        {
            if (tag == null) return this;
            this.tag = tag;
            return this;
        }

        public AudioPlay SetIgnorePause(bool ignorePause)
        {
            this._ignorePause.ChangeValue(ignorePause);
            SetArgs();
            return this;
        }

        public AudioPlay SetID(object ID, string tag1 = null)
        {
            if (ID == null) return this;
            this.ID = ID;
            SetTag(tag1);
            return this;
        }

        public AudioPlay SetSpeed(float speed)
        {
            if (!CanSet()) return this;
            this._speed.ChangeValue(ignorePause ? speed : speed * TimeHelper.timeScale);
            SetArgs();
            return this;
        }

        public void SetListener(Func<bool> listen)
        {
            this.listener = listen;
        }

        public AudioPlay Set3D(float value = 1)
        {
            if (!CanSet()) return this;
            if (playType == AudioPlayType.Music)
            {
                this._3dBlend.ChangeValue(0);
            }
            else
            {
                this._3dBlend.ChangeValue(value);
            }
            SetArgs();
            return this;
        }

        public AudioPlay SetLoop(bool loop)
        {
            if (isOneShot) return this;
            this._isLoop.ChangeValue(loop);
            SetArgs();
            return this;
        }

        public AudioPlay Set3DRange(float range)
        {
            if (!CanSet()) return this;
            this._3dRange.ChangeValue(range);
            SetArgs();
            return this;
        }

        public bool IsCommonAudio()
        {
            return audioSource != globleAudioSource && audioSource != bgm;
        }

        public void Reset()
        {

            audioSource = null;
            this.info = null;
        }

        #endregion

        #endregion


        public void MuteAudioPlay(bool mute)
        {
            this.audioSource.audioSource.mute = mute || Mute;
        }

        public override string ToString()
        {
            return audioSource.gameObject.name + "_" + audioSource.audioSource.clip;
        }


    }
}