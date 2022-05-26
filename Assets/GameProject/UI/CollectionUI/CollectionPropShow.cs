using Module;
using Project.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPropShow : MonoBehaviour
{
    [Sirenix.OdinInspector.ReadOnly]
    public string skinItemPath = "UI/CollectionUI/CollectionSkinItem.prefab";
    public Text propName;
    public GameObject skinItemRoot;
    public Transform itemGrid;
    private Dictionary<int, List<CollectionSkinItem>> weaponSkinDic = new Dictionary<int, List<CollectionSkinItem>>();
    private int dicKey = 0;
    private ToggleGroup group;
    public void RefeshProp(CollectionItem item)
    {
        if (item == null)
        {
            this.gameObject.OnActive(false);
            return;
        }
        else {
            this.gameObject.OnActive(true);
        }
        propName.text = Language.GetContent((item.target.target.collectionData as PropData).title);
        if (item.target.target.collectionType == CollectionType.Weapon)
        {
            WeaponEntity entity = (WeaponEntity)item.target.target;
            if (dicKey != 0)
            {
                for (int i = 0; i < weaponSkinDic[dicKey].Count; i++)
                {
                    weaponSkinDic[dicKey][i].gameObject.OnActive(false);
                }
                dicKey = 0;
            }
            if (weaponSkinDic.ContainsKey(entity.dbData.ID))
            {
                dicKey = entity.dbData.ID;
                for (int i = 0; i < weaponSkinDic[dicKey].Count; i++)
                {
                    weaponSkinDic[dicKey][i].Refesh(); ;
                    weaponSkinDic[dicKey][i].gameObject.OnActive(true);
                    //if (weaponSkinDic[dicKey][i].skin.collectionStation == SkinEntity.SkinStation.Equip)
                    //{
                    //    weaponSkinDic[dicKey][i].toggle.isOn = true;
                    //}
                }
            }
            skinItemRoot.OnActive(entity.weaponSkins.Count > 0);
        }
        else
        {
            skinItemRoot.OnActive(false);
            if (dicKey != 0)
            {
                for (int i = 0; i < weaponSkinDic[dicKey].Count; i++)
                {
                    weaponSkinDic[dicKey][i].gameObject.OnActive(false);
                }
                dicKey = 0;
            }
        }
    }

    public void AddWeaponSkin(ICollection collection)
    {
        if (group == null)
        {
            group = itemGrid.GetComponent<ToggleGroup>();
        }

        WeaponEntity entity = (WeaponEntity)collection;
        if (entity.weaponSkins.Count > 0)
        {
            if (!weaponSkinDic.ContainsKey(entity.dbData.ID))
            {
                weaponSkinDic.Add(entity.dbData.ID, new List<CollectionSkinItem>());
            }
            for (int i = 0; i < entity.weaponSkins.Count; i++)
            {
                AssetLoad.LoadGameObject<CollectionSkinItem>(skinItemPath, itemGrid, (s, o) =>
                {
                    s.gameObject.OnActive(false);
                    //s.toggle.group = group;
                    //s.toggle.onValueChanged.AddListener((b)=> { OnToggleChange(b, s); });
                    s.btn.AddListener(()=>OnClickItem(s));
                    s.Init(entity.weaponSkins[o[0].ToInt()]);
                    weaponSkinDic[entity.dbData.ID].Add(s);
                },i);
            }
        }
    }

    private void OnToggleChange(bool b,CollectionSkinItem item)
    {
        if (b) 
        {
            if (item.skin.collectionStation != SkinEntity.SkinStation.Equip)
            {
                WeaponManager.weaponAllEntitys[item.skin.dbData.weaponID].ChangeSkin(item.skin);
            }
            for (int i = 0; i < weaponSkinDic[dicKey].Count; i++)
            {
                weaponSkinDic[dicKey][i].Refesh(); ;
                if (weaponSkinDic[dicKey][i].skin.collectionStation == SkinEntity.SkinStation.Equip)
                {
                    weaponSkinDic[dicKey][i].toggle.isOn = true;
                }
            }
        }
    }

    private void OnClickItem(CollectionSkinItem item)
    {
        if (item.skin.collectionStation == SkinEntity.SkinStation.UnGet)
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent(item.skin.dbData.lockDes.ToString()), null,
              new PupupOption(null, Language.GetContent("702")));
            return;
        }
        if (item.skin.collectionStation != SkinEntity.SkinStation.Equip)
        {
            WeaponManager.weaponAllEntitys[item.skin.dbData.weaponID].ChangeSkin(item.skin);
        }
        for (int i = 0; i < weaponSkinDic[dicKey].Count; i++)
        {
            weaponSkinDic[dicKey][i].Refesh(); ;
            //if (weaponSkinDic[dicKey][i].skin.collectionStation == SkinEntity.SkinStation.Equip)
            //{
            //    weaponSkinDic[dicKey][i].toggle.isOn = true;
            //}
        }
    }
}
