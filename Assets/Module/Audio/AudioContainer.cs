using UnityEngine;
using Sirenix.OdinInspector;
using Module;
/// <summary>
/// 声音容器,播放第一个
/// </summary>
[CreateAssetMenu(fileName = "SingleAudioContainer", menuName = "Audio/单个声音容器")]
public class AudioContainer : ScriptableObject
{
    public bool Test = false;
    [ShowIf("Test")]
    public AudioClip[] clips;
    public string[] audioNames;
    [LabelText("忽略暂停")]
    public bool ignorePause = false;
    [ReadOnly]
    public int index = 0;
    //对音效随机变化
    public bool randomPitch;
    [ShowIf("randomPitch"), Range(0, 0.5f)]
    public float minRandom = 0;
    [ShowIf("randomPitch"), Range(0, 0.5f)]
    public float maxRandom = 0;
    protected const string _audioPrefabPath = "AudioCompont/AudioPrefab.prefab";
    public virtual void Play(Transform transform = null)
    {
        if (Test)
        {
            AssetLoad.LoadGameObject<AudioSourceProxy>(_audioPrefabPath, transform, (go, a) =>
            {
                go.source.PlayOneShot(clips[0]);
            });
            //AudioSource.PlayClipAtPoint(clips[0], transform?transform.position: Vector3.zero);
            return;
        }
        if (audioNames.Length>=0)
        {
            PlayAudio(audioNames[0]);
        }
    }

    public virtual AudioPlay PlayAudio(string audioName)
    {
        return AudioPlay.PlayOneShot(audioName).SetIgnorePause(ignorePause);
    }

    public virtual AudioPlay PlayAudio(string audioName, Transform trans)
    {
        return AudioPlay.PlayOneShot(audioName, trans).SetIgnorePause(ignorePause);
    }
}
