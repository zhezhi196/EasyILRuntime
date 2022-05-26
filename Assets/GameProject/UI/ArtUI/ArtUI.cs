using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using DG.Tweening;
using UnityEngine.UI;
public class ArtUI : UIViewBase
{
    public UIBtnBase backBtn;
    [Header("概念设计")]
    public UIBtnBase artBtn;
    public UIBtnBase artBackBtn;
    public GameObject artPanel;
    public GameObject itemsGroup;
    public GameObject itemView;
    public Image viewImage;
    public Text viewText;
    public ArtUIItem[] items;
    [Header("制作名单")]
    public UIBtnBase productionBtn;
    public GameObject nameList;
    public RectTransform namesRoot;
    public Vector2 startPos;
    public Vector2 endPos;
    public float speed = 5;
    private Tweener nameListTweener;

    protected override void OnChildStart()
    {
        backBtn.AddListener(OnClickBackBtn);
        artBtn.AddListener(OpenArtPanel);
        artBackBtn.AddListener(CloseArtPanel);

        productionBtn.AddListener(OpenNameList);
        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            items[i].btn.AddListener(() =>
            {
                ViewItem(index);
            });
        }
    }

    private void OnClickBackBtn()
    {
        UIController.Instance.Back();
    }

    private void OpenArtPanel()
    {
        itemsGroup.OnActive(true);
        itemView.OnActive(false);
        artPanel.OnActive(true);
    }

    private void CloseArtPanel()
    {
        artPanel.OnActive(false);
    }

    private void ViewItem(int index)
    {
        //GameDebug.Log("ViewIndex:" + index);
        viewImage.sprite = items[index].bigSprite;
        viewText.text = Language.GetContent(items[index].nameID);
        itemsGroup.OnActive(false);
        itemView.OnActive(true);
    }

    public void CloseViewItem()
    {
        itemsGroup.OnActive(true);
        itemView.OnActive(false);
    }

    private void OpenNameList()
    {
        nameList.OnActive(true);
        nameListTweener = namesRoot.DOAnchorPos(endPos, speed).SetId(winName).SetEase(Ease.Linear).SetDelay(0.5f)
            .OnComplete(NameListComplete);
    }

    private void NameListComplete()
    {
        CloseNameList();
    }


    public void CloseNameList()
    {
        nameList.OnActive(false);
        namesRoot.anchoredPosition = startPos;
        if (nameListTweener != null)
        {
            nameListTweener.Kill();
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(winName);
    }

#if UNITY_EDITOR
    [Header("创建字幕工具")]
    public TextAsset textAsset;
    public GameObject textPrefab;
    public float offset = -50f;
    [Sirenix.OdinInspector.Button("创建名单")]
    public void CreatText()
    {
        string[] str = textAsset.text.Split('\n');
        List<List<string>> strList = new List<List<string>>();
        for (int i = 0; i < str.Length; i++)
        {
            if (!string.IsNullOrEmpty(str[i]))
            {
                if (str[i].Contains("*"))
                {
                    List<string> ss = new List<string>();
                    ss.Add(str[i].Remove(0, 1));
                    strList.Add(ss);
                }
                else {
                    if (strList.Count > 0)
                    {
                        strList[strList.Count - 1].Add(str[i]);
                    }
                }
            }
        }
        float endOffset = 0;
        for (int i = 0; i < strList.Count; i++)
        {
            endOffset += offset;
            RectTransform o = GameObject.Instantiate(textPrefab, namesRoot).transform as RectTransform;
            o.anchoredPosition = new Vector2(0, endOffset);
            o.GetComponent<Text>().text = strList[i][0];
            GameObject childText = o.GetChild(0).GetChild(0).gameObject;
            RectTransform childRoot = o.GetChild(0) as RectTransform;
            for (int j = 1; j < strList[i].Count; j++)
            {
                RectTransform cT = GameObject.Instantiate(childText, childRoot).transform as RectTransform;
                cT.gameObject.OnActive(true);
                cT.GetComponent<Text>().text = strList[i][j];
                GameDebug.Log(strList[i][j]);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(childRoot);
            endOffset += offset*2- childRoot.sizeDelta.y;
        }
        endPos = new Vector2(0, -endOffset);
    }
#endif
}
