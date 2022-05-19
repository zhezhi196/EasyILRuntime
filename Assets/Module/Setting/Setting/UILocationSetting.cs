using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public struct UILocation
    {
        public string uiName;
        public string itemName;
        public Vector3 position;
        public float scale;
        public float alpha;
        public bool hasKey;

        public UILocation(string uiName, string itemName, string key)
        {
            this.uiName = uiName;
            this.itemName = itemName;
            hasKey = !key.IsNullOrEmpty();
            if (hasKey)
            {
                string[] temp = key.Split(ConstKey.Spite0);
                position = new Vector3(temp[0].ToFloat(), temp[1].ToFloat());
                scale = temp[2].ToFloat();
                alpha = temp[3].ToFloat();
            }
            else
            {
                position = Vector2.zero;
                scale = 1;
                alpha = 1;
            }
        }
        public UILocation(string uiName, string itemName, Vector2 pos,float scale,float alpha)
        {
            this.uiName = uiName;
            this.itemName = itemName;
            hasKey = true;
            position = pos;
            this.scale = scale;
            this.alpha = alpha;
        }
        
        public void Set(Vector2 pos, float scale, float alpha)
        {
            this.position = pos;
            this.scale = scale;
            this.alpha = alpha;
            LocalFileMgr.SetString($"{uiName}_{itemName}", ToString());
        }

        public void Delete()
        {
            LocalFileMgr.RemoveKey($"{uiName}_{itemName}");
        }

        public override string ToString()
        {
            return string.Join(ConstKey.Spite0.ToString(), position.x, position.y, scale, alpha);
        }
    }
    
    [Flags]
    public enum UILocationFlag
    {
        Position = 1,
        Scale = 2,
        Alpha = 4
    }

    public class UILocationSetting : SettingConfig
    {
        public override void Init()
        {
        }

        public override void Update()
        {
        }

        public void SetLocation(string uiName, string itemName, Vector2 positon, float scale, float alpha)
        {
            UILocation tar = new UILocation() {uiName = uiName, itemName = itemName};
            tar.Set(positon, scale, alpha);
        }

        public void DeleteLocation(string uiName, string itemName)
        {
            UILocation tar = new UILocation() {uiName = uiName, itemName = itemName};
            tar.Delete();
        }

        public UILocation GetPositon(string uiName, string itemName)
        {
            string localKey = $"{uiName}_{itemName}";
            return new UILocation(uiName, itemName, LocalFileMgr.GetString(localKey));
        }
    }
}