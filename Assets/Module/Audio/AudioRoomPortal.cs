using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioRoomPortal : MonoBehaviour
{
    [OnValueChanged("EditorPortalIdChanged") , LabelText("通道ID (用于接受门事件)")]
    public int portalId = 1;
    [LabelText("是否开启通道")]
    public bool enable = false;
    [LabelText("开启声音通过比例"),Range(0,1)]
    public float enableSoundPassRate = 0.3f; //打开时，声音通过比例
    [LabelText("门宽度"),Range(0,5)]
    public float portalWidth = 1f;

    [LabelText("通道检测最小范围"),OnValueChanged("EditorRangeChangeMin")]
    public float portalDetectRangeMin = 5;
    
    [LabelText("通道检测最大范围"),OnValueChanged("EditorRangeChangeMax")]
    public float portalDetectRangeMax = 5;

    [HideInInspector]
    public Transform LeftPoint;
    [HideInInspector]
    public Transform RightPoint;
    
    private float _soundPassRate = 0;
    private Tween t;

    private const string eventKey = "OnOpenCloseRoomPortal";

    [ReadOnly]
    public bool showGizmos;
    
    private void OnEnable()
    {
        _soundPassRate = enable ? enableSoundPassRate : 0;
        
        EventCenter.Register<int,bool>(eventKey, EventOpen);
    }

    private void OnDisable()
    {
        EventCenter.UnRegister<int,bool>(eventKey , EventOpen);
    }


    [Button("显示线框")]
    private void OnClickShowGizmos()
    {
        showGizmos = !showGizmos;
    }

    [Button("隐藏该场景所有线框")]
    private void HideAllGizmos()
    {
        var portals = GetComponentInParent<AudioRoom>().transform.parent.GetComponentsInChildren<AudioRoomPortal>();
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].showGizmos = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            RightPoint.transform.localPosition = new Vector3(portalWidth / 2,0,0);
            LeftPoint.transform.localPosition = new Vector3(-portalWidth / 2,0,0);
            if (LeftPoint && RightPoint)
            {
                DrawTools.DrawBounds(new Bounds((LeftPoint.position + RightPoint.position) / 2, new Vector3((LeftPoint.position - RightPoint.position).magnitude,1,0.2f)),Quaternion.identity,Color.red);
                // Debug.DrawLine(LeftPoint.position,RightPoint.position,Color.blue);
            }
            
            DrawTools.DrawSector(transform.position,Quaternion.Euler(-90,180 - transform.rotation.eulerAngles.y,90),portalDetectRangeMin,180,Color.magenta);
            DrawTools.DrawSector(transform.position,Quaternion.Euler(-90,180 - transform.rotation.eulerAngles.y,90),portalDetectRangeMax,180,Color.yellow);
        }

    }

    public float GetCurSoundRate()
    {
        return _soundPassRate;
    }

    /// <summary>
    /// 得到给定位置在portal的音量大小比例
    /// </summary>
    public float GetVolumeRateInPos(Vector3 pos)
    {
        if (!enable)
        {
            return 0;
        }
        else if (Mathf.Abs(pos.y - transform.position.y) > 2) //不在统一水平面
        {
            return 0;
        }
        else if (Vector3.Dot(pos - transform.position, transform.forward) < 0) //在洞内
        {
            return 0;
        }

        return GetSourceDistanceRate(pos);
    }

    public float GetSourceDistanceRate(Vector3 sourcePos)
    {
        sourcePos.y = transform.position.y;
        var distance = Vector3.Distance(sourcePos, transform.position);
        if(distance > portalDetectRangeMax)
        {
            return 0;
        }
        else if (distance > portalDetectRangeMin)
        {
            return Mathf.Lerp(1, 0, (distance - portalDetectRangeMin) / (portalDetectRangeMax - portalDetectRangeMin));
        }
        else
        {
            return 1;
        }
    }

    private void EventOpen(int portalId , bool open)
    {
        if (portalId != this.portalId)
        {
            return;
        }
        GameDebug.Log($"声音通道{this.portalId}接收到事件：{open}");
        enable = open;
        Open(open);
    }
    
    public void Open(bool open , float duration = 0.5f)
    {
        if (t != null)
        {
            t.Kill();
            t = null;
        }
        
        if (open)
        {
            DOTween.To(() => { return 0; }, (v) => { _soundPassRate = v; }, enableSoundPassRate, 0.5f);
        }
        else
        {
            DOTween.To(() => { return _soundPassRate; }, (v) => { _soundPassRate = v; }, 0, 0.5f);
        }
    }
    
    
    #if UNITY_EDITOR
    private void EditorRangeChangeMin()
    {
        if (portalDetectRangeMax < portalDetectRangeMin)
        {
            portalDetectRangeMax = portalDetectRangeMin;
        }
    }
    
    private void EditorRangeChangeMax()
    {
        if (portalDetectRangeMax < portalDetectRangeMin)
        {
            portalDetectRangeMin = portalDetectRangeMax;
        }

    }

    private void EditorPortalIdChanged()
    {
        gameObject.name = portalId + "_AudioPortal";
    }
    #endif
}