using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module;
using Project.Data;
using System.Reflection;

/// <summary>
/// 武器升级,下一等级属性
/// </summary>
public class PartDataItem : MonoBehaviour
{
    public Text attName;
    public Text upgradeData;

    public void Refesh(string name,float data)
    {
        attName.text = name;
        upgradeData.text = (data>0?"+ ":"- ")+Mathf.Abs(data).ToString() ;
    }
}
