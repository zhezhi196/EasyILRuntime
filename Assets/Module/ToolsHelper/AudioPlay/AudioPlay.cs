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

    public class AudioPlay : IDMark<object, AudioPlay>, IProcess
    {
        #region AudioArg

        private struct AudioArg<T>
        {
            public T args;
            public bool changed;

            public AudioArg(T arg)
            {
                this.args = arg;
                this.changed = false;
            }

            public void Change(T args)
            {
                this.args = args;
                changed = true;
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

        private static Dictionary<string, AudioInfo> audioPath = new Dictionary<string, AudioInfo>();
        private static List<AudioPlay> allPlay = new List<AudioPlay>();
        private static AudioPlay _currentMusic;
        
        private static AudioListener _listener;
        private static Audio _2dAudioSource;
        private static Audio _bgm;
        private const string audioPrefabPath = "AudioCompont/AudioPrefab.prefab";

        
        private static AudioPlay PlayPrivate(string name, Audio audio, Transform parent,bool isOneShot,AudioPlayType playType, Action<AudioPlay, AudioClip, Audio> callback)
        {
            AudioPlay play = new AudioPlay {isOneShot = isOneShot, playType = playType};
            allPlay.Add(play);
            
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
            if (!audioPath.TryGetValue(name, out info))
            {
                AudioData data = DataMgr.Instance.GetSqlService<AudioData>().Where(fd => fd.name == name);
                if (data == null)
                {
                    LogError("无法找到音频文件: " + name);
                    return play;
                }

                info = new AudioInfo(name, data.path);
                audioPath.Add(name, info);
            }

            play.info = info;
            info.play = play;

            AssetLoad.PreloadAsset<AudioClip>(info.path, res =>
            {
                if (res.Result != null)
                {
                    info.clip = res.Result;
                    if (audio == null)
                    {
                        AssetLoad.LoadGameObject<Audio>(audioPrefabPath, parent, (go, a) =>
                        {
                            play.audioSource = go;
                            callback?.Invoke(play, info.clip, go);
                            play.SetArgs();
                        });
                    }
                    else
                    {
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
        
        public static AudioPlay Play(string name, Audio audio)
        {
            return PlayPrivate(name, audio, null, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip));
        }

        public static AudioPlay Play(string name, Transform parent, Vector3 pos)
        {
            return PlayPrivate(name, null, parent, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip, pos));
        }

        public static AudioPlay Play(string name, Vector3 pos)
        {
            return PlayPrivate(name, null, null, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip, pos));
        }

        public static AudioPlay Play(string name, Transform parent)
        {
            return PlayPrivate(name, null, parent, false, AudioPlayType.Audio, (play, clip, ago) => play.Play(clip));
        }

        public static AudioPlay Play(string name, AudioPlayType playType)
        {
            if (playType == AudioPlayType.Music)
            {
                var tar = PlayPrivate(name, bgm, null, false, playType, (play, clip, ago) => play.Play(clip)).SetLoop(true);
                _currentMusic = tar;
                return tar;
            }
            else
            {
                return PlayPrivate(name, globleAudioSource, null, false, playType, (play, clip, ago) => play.Play(clip));
            }
        }

        public static AudioPlay PlayOneShot(string name, Audio audio)
        {
            return PlayPrivate(name, audio, null, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip));
        }

        public static AudioPlay PlayOneShot(string name, Transform parent, Vector3 pos)
        {
            return PlayPrivate(name, null, parent, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, pos));
        }

        public static AudioPlay PlayOneShot(string name, Vector3 pos)
        {
            return PlayPrivate(name, null, null, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, pos));
        }

        public static AudioPlay PlayOneShot(string name, Transform parent)
        {
            return PlayPrivate(name, null, parent, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip, parent.position));
        }

        public static AudioPlay PlayOneShot(string name)
        {
            return PlayPrivate(name, globleAudioSource, globleAudioSource.transform, true, AudioPlayType.Audio, (play, clip, ago) => play.PlayOneShot(clip));
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
                for (int i = 0; i < allPlay.Count; i++)
                {
                    allPlay[i].Pause();
                }
                
                return;
            }
            for (int i = 0; i < allPlay.Count; i++)
            {
                var temp = allPlay[i];
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
                for (int i = 0; i < allPlay.Count; i++)
                {
                    allPlay[i].Continue();
                }
                
                return;
            }
            for (int i = 0; i < allPlay.Count; i++)
            {
                var temp = allPlay[i];
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
                for (int i = 0; i < allPlay.Count; i++)
                {
                    allPlay[i].Stop();
                }
                
                return;
            }
            for (int i = 0; i < allPlay.Count; i++)
            {
                var temp = allPlay[i];
                if (predicate.Invoke(temp))
                {
                    temp.Stop();
                }
            }
        }

        public static void GetAudioClip(string name, Action<AudioClip> callback)
        {
            AudioInfo info = default;
            if (!audioPath.TryGetValue(name, out info))
            {
                AudioData data = DataMgr.Instance.GetSqlService<AudioData>().Where(fd => fd.name == name);
                if (data == null)
                {
                    LogError("无法找到音频文件: " + name);
                }

                info = new AudioInfo(name, data.path);
                audioPath.Add(name, info);
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
            get { return _ignorePause.args; }
        }

        private void SetArgs()
        {
            if (this._isLoop.changed)
            {
                this.audioSource.audioSource.loop = this._isLoop.args;
                this._isLoop.changed = false;
            }

            if (_3dBlend.changed)
            {
                this.audioSource.audioSource.spatialBlend = _3dBlend.args;
                this._3dBlend.changed = false;
            }
            
            if (_ignorePause.changed)
            {
                if (!_ignorePause.args)
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

                _ignorePause.changed = false;
            }

            if (_isPause.changed)
            {
                if (_isPause.args)
                {
                    this.audioSource.audioSource.Pause();
                }
                else
                {
                    this.audioSource.audioSource.UnPause();
                }
                
                this._isPause.changed = false;
            }

            if (this._speed.changed)
            {
                this.audioSource.audioSource.pitch = _speed.args;
                this._speed.changed = false;
            }

            if (_3dRange.changed)
            {
                this.audioSource.audioSource.maxDistance = _3dRange.args;
                this._3dRange.changed = false;
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

        public object Current
        {
            get { return audioSource; }
        }
        
        public bool MoveNext()
        {
            if (playType == AudioPlayType.Music) return true;
            SetArgs();
            return !isComplete;
        }

        public bool isComplete
        {
            get
            {
                if (audioSource == null) return true;
                bool temp = !audioSource.audioSource.isPlaying && !_isPause.args && !_isLoop.args;
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
        
        public void Play(AudioClip clip, Vector3 pos)
        {
            audioSource.playType = playType;
            audioSource.Play(clip, false, this);
            audioSource.transform.position = pos;
        }

        public void Play(AudioClip clip)
        {
            audioSource.playType = playType;
            audioSource.Play(clip, false, this);
        }

        public void PlayOneShot(AudioClip clip, Vector3 pos)
        {
            audioSource.Play(clip, true, this);
            audioSource.transform.position = pos;
        }

        public void PlayOneShot(AudioClip clip)
        {
            audioSource.playType = AudioPlayType.Audio;
            audioSource.Play(clip, true, this);
        }
        
        public AudioPlay Pause()
        {
            _isPause.Change(true);
            return this;
        }

        public AudioPlay Continue()
        {
            _isPause.Change(false);
            return this;
        }

        public AudioPlay Stop()
        {
            allPlay.Remove(this);
            
            if (audioSource != null)
            {
                audioSource.StopPlay();
                if (playType == AudioPlayType.Music)
                {
                    Log("关闭背景音乐");
                }
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
            this._ignorePause.Change(ignorePause);
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
            this._speed.Change(speed);
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
                this._3dBlend.Change(0);
            }
            else
            {
                this._3dBlend.Change(value);
            }
            return this;
        }

        public AudioPlay SetLoop(bool loop)
        {
            if (isOneShot) return this;
            this._isLoop.Change(loop);
            return this;
        }

        public AudioPlay Set3DRange(float range)
        {
            if (!CanSet()) return this;
            this._3dRange.Change(range);
            return this;
        }

        public void Reset()
        {
            _isPause = new AudioArg<bool>(false);
            _3dBlend = new AudioArg<float>(0);
            _3dRange = new AudioArg<float>(15);
            _isLoop = new AudioArg<bool>(false);
            _speed = new AudioArg<float>(1);
            _ignorePause=new AudioArg<bool>(false);
            if (info != null)
                info.play = null;
            audioSource = null;
            this.info = null;
        }
#endregion

#endregion
    }
}