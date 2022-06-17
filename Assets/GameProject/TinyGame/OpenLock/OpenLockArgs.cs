using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct OpenLockArgArea
{
    [LabelText("区域位置长度"),MinMaxSlider(0,1),OnValueChanged("OnLengthChange")]
    public Vector2 pos;
    [LabelText("滑块速度"),PropertyRange(1,40)]
    public float moveSpeed;

    [LabelText("长度"),ReadOnly] public float length;

    public void OnLengthChange()
    {
        length = pos.y - pos.x;
    }
}

[CreateAssetMenu(fileName = "New OpenLockArgs", menuName = "小游戏/创建开锁游戏配置文件", order = 0)]
public class OpenLockArgs : ScriptableObject
{
    [LabelText("次数"),MinValue(1),MaxValue(5),OnValueChanged("OnChangeLockNum")]
    public int lockNum;

    [LabelText("单次区间")]
    public List<OpenLockArgArea> areas;
    
    [LabelText("广告赠与数量"),MinValue(1)]
    public int adGive;

    public void OnChangeLockNum()
    {
        if (areas.Count > lockNum)
        {
            areas.RemoveRange(lockNum , areas.Count - lockNum);
        }
        else
        {
            var t = lockNum - areas.Count;
            for (int i = 0; i < t; i++)
            {
                areas.Add(new OpenLockArgArea());
            }
        }
    }

    [Button("刷新长度")]
    private void OnClickRefreshLength()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            areas[i].OnLengthChange();
        }
    }
    
    
}
