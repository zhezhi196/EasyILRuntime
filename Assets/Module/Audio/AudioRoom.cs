using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AudioRoom : MonoBehaviour
{
    public static Vector3 PlayerPos;
    [LabelText("声音通过率"),Range(0,1)]
    public float roomVolumePassRate = 0.3f; //房间对声音的通过比率
    
    [LabelText("低通率"),Range(0,1)]
    public float lowPassRate = 0.2f;

    [LabelText("房间的混音类型")]
    public AudioReverbPreset reverbPreset = AudioReverbPreset.Off;
    
    [LabelText("声音洞"), ReadOnly]
    public List<AudioRoomPortal> portals = new List<AudioRoomPortal>();
    [ReadOnly]
    public List<AudioSourceProxy> sources = new List<AudioSourceProxy>(); //房间内的source
    


    
    private bool isPlayerInRoom;
    private BoxCollider[] _colliders;
    
    private void OnEnable()
    {
        sources = new List<AudioSourceProxy>();
        _colliders = GetComponents<BoxCollider>();
        AudioManager.RegisterRoom(this);
    }

    private void OnDisable()
    {
        AudioManager.UnRegisterRoom(this);
    }

    public bool IsInRoom(Vector3 pos)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            return _colliders[i].bounds.Contains(pos);
        }
        return false;
    }

    public void UpdateLogic()
    {
        UpdateSourceInRoom();
        
        float roomPassRate = GetCurRoomPassRate() + roomVolumePassRate;
        float lowPassRateTemp = GetCurRoomPassRate() + lowPassRate;
        //假设角色没有在portal区域的声音计算
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].SetLowPass(lowPassRateTemp);
            sources[i].SetVolume(roomPassRate);
        }
        
        float rateMax = 0; 
        int index = -1;
        //计算角色在portal区域的声音
        for (int i = 0; i < portals.Count; i++)
        {
            if (!portals[i].enable)
            {
                continue;
            }
            float temp = portals[i].GetVolumeRateInPos(PlayerPos);
            if (temp >= rateMax) //找到角色在这个房间声音最大的portal
            {
                rateMax = temp;
                index = i;
            }
        }
        
        for (int j = 0; j < sources.Count; j++)
        {
            if (isPlayerInRoom)
            {
                sources[j].SetLowPass(1);
                sources[j].SetVolume(1);
            }
            else if (index > -1)
            {
                var closeRate = portals[index].GetSourceDistanceRate(sources[j].transform.position);
                rateMax = rateMax + (1 - rateMax) * closeRate; //音源距离最大音量portal的距离补正
                sources[j].SetLowPass(Mathf.Lerp(lowPassRateTemp,1, rateMax));
                sources[j].SetVolume(Mathf.Lerp(roomPassRate,1, rateMax));
            }
        }
    }

    private void ClearUselessAudio()
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].gameObject.activeSelf || !IsInRoom(sources[i].transform.position))
            {
                sources[i].OutRoom(this);
                sources.RemoveAt(i);
                i--;
            }
        }
    }
    private void UpdateSourceInRoom()
    {
        ClearUselessAudio();
        var sourceList = AudioManager.sources;
        for (int i = 0; i < sourceList.Count; i++)
        {
            //挑选出已经激活的、上一帧不在房间内的、这一帧在房间内的source
            if(sourceList[i].gameObject.activeSelf && !sources.Contains(sourceList[i]) && IsInRoom(sourceList[i].transform.position))
            {
                sourceList[i].InRoom(this);
                sources.Add(sourceList[i]);
            }
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        PlayerEar p = other.GetComponent<PlayerEar>();
        if (p == null)
        {
            return;
        }
        isPlayerInRoom = true;
        
        p.reverbZone.reverbPreset = reverbPreset;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerEar p = other.GetComponent<PlayerEar>();
        if (p == null)
        {
            return;
        }

        isPlayerInRoom = false;
        if (p.reverbZone.reverbPreset != reverbPreset)
        {
            return;
        }
        p.reverbZone.reverbPreset = reverbPreset;
    }

    private float GetCurRoomPassRate()
    {
        float ret = 0;
        for (int i = 0; i < portals.Count; i++)
        {
            ret += portals[i].GetCurSoundRate();
        }

        return ret;
    }


    
    #if UNITY_EDITOR

    [Button("添加声音洞")]
    private void EditorAddPortal()
    {
        for (int i = 0; i < portals.Count; i++)
        {
            if (portals[i] == null)
            {
                portals.RemoveAt(i);
                i--;
            }
        }
        GameObject go = new GameObject("Portal");
        go.transform.SetParentZero(transform);
        var portal = go.AddComponent<AudioRoomPortal>();
        var pointL = new GameObject("LeftPoint");
        var pointR = new GameObject("RightPoint");
        
        pointL.transform.SetParentZero(go.transform);
        pointR.transform.SetParentZero(go.transform);
        
        portal.LeftPoint = pointL.transform;
        portal.RightPoint = pointR.transform;
        portals.Add(portal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var c = GetComponent<BoxCollider>();
        Gizmos.DrawWireCube(c.bounds.center,c.bounds.size);
    }

#endif
    
    
}
