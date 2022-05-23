using Module;
using UnityEngine;
/// <summary>
/// 声音容器,顺序播放
/// </summary>
[CreateAssetMenu(fileName = "SequenceAudioContainer", menuName = "Audio/顺序声音容器")]
public class SequenceAudioContainer : AudioContainer
{
    public override void Play(Transform transform = null)
    {
        if (Test)
        {
            AssetLoad.LoadGameObject<AudioSourceProxy>(_audioPrefabPath, transform, (go, a) =>
            {
                go.source.PlayOneShot(clips[index]);
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
        PlayAudio(audioNames[index]);
        index += 1;
        if (index >= audioNames.Length)
        {
            index = 0;
        }
    }
}
