using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;
using Project.Data;
using DG.Tweening;

public class CollectionUI : UIViewBase
{
    [System.Serializable]
    public class ToggleConfig
    {
        public Toggle toggle;
        public CollectionType type;
        public CollectionPlane viewGroup;
        public RedPoint redPoint;
    }
    public UIBtnBase backBtn;
    public GameObject propGroup;
    [Space]
    //public GameObject previewGroup;
    public RawImage uiShow;
    public UIScrollBase scrollBase;
    public UIBtnBase desBtn;
    public CollectionPropShow propShow;
    public CanvasGroup itemsCanvas;
    [Space]
    public GameObject desPanelObj;
    public Text propDes;
    public UIBtnBase desBackBtn;
    public ScrollRect scrollRect;
    public ToggleConfig[] toggleConfigs;
    private CollectionItem currentItem;
    private bool initComplete = false;
    private bool isOpen = false;

    protected override void OnChildStart()
    {
        backBtn.AddListener(Back);
        desBtn.AddListener(OpenDesPanel);
        desBackBtn.AddListener(CloseDesPanel);
        scrollBase.AddDrag(((v1, v2, time) => { UI3DShow.Instance.OnRotateModel(winName,v2); }));
        uiShow.texture = RenderTextureTools.commonTexture;
        uiShow.transform.localScale = Tools.GetScreenScale();
        Voter voter = new Voter(Collection.collectionList.Count,()=> {
            initComplete = true;
            itemsCanvas.DOFade(1f, 0.5f).SetUpdate(true).OnComplete(()=> {
                itemsCanvas.interactable = true;
                itemsCanvas.blocksRaycasts = true;
            });

            //道具都加载完毕后在加载Toggle
            InitToggle();
        });
        
        for (int i = 0; i < Collection.collectionList.Count; i++)
        {
            //遍历所有的item异步加载
            toggleConfigs[(int)Collection.collectionList[i].target.collectionType].viewGroup.CreatItem(Collection.collectionList[i], () =>
            {
                voter.Add();
            });
            if (Collection.collectionList[i].target.collectionType == CollectionType.Weapon)
            {
                propShow.AddWeaponSkin(Collection.collectionList[i].target);
            }
        }
       
    }

    private void InitToggle()
    {
        for (int i = 0; i < toggleConfigs.Length; i++)
        {
            int ii = i;
            toggleConfigs[i].toggle.onValueChanged.AddListener((b) =>
            {
                OnChangeCollection(ii,b);
            });
            toggleConfigs[i].viewGroup.onSelectItem += OnSelectItem;
            toggleConfigs[i].redPoint.SetTarget(toggleConfigs[i].viewGroup.GetCollectionsArray());
        }
    }

    public async override void OnOpenStart()
    {
        await Async.WaitUntil(() => initComplete, gameObject);
        await Async.WaitForEndOfFrame(gameObject);
        isOpen = true;
        for (int i = 0; i < toggleConfigs.Length; i++)
        {
            toggleConfigs[i].viewGroup.Refesh();
        }
        // toggleConfigs[0].viewGroup.Show();
        toggleConfigs[0].toggle.isOn = true;
    }

    public override void OnCloseComplete()
    {
        toggleConfigs[0].toggle.isOn = true;
    }

    private void OnChangeCollection(int i,bool b)
    {
        toggleConfigs[i].toggle.interactable=!b;
        toggleConfigs[i].viewGroup.ChangeState(b);
    }

    private void OnSelectItem(CollectionItem item)
    {
        if (!isOpen)
            return;
        currentItem = item;
        if (item != null)
        {
            UI3DShow.Instance.OnShow(winName, item.target);
            if (currentItem?.target.station == CollectionStation.NewGet)
            {
                currentItem.target.station = CollectionStation.Get;
                currentItem.Refesh();
            }
        }
        else {
            UI3DShow.Instance.OnClose(winName);
        }
        propShow.RefeshProp(item);
    }

    private void OpenDesPanel()
    {
        propGroup.OnActive(false);
        propDes.text = string.Format("\n{0}\n", Language.GetContent((currentItem.target.target.collectionData as PropData).des));
        desPanelObj.OnActive(true);
    }

    private void CloseDesPanel()
    {
        propGroup.OnActive(true);
        desPanelObj.OnActive(false);
        scrollRect.content.anchoredPosition = Vector2.zero;
    }

    private void Back()
    {
        isOpen = false;
        UIController.Instance.Back();
        UI3DShow.Instance.OnClose(winName);
        for (int i = 0; i < toggleConfigs.Length; i++)
        {
            toggleConfigs[i].viewGroup.Hide();
        }
    }

    private void OnDestroy()
    {
        Async.StopAsync(gameObject);
    }
}
