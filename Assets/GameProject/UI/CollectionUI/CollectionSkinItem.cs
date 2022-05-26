using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module;

public class CollectionSkinItem : MonoBehaviour
{
    public Toggle toggle;
    public UIBtnBase btn;
    public Image skinIcon;
    public Image lockIcon;
    public Image equipICon;
    public SkinEntity skin;
    public void Init(SkinEntity skinEntity)
    {
        skin = skinEntity;
        skin.GetIcon(TypeList.Normal, (s) =>
        {
            skinIcon.sprite = s;
        });
    }

    public void Refesh()
    {
        lockIcon.gameObject.OnActive(skin.collectionStation == SkinEntity.SkinStation.UnGet);
        equipICon.gameObject.OnActive(skin.collectionStation == SkinEntity.SkinStation.Equip);
        //toggle.interactable = skin.collectionStation != SkinEntity.SkinStation.UnGet;
    }
}
