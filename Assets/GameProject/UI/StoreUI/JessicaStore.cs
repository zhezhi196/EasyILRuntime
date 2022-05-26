using System;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using Module;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

public class JessicaStore : UIViewBase
{
    public UIBtnBase back;
    public ToggleTree toggleTree;
    public StoreButton[] toggleButton;
    private List<JessicaShopItem> lastItem = new List<JessicaShopItem>();
    public ScrollRect scrollRect;
    public Text limitTime;
    public CanvasGroup fadeCanvas;
    public int currIndex;
    public Image jessica;
    public UIBtnBase refresh;
    public RewardBag refreshReward;
    public int type;
    public Label title;

    public string localWriteKey
    {
        get { return "JesscalLimit" + type; }
    }
    protected override void OnChildStart()
    {
        base.OnChildStart();
        back.AddListener(OnBack);
        refresh.AddListener(Refrt);
        for (int i = 0; i < toggleButton.Length; i++)
        {
            int index = i;
            toggleButton[i].AddListener(OnToggleButton, index);
        }
        refreshReward = (RewardBag)Commercialize.GetRewardBag(DataMgr.CommonData(33004).ToInt());
        fadeCanvas.transform.localScale = Tools.GetScreenScale();
    }

    public override void OnOpenStart()
    {
        base.OnOpenStart();
        //AudioPlay.PlayOneShot("Jessica_hi",(Audio)model.args[1]).SetIgnorePause(true);

    }
    
    

    private void Refrt()
    {
        refreshReward.GetReward(res =>
        {
            if (res.result == IapResultMessage.Success)
            {
                LocalFileMgr.RemoveKey(localWriteKey);
                toggleTree.NotifyToggleOn(3, true);
                jessica.gameObject.OnActive(false);
                refresh.gameObject.OnActive(false);
            }
        });
    }

    private void OnToggleButton(ToggleTreeStation from, ToggleTreeStation to, int index)
    {
        if (to == ToggleTreeStation.Off)
        {
            toggleButton[index].Select(false);
        }
        else if (to == ToggleTreeStation.Unactive)
        {
            toggleButton[index].gameObject.OnActive(false);
        }
        else if (to == ToggleTreeStation.On)
        {
            currIndex = index;
            toggleButton[index].Select(true);
            scrollRect.horizontalNormalizedPosition = 0;
            for (int i = 0; i < lastItem.Count; i++)
            {
                lastItem[i].ReturnToPool();
            }
            lastItem.Clear();

            if (index == 3)
            {
                string s = LocalFileMgr.GetString(localWriteKey);
                if (!TimeHelper.IsNewHour("xianshijiexika" + type) && s != null)
                {
                    if (s == string.Empty)
                    {
                        jessica.gameObject.OnActive(true);
                        refresh.gameObject.OnActive(true);
                    }
                    else
                    {
                        string[] id = s.Split(ConstKey.Spite0);
                        var alljessicaTimereward = GetAllReward(index, fd => id.Contains(fd.ID.ToString()));
                        jessica.gameObject.OnActive(false);
                        refresh.gameObject.OnActive(false);
                        SetItem(index,alljessicaTimereward);
                    }
                }
                else
                {
                    List<GameReward> resultBag = GetAllReward(index, fd => true);
                    var temp = RandomReward(resultBag);
                    SetItem(index,temp);
                }
            }
            else
            {
                jessica.gameObject.OnActive(false);
                refresh.gameObject.OnActive(false);
                List<GameReward> resultBag = GetAllReward(index,fd=>true);
                SetItem(index,resultBag);
            }
        }
    }
    
    protected override void Update()
    {
        base.Update();
        var now = TimeHelper.now;
        var sss = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
        var span = sss - now;
        limitTime.text = string.Format(Language.GetContent("1011"), span.Minutes, span.Seconds);
    }

    public override void Refresh(params object[] args)
    {
        base.Refresh(args);
        type = 1;//args[0].ToInt();
        GameDebug.LogError("Jesscal Refresh Start"+currIndex);
        toggleTree.NotifyToggleOn(currIndex, true);
        GameDebug.LogError("Jesscal End");
        if (type == 1)
        {
            title.SetKey("1016");
        }
        else
        {
            title.SetKey("1001");
        }
        
    }

    public override void OnCloseComplete()
    {
        base.OnCloseComplete();
        currIndex=0;
        // AudioPlay.PlayOneShot("Jessica_bye",(Audio)model.args[1]);
    }

    private void SetItem(int toggleIndex,List<GameReward> resultBag)
    {
        List<IRewardBag> newBag = new List<IRewardBag>();
        for (int i = 0; i < resultBag.Count; i++)
        {
            newBag.Add(resultBag[i]);
        }
        toggleButton[toggleIndex].SetReward(newBag);
        fadeCanvas.alpha = 0;
        for (int i = 0; i < lastItem.Count; i++)
        {
            lastItem[i].ReturnToPool();
        }

        lastItem.Clear();
        Voter voter = new Voter(resultBag.Count, () =>
        {
            scrollRect.content.Sort((a, b) => a.index.CompareTo(b.index), lastItem);
            fadeCanvas.DOFade(1, 0.2f).SetUpdate(true);
        });

        for (int i = 0; i < resultBag.Count; i++)
        {
            int index = i;
            LoadPrefab<JessicaShopItem>("ShopItem", scrollRect.content, item =>
            {
                item.SetItem(index, resultBag[index] ,voter.Add);
                lastItem.Add(item);
            });
        }
    }

    private List<GameReward> RandomReward(List<GameReward> resultBag)
    {
        var tem = resultBag.Random(4, rd => rd.stationCode == 0);
        LocalFileMgr.SetString(localWriteKey, string.Join(ConstKey.Spite0.ToString(), tem));
        return tem;
    }
    
    private List<GameReward> GetAllReward(int index,Predicate<JessicaData> predicate)
    {
        List<JessicaData> jessicaDatas = DataMgr.Instance.GetSqlService<JessicaData>().WhereList(da => da.type == index&& da.jessicaType==type);
        List<GameReward> resultBag = new List<GameReward>();
            
        for (int i = 0; i < jessicaDatas.Count; i++)
        {
            IRewardBag rew = null;
            rew = Commercialize.GetRewardBag(jessicaDatas[i]);

            if ((rew.stationCode == 0||rew.stationCode==2) && predicate.Invoke(jessicaDatas[i]))
            {
                resultBag.Add((GameReward) rew);
            }
        }

        return resultBag;
    }

    private void OnBack()
    {
        OnExit();
    }

}