using System;
using System.Collections.Generic;
using Module;
using UnityEditor.Audio;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音效管理器
/// </summary>
public partial class AudioManager : MonoBehaviour
{
    private const string AudioPrefabPath = "AudioCompont/AudioPrefab.prefab";
    public const int PoolNum = 50;
    
    
    //正在播放的Source
    public static List<AudioSourceProxy> sources = new List<AudioSourceProxy>();
    public static List<AudioRoom> rooms = new List<AudioRoom>();
    private static AudioSourceProxy _bgmProxy;
    private static AudioListener _listener;
    public static AudioSourceProxy BGMProxy {
        get
        {
            if (_bgmProxy == null)
            {
                _bgmProxy = GameObject.Find("AudioManager/BGM").GetComponent<AudioSourceProxy>();
            }
            return _bgmProxy;
        }
    }
    public static AudioListener listener
    {
        get
        {
            if (_listener == null)
            {
                _listener = GameObject.Find("AudioManager/AudioListener").GetComponent<AudioListener>();
            }

            return _listener;
        }
    }
    
    //音效文件路径 clip缓存
    private static Dictionary<string, AudioInfo> _audioPathCache = new Dictionary<string, AudioInfo>();
    
    private static AudioDataSO _audioData; 
    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.Reset();
        
        AssetLoad.PreloadAsset<AudioDataSO>("AudioData/AudioData.asset", (ret) =>
        {
            _audioData = ret.Result;
            //加载mixer
            LoadMixer(() =>
            {
                //加载并设置对象池数量
                var pool = AssetLoad.GetPool(AudioPrefabPath,null);
                pool.SetDefaultCountCallBack(PoolNum, () =>
                {
                    process.SetComplete();
                });
            });
        });
        return process;
    }
    
    
    
    private static void GetAudioClip(string name, Action<AudioClip, AudioDataUnit, AudioDataUnitPath> callback , bool fromSequenceBegin = false)
    {
        AudioDataUnitPath path = _audioData.GetAudioPath(name, out AudioDataUnit unit,fromSequenceBegin);

        if (path == null)
        {
            GameDebug.LogError("无法找到音频配置，检查Bundles/AudioData配置: " + name);
            return;
        }
        
        if (!_audioPathCache.TryGetValue(path.path, out AudioInfo info))
        {
            if (string.IsNullOrEmpty(path.path))
            {
                GameDebug.LogError("无法找到音频文件: " + name);
                return;
            }
            info = new AudioInfo(path.path);
            _audioPathCache.Add(path.path, info);
        }
        AssetLoad.PreloadAsset<AudioClip>(info.path, res =>
        {
            if (res.Result != null)
            {
                info.clip = res.Result;
                callback?.Invoke(res.Result , unit, path);
            }
        });
    }

    
    #region Play方法

    public static AudioSourceProxy PlayMusic(string name)
    {
        GetAudioClip(name, (clip,data,path) =>
        {
            BGMProxy.Play(clip,AudioFlag.UI|AudioFlag.Loop);
        },true);
        
        //循环播放
        BGMProxy.OnStepComplete(() =>
        {
            BGMProxy.Stop();
            GetAudioClip(name, (clip,data,path) =>
            {
                BGMProxy.Play(clip,AudioFlag.UI|AudioFlag.Loop);
            },false);
        });

        return BGMProxy;
    }

    public static AudioSourceProxy PlayUI(string name)
    {
        return PlayPrivate(name,null,null, (proxy,clip) =>
        {
            proxy.Play(clip, AudioFlag.UI);
        });
    }
    
    /// <summary>
    /// 根据一个audioSource播放
    /// </summary>
    public static AudioSourceProxy Play(string name, AudioSource modelSource , AudioFlag flag = AudioFlag.None)
    {
        var p = PlayPrivate(name,null, modelSource.transform, (proxy, clip) =>
        {
            proxy.Play(clip, flag);
        });
        p.source.CopyFrom(modelSource);
        return p;
    }


    public static AudioSourceProxy Play(string name, Vector3 pos , AudioFlag flag = AudioFlag.None)
    {
        return PlayPrivate(name,null,null, (proxy,clip) =>
        {
            proxy.transform.position = pos;
            proxy.Play(clip, flag);
        });
    }
    
    public static AudioSourceProxy Play(string name, Transform playParent , AudioFlag flag = AudioFlag.None)
    {
        return PlayPrivate(name,null, playParent, (proxy,clip) =>
        {
            proxy.Play(clip, flag);
        });
    }
    

    public static AudioSourceProxy Play(string name, string group, Transform playParent , AudioFlag flag = AudioFlag.None)
    {
        return PlayPrivate(name, group, playParent, (proxy,clip) =>
        {
            proxy.Play(clip, flag);
        });
    }
    
    public static AudioSourceProxy Play(string name, string group, Vector3 pos , AudioFlag flag = AudioFlag.None)
    {
        return PlayPrivate(name,group,null, (proxy,clip) =>
        {
            proxy.transform.position = pos;
            proxy.Play(clip, flag);
        });
    }

    private static AudioSourceProxy PlayPrivate(string name, string group, Transform playParent, Action<AudioSourceProxy, AudioClip> callback)
    {
        //为啥这里要修改对象池的加载方式吧，因为有时候对象池里确定有
        var pool = AssetLoad.GetPool(AudioPrefabPath,playParent);
        if (pool.GetActivePoolCount() <= 0)
        {
            GameObject go = new GameObject("AudioPrefab");
            var p = go.AddComponent<AudioSourceProxy>();
            pool.AddNewToPoolFromOutside(go);
        }
        
        var proxy = pool.GetObjectSync<AudioSourceProxy>();
        if (playParent != null)
        {
            proxy.transform.SetParentZero(playParent);
        }

        if (!string.IsNullOrEmpty(group))
        {
            TryAddGroup(proxy, group);
        }
        GetAudioClip(name , (clip, data, path) =>
        {
            //这里默认赋值mixer
            var mixer = GetMixer(data.mixerName);
            if (mixer) proxy.SetMixerGroup(mixer);
            proxy.SetLowPassRange(path.lowpassRange);
            callback?.Invoke(proxy, clip); //异步加载clip，但是这里做了缓存，可能不是异步的
        });

        return proxy;
    }


    public static void PauseAll(bool includeBgm = true)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].Pause();
        }

        if (includeBgm)
        {
            BGMProxy.Pause();
        }
    }

    public static void ResumeAll()
    {
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].Resume();
        }
    }

    public static void StopMusic()
    {
        BGMProxy.Stop();
    }

    #endregion
    
    #region 分组相关
    
    private static Dictionary<string,List<AudioSourceProxy>> _group2Sources = new Dictionary<string, List<AudioSourceProxy>>();
    /// <summary>
    /// 尝试分组
    /// </summary>
    public static void TryAddGroup(AudioSourceProxy proxy , string group)
    {
        if (string.IsNullOrEmpty(group))
        {
            return;
        }
        
        if (!_group2Sources.TryGetValue(group , out List<AudioSourceProxy> proxys))
        {
            proxys = new List<AudioSourceProxy>();
            proxys.Add(proxy);
            
            _group2Sources.Add(group, proxys);
        }
        else
        {
            //添加的时候清除旧数据
            ClearGroupData(proxys);
            if (!proxys.Contains(proxy))
            {
                proxys.Add(proxy);
            }
        }
        
                    

        
    }

    public static void StopGroup(string group)
    {
        if (string.IsNullOrEmpty(group))
        {
            return;
        }
        
        if (_group2Sources.TryGetValue(group , out List<AudioSourceProxy> proxys))
        {
            //清除旧数据
            ClearGroupData(proxys);

            for (int i = 0; i < proxys.Count; i++)
            {
                proxys[i].Stop(true);
            }
        }
    }

    private static void ClearGroupData(List<AudioSourceProxy> proxys)
    {
        for (int i = 0; i < proxys.Count; i++)
        {
            if (proxys[i].status == AudioSourceStatus.Pool)
            {
                proxys.RemoveAt(i);
                i--;
            }
        }
    }
    

    #endregion
    
    #region 音量控制
    
    public static float MusicVolume { get; set; }
    public static float AudioVolume { get; set; }

    public static void MuteAudio(bool mute)
    {
        Mixer.SetFloat(SoundVolumeName,VolumeLerp(mute ? 0:1));
    }

    public static void MuteMusic(bool mute)
    {
        Mixer.SetFloat(MusicVolumeName,VolumeLerp(mute ? 0:1));
    }

    public static void SetAudioVolume(float volume)
    {
        Mixer.SetFloat(SoundVolumeName,VolumeLerp(Mathf.Clamp01(volume)));
    }
    public static void SetMusicVolume(float volume)
    {
        Mixer.SetFloat(MusicVolumeName,VolumeLerp(Mathf.Clamp01(volume)));
    }

    private static float VolumeLerp(float input01)
    {
        return Mathf.Lerp(-80, 0, input01);
    }
    
    #endregion
    
    #region 音效注册
    public static void RegisterAudio(AudioSourceProxy source)
    {
        if (!sources.Contains(source))
        {
            sources.Add(source);    
        }
    }
    public static void UnRegisterAudio(AudioSourceProxy source)
    {
        sources.Remove(source);
    }
    
    
    public static void RegisterRoom(AudioRoom room)
    {
        if (!rooms.Contains(room))
        {
            rooms.Add(room);    
        }
        
    }
    public static void UnRegisterRoom(AudioRoom room)
    {
        rooms.Remove(room);
    }
    #endregion



    #region Mixer
    
    /// <summary>
    /// Mixer组件
    /// </summary>
    public static Dictionary<string,AudioMixerGroup> mixerGroups = new Dictionary<string,AudioMixerGroup>();
    public static AudioMixer Mixer;
    public const string MixerPath = "AudioData/Mixer/Mixer.mixer";
    public const string MusicVolumeName = "MusicVolume";
    public const string SoundVolumeName = "SoundVolume";
    
    public static void LoadMixer(Action cb)
    {
        AssetLoad.PreloadAsset<AudioMixer>(MixerPath, (m) =>
        {
            Mixer = m.Result;
            var groups = Mixer.FindMatchingGroups("");
            for (int i = 0; i < groups.Length; i++)
            {
                mixerGroups.Add(groups[i].name , groups[i]);
            }
            cb?.Invoke();
        });
    }

    public static AudioMixerGroup GetMixer(string name)
    {
        if (mixerGroups.TryGetValue(name, out AudioMixerGroup mixerGroup))
        {
            return mixerGroup;
        }
        return null;
    }
    #endregion


    #region Mono

    private void Update()
    {
        //更新所有的source和room
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].UpdateLogic();
        }
        
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].UpdateLogic();
        }
    }

    #endregion
}