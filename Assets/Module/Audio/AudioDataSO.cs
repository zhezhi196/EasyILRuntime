using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public enum AudioRandomRules
{
    [LabelText("无")]None,
    [LabelText("随机")]Random,
    [LabelText("顺序")]Sequence,
}

[Serializable]
public class AudioDataUnitPath
{
    [HorizontalGroup("2",MinWidth = 200,MaxWidth = 400),LabelWidth(30)]
    public string path;
    [HorizontalGroup("2",Width = 150),LabelText("低通范围"),LabelWidth(50)]
    public Vector2Int lowpassRange;
}

[Serializable]
public class AudioDataUnit
{
    private static string[] mixerNames = new[] {"BGM","Sound","Environment"};
    
    [HorizontalGroup("1"),HideLabel,LabelWidth(80)]
    public string name;
    [HorizontalGroup("1"),HideLabel,LabelWidth(30)]
    public AudioRandomRules rule;
    [HorizontalGroup("1"),LabelText("路径"),LabelWidth(30)]
    public List<AudioDataUnitPath> paths;
    [HorizontalGroup("1"),LabelText("BGM"),LabelWidth(30)]
    public bool isBGM = false;
    [HorizontalGroup("1",100),HideLabel,ValueDropdown("mixerNames",DropdownWidth = 100)]
    public string mixerName;
    
    [HideInInspector]
    public int rulePtr = 0; //根据规则制定的是随机index还是顺序index
}

[Serializable]
[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData", order = 0)]
public class AudioDataSO : SerializedScriptableObject
{
    public List<AudioDataUnit> units;
    
    public AudioDataUnitPath GetAudioPath(string name , out AudioDataUnit outUnit, bool fromSequenceBegin = false)
    {
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            if (unit.name != name)
            {
                continue;
            }

            outUnit = unit;
            if (unit.rule == AudioRandomRules.None)
            {
                return unit.paths[0];
            }
            else if (unit.rule == AudioRandomRules.Random)
            {
                return unit.paths[Random.Range(0, unit.paths.Count)];
            }
            else if (unit.rule == AudioRandomRules.Sequence)
            {
                if (fromSequenceBegin)
                {
                    unit.rulePtr = 0;
                }
                var ret = unit.paths[unit.rulePtr];
                unit.rulePtr = (unit.rulePtr + 1) % unit.paths.Count;
                return ret;
            }
        }

        outUnit = null;
        return null;
    }
}