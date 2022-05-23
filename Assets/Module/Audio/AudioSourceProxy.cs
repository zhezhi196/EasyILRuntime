using System;
using System.Collections;
using ICSharpCode.NRefactory.Ast;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioSourceStatus
{
    None,    //没有播放
    Loading, //正在加载clip
    Playing,    //正在播放，loop会一直处于playing
    Pause,    //暂停中
    Pool,     //在对象池中（规定只要播放完毕应当立刻返回对象池）
}

[Flags]
public enum AudioFlag
{
    None = 0,
    Loop = 1,
    UI = 2, //2d音效3
    // RestartOnSame = 1,
}

/// <summary>
/// Audio代理，用于控制AudioSource
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
// [RequireComponent(typeof(AudioReverbFilter))]
public class AudioSourceProxy : MonoBehaviour , IPoolObject
{
    [ReadOnly]
    public Vector2Int LowPassRange; 
    [ReadOnly]
    public AudioRoom belongRoom;
    [ReadOnly]
    public AudioSource source;
    [ReadOnly]
    public AudioLowPassFilter lowPassFilter;
    // [ReadOnly]
    // public AudioReverbFilter reverbFilter;
    [ReadOnly]
    public AudioSourceStatus status;
    
    //各种回调
    public Action _onPause;
    public Action _onResume;
    public Action _onPlay;
    public Action _onComplete;
    public Action _onStepComplete;
    
    //各种参数
    private float _playSpeed = 1;
    private bool _ignorePause = false; //是否忽略暂停
    private float _maxVolume = 1; //原始音量，相当于这个source的最大音量
    
    private float _audioLength = 0;
    private float _curPlayLength = 0;
    
    public void SetStatus(AudioSourceStatus s)
    {
        status = s;
    }

    

    public void Play(AudioClip clip, AudioFlag flag)
    {
        source.clip = clip;
        source.spatialBlend = flag.HasFlag(AudioFlag.UI) ? 0 : 1;
        source.loop = flag.HasFlag(AudioFlag.Loop);
        _audioLength = clip.length;
        _curPlayLength = 0;
        _ignorePause = flag.HasFlag(AudioFlag.UI); //UI忽略暂停
        source.Play();
        source.volume = _maxVolume;

        SetStatus(AudioSourceStatus.Playing);
        _onPlay?.Invoke();
    }

    public void Pause()
    {
        if (_ignorePause)//忽略暂停
        {
            return;
        }

        PausePrivate();
    }

    public void PauseForce()
    {
        PausePrivate();
    }

    private void PausePrivate()
    {
        source.Pause();
        SetStatus(AudioSourceStatus.Pause);
        _onPause?.Invoke();
    }


    public void Resume()
    {
        if (_ignorePause || source.isPlaying)//忽略暂停
        {
            return;
        }
        source.Play();
        SetStatus(AudioSourceStatus.Playing);
        _onResume?.Invoke();
    }
    
    public void Stop(bool forceRecycle = false)
    {
        source.Stop();
        source.clip = null;
        SetStatus(AudioSourceStatus.None);

        if (forceRecycle)
        {
            pool.ReturnObject(this);
        }
    }

    private void Complete()
    {
        if (source.loop)
        {
            _onStepComplete?.Invoke();
        }
        else
        {
            _onComplete?.Invoke();
            StartCoroutine(DelayComplete());
            pool.ReturnObject(this); //播放完毕自动归还到对象池
        }
    }

    private WaitForSeconds wait = new WaitForSeconds(0.5f);
    private IEnumerator DelayComplete()
    {
        yield return wait;
        pool.ReturnObject(this); //播放完毕自动归还到对象池
    }


    #region CallBack

    public AudioSourceProxy OnPause(Action cb)
    {
        _onPause += cb;
        return this;
    }
    
    public AudioSourceProxy OnResume(Action cb)
    {
        _onResume += cb;
        return this;
    }
    
    public AudioSourceProxy OnPlay(Action cb)
    {
        _onPlay += cb;
        return this;
    }
    
    public AudioSourceProxy OnComplete(Action cb)
    {
        _onComplete += cb;
        return this;
    }
    
    public AudioSourceProxy OnStepComplete(Action cb)
    {
        _onStepComplete += cb;
        return this;
    }

    public void ClearAllCallback()
    {
        _onPause = null;
        _onPlay = null;
        _onComplete = null;
        _onStepComplete = null;
        _onResume = null;
    }
    

    #endregion


    #region Setter&Getter

    public AudioSourceProxy SetSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, -3, 3);
        source.pitch = speed;
        _playSpeed = speed;
        return this;
    }

    public AudioSourceProxy SetIgnorePause(bool ignore)
    {
        _ignorePause = ignore;
        return this;
    }

    public AudioSourceProxy SetMaxVolume(float max)
    {
        _maxVolume = max;
        return this;
    }

    public AudioSourceProxy SetVolume(float volume)
    {
        source.volume = volume * _maxVolume;
        return this;
    }

    /// <summary>
    /// 设置低通
    /// </summary>
    /// <param name="value">0-1之间的数</param>
    public AudioSourceProxy SetLowPass(float value)
    {
        lowPassFilter.cutoffFrequency = Mathf.Lerp(LowPassRange.x, LowPassRange.y, value);
        return this;
    }

    /// <summary>
    /// 设置低通范围
    /// </summary>
    public AudioSourceProxy SetLowPassRange(Vector2Int range)
    {
        LowPassRange = range;
        return this;
    }

    public AudioSourceProxy SetMixerGroup(AudioMixerGroup group)
    {
        source.outputAudioMixerGroup = group;
        return this;
    }

    public float GetMaxVolume()
    {
        return _maxVolume;
    }

    public AudioSourceProxy SetLoop(bool loop)
    {
        source.loop = loop;
        return this;
    }
    

    #endregion
    
    
    #region AudioRoom
    
    public void OutRoom(AudioRoom outRoom)
    {
        if (outRoom != belongRoom) //说明在离开某个房间之前，已经进入了另一个房间了
        {
            return;
        }
        SetVolume(1);
        SetLowPass(1);
        belongRoom = null;
    }

    public void InRoom(AudioRoom inRoom)
    {
        belongRoom = inRoom;
    }
    
    #endregion
    
    #region mono

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        // reverbFilter = GetComponent<AudioReverbFilter>();
    }

    private void OnEnable()
    {
        AudioManager.RegisterAudio(this);
    }

    private void OnDisable()
    {
        AudioManager.UnRegisterAudio(this);
    }

    public void UpdateLogic()
    {
        if (status != AudioSourceStatus.Playing)
        {
            return;
        }
        
        _curPlayLength += Time.unscaledDeltaTime * _playSpeed;
        if (_curPlayLength >= _audioLength)
        {
            _curPlayLength = 0;
            Complete();
        }
    }
    #endregion
    
    #region IPoolObject

    public ObjectPool pool { get; set; }
    public void ReturnToPool()
    {
        ClearAllCallback();
        belongRoom = null;
        _ignorePause = false; 
        _maxVolume = 1;
        _audioLength = 0;
        _curPlayLength = 0;
        SetStatus(AudioSourceStatus.Pool);
    }

    public void OnGetObjectFromPool()
    {
        
    }

    #endregion
    
}