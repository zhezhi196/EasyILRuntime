using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Module
{
    public enum AudioPlayType
    {
        Audio,
        Music
    }

    public class AudioPlay : ISetID<object, AudioPlay>, IProcess
    {
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
        public const string audioPrefabPath = "AudioCompont/AudioPrefab.prefab";

        public static float MusicVolume;
        public static float AudioVolume;
        public static bool Mute;
        public static string currentMusic;
        
        public static event Action<float> onMusicVolumeChanged;
        public static event Action<float> onAudioVolumeChanged;
        public static event Action<bool> onMuteChanged;

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

        #region Dic

        private static Dictionary<object, List<AudioPlay>> audioPlays = new Dictionary<object, List<AudioPlay>>();
        private static Dictionary<string, AudioInfo> audioPath = new Dictionary<string, AudioInfo>();

        #endregion

        #region 2dPoint

        private static AudioListener _listener;

        public static AudioListener listener
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

        private static Audio _2dAudioSource;

        public static Audio globleAudioSource
        {
            get
            {
                if (_2dAudioSource == null)
                {
                    _2dAudioSource = listener.GetComponent<Audio>();
                    _2dAudioSource.autoCollection = false;
                }

                return _2dAudioSource;
            }
        }

        #endregion

        #region ID Controller

        public static void StopByID(object id)
        {
            List<AudioPlay> list = null;
            if (audioPlays.TryGetValue(id, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Stop();
                }
            }
        }

        public static void PauseByID(object id)
        {
            List<AudioPlay> list = null;
            if (audioPlays.TryGetValue(id, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Pause();
                }
            }
        }

        public static void ContinueByID(object id)
        {
            List<AudioPlay> list = null;
            if (audioPlays.TryGetValue(id, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Continue();
                }
            }
        }

        public static void UnloadByID(object id)
        {
            List<AudioPlay> list = null;
            if (audioPlays.TryGetValue(id, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].UnLoad();
                }
            }
        }

        #endregion

        #region Globle

        public static void Unload(string name)
        {
            AudioInfo info = null;
            if (audioPath.TryGetValue(name, out info))
            {
                info.play.UnLoad();
            }
        }

        #endregion

        #region Play

        private static AudioPlay PlayPrivate(string name, Audio audio, Transform parent,bool isOneShot,AudioPlayType playType, Action<AudioPlay, AudioClip, Audio> callback)
        {
            AudioPlay play = new AudioPlay();
            if (audio != null && !audio.gameObject.activeInHierarchy) return play;
            play.isOneShot = isOneShot;
            play.playType = playType;
            //GameDebug.Log("尝试播放音效: "+name);
            AudioInfo info = default;
            if (!audioPath.TryGetValue(name, out info))
            {
                AudioData data = DataMgr.Instance.GetSqlService<AudioData>().Where(fd => fd.name == name);
                if (data == null)
                {
                    GameDebug.LogError("无法找到音频文件: "+name);
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

        public static AudioPlay Play(string name, AudioPlayType playType, Audio audio)
        {
            return PlayPrivate(name, audio, null, false, playType, (play, clip, ago) => play.Play(clip));
        }

        public static AudioPlay Play(string name, AudioPlayType playType, Transform parent, Vector3 pos)
        {
            return PlayPrivate(name, null, parent, false, playType, (play, clip, ago) => play.Play(clip, pos));
        }

        public static AudioPlay Play(string name, AudioPlayType playType, Vector3 pos)
        {
            return PlayPrivate(name, null, null, false, playType, (play, clip, ago) => play.Play(clip, pos));
        }

        public static AudioPlay Play(string name, AudioPlayType playType, Transform parent)
        {
            return PlayPrivate(name, null, parent, false, playType, (play, clip, ago) => play.Play(clip));
        }

        public static AudioPlay Play(string name, AudioPlayType playType)
        {
            return PlayPrivate(name, globleAudioSource, null, false, playType, (play, clip, ago) => play.Play(clip));
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

        #endregion

        #region 属性
        private bool isOneShot;

        private AudioArg<bool> isPause = new AudioArg<bool>(false);
        private AudioArg<float> _3dBlend = new AudioArg<float>(0);
        private AudioArg<float> _3dRange = new AudioArg<float>(15);
        private AudioArg<bool> isLoop = new AudioArg<bool>(false);
        private AudioArg<float> speed = new AudioArg<float>(1);
        
        public object ID { get; set; }
        public Func<bool> monitor { get; set; }
        public Audio audioSource { get; set; }
        public AudioInfo info { get; set; }
        public AudioPlayType playType { get; set; }
        public Action callback;

        public bool MoveNext()
        {
            return !isComplete;
        }

        public object Current
        {
            get { return audioSource; }
        }

        public bool isComplete
        {
            get
            {
                if (audioSource == null) return true;
                bool temp = !audioSource.audioSource.isPlaying && !isPause.args && !isLoop.args;
                if (monitor == null)
                {
                    return temp;
                }
                else
                {
                    return temp || monitor.Invoke();
                }
            }
        }



        #endregion

        #region 函数



        public void Play(AudioClip clip, Vector3 pos)
        {
            if (playType == AudioPlayType.Music) currentMusic = info.name;
            audioSource.playType = playType;
            audioSource.Play(clip, false, this);
            audioSource.transform.position = pos;
        }

        public void Play(AudioClip clip)
        {
            if (playType == AudioPlayType.Music) currentMusic = info.name;
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
            isPause.Change(true);
            return this;
        }

        public AudioPlay Continue()
        {
            isPause.Change(false);
            return this;
        }

        #region Set

        public AudioPlay SetOnLoad(Action callback)
        {
            this.callback = callback;
            return this;
        }

        public AudioPlay SetID(object ID)
        {
            if (audioSource == globleAudioSource) return this;
            if (ID == null) return this;
            List<AudioPlay> temp = null;
            this.ID = ID;
            if (!audioPlays.TryGetValue(ID, out temp))
            {
                temp = new List<AudioPlay>();
                audioPlays.Add(ID, temp);
            }

            temp.Add(this);
            return this;
        }

        public AudioPlay SetSpeed(float speed)
        {
            if (audioSource == globleAudioSource) return this;
            this.speed.Change(speed);
            return this;
        }

        public void SetMonitor(Func<bool> monitor)
        {
            if (audioSource == globleAudioSource) return;
            this.monitor = monitor;
        }

        public AudioPlay Set3D(float value = 1)
        {
            if (audioSource == globleAudioSource) return this;
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
            this.isLoop.Change(loop);
            return this;
        }

        public AudioPlay Set3DRange(float range)
        {
            if (audioSource == globleAudioSource) return this;
            this._3dRange.Change(range);
            return this;
        }

        #endregion

        public AudioPlay Stop()
        {
            if (ID != null)
            {
                if (audioPlays.ContainsKey(ID))
                {
                    audioPlays[ID].Remove(this);
                    if (audioPlays[ID].Count == 0)
                    {
                        audioPlays.Remove(ID);
                    }
                }
            }

            if (audioSource != null)
            {
                audioSource.ReturnToPool();
            }
            Reset();
            return this;
        }

        public void UnLoad()
        {
            AssetLoad.Release(info.clip);
            Stop();
        }

        public void Reset()
        {
            isPause = new AudioArg<bool>(false);
            _3dBlend = new AudioArg<float>(0);
            _3dRange = new AudioArg<float>(15);
            isLoop = new AudioArg<bool>(false);
            speed = new AudioArg<float>(1);

            if (info != null)
                info.play = null;
            audioSource = null;
            this.info = null;
        }

        private void SetArgs()
        {
            if (this.isLoop.changed)
            {
                this.audioSource.audioSource.loop = this.isLoop.args;
            }

            if (_3dBlend.changed)
            {
                this.audioSource.audioSource.spatialBlend = _3dBlend.args;
            }

            if (isPause.changed)
            {
                this.audioSource.audioSource.Pause();
            }

            if (this.speed.changed)
            {
                this.audioSource.audioSource.pitch = speed.args;
            }

            if (_3dRange.changed)
            {
                this.audioSource.audioSource.maxDistance = _3dRange.args;
            }

            this.callback?.Invoke();
        }

        #endregion
    }
}