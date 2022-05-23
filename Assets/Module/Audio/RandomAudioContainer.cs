using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 声音容器,随机播放
/// </summary>
[CreateAssetMenu(fileName = "RandomAudioContainer", menuName = "Audio/随机声音容器")]
public class RandomAudioContainer : AudioContainer
{
    public enum RandomType
    {
        Standard,
        Shuffle
    }
    public RandomType radomType;
    [ShowIf("radomType", RandomType.Shuffle)]
    public int shuffleCount = 0;


    public override void Play(Transform transform = null)
    {
        if (Test)
        {
            AssetLoad.LoadGameObject<AudioSourceProxy>(_audioPrefabPath, transform, (go, a) =>
            {
                go.source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
            });
            index += 1;
            if (index >= clips.Length)
            {
                index = 0;
            }
            return;
        }
        if (audioNames.Length <= 0)
        {
            return;
        }

        PlayAudio(audioNames[Random.Range(0, audioNames.Length)]);
        index += 1;
        if (index >= audioNames.Length)
        {
            index = 0;
        }
    }
}
