using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Player/属性配置")]
public class AttributeLanguage : ScriptableObject
{
    [System.Serializable]
    public class AttributeLangueConfig
    {
        [ValueDropdown("@AttributeHelper.GetAttFiled")]
        public string attName;
        public WeaponType weaponType;
        public string key;
    }

    public List<AttributeLangueConfig> configs = new List<AttributeLangueConfig>();

    public string GetAttKey(string attName,Weapon w)
    {
        string key = "";
        for (int i = 0; i < configs.Count; i++)
        {
            if (configs[i].attName == attName && configs[i].weaponType.HasFlag(w.weaponType))
            {
                key = configs[i].key;
            }
        }
        return key;
    }
}
