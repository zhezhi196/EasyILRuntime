using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;

public class MainUILevel : MonoBehaviour
{
    [System.Serializable]
    public class LevelItem
    {
        public UIBtnBase btn;
        public UIBtnBase startBtn;
        public UIBtnBase rankBtn;
        public UIBtnBase unlockBtn;
        public UIBtnBase lockTipBtn;
        public int missionID;
        public Mission mission;
        public GameObject selectObj;
        public Text des;
        public Text time;
        public GameObject[] lockIcons;
        public Image levelIcon;
    }

    public List<LevelItem> levelItems = new List<LevelItem>();
    private int index = 0;

    private void Awake()
    {
        for (int i = 0; i < levelItems.Count; i++)
        {
            int ii = i;
            levelItems[i].btn.AddListener(()=> {
                SeleceLevel(ii);
            });
            levelItems[i].startBtn.AddListener(() => {
                EnterLevel(ii);
            });
            levelItems[i].rankBtn.AddListener(() =>
            {
                OpenRankList(ii);
            });
            levelItems[i].unlockBtn.AddListener(() =>
            {
                UnlockLevel(ii);
            });
            levelItems[i].lockTipBtn.AddListener(() =>
            {
                OnLockTipBtn(ii);
            });
            //mission初始化
            var mission = Mission.missionList.Find(m => m.dbData.ID == levelItems[i].missionID);
            levelItems[i].mission = mission;
            //levelItems[i].startBtn.gameObject.OnActive(mission.station != CommonStation.Locked);
            levelItems[i].des.text = mission.des;
            levelItems[i].time.transform.parent.OnActive(mission.isComplete);
            if (mission.station != CommonStation.Locked)//如果解锁了
            {
                //levelItems[i].time.transform.parent.OnActive(true);
                levelItems[i].time.text = mission.time.ToTimeShow(Language.GetContent("1507"));
                for (int j = 0; j < levelItems[i].lockIcons.Length; j++)
                {
                    levelItems[i].lockIcons[j].OnActive(false);
                }
                levelItems[i].levelIcon.SetAlpha(1f);
                levelItems[i].unlockBtn.OnActive(mission.station == CommonStation.Unlocked);//需要看广告解锁
            }
            levelItems[i].startBtn.OnActive(mission.station.ToInt() > 1);
            //else
            //{
            //    //levelItems[i].time.transform.parent.OnActive(false);
            //    levelItems[i].startBtn.OnActive(mission.station != CommonStation.Unlocked);
            //    levelItems[i].unlockBtn.OnActive(mission.station == CommonStation.Unlocked);//需要看广告解锁
            //}
        }
    }

    private void SeleceLevel(int i)
    {
        levelItems[index].selectObj.OnActive(false);
        index = i;
        levelItems[index].selectObj.OnActive(true);
    }

    private void EnterLevel(int i)
    {
        if (LocalSave.hasSave)
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("716"), null, new PupupOption(() =>
            {
                LocalSave.DeleteFile();
                BattleController.Instance.EnterBattle(levelItems[index].mission, EnterNodeType.Restart);
            }, Language.GetContent("702")), new PupupOption(null, Language.GetContent("703")));
        }
        else {
            BattleController.Instance.EnterBattle(levelItems[index].mission, EnterNodeType.Restart);
        }
    }

    private void OnLockTipBtn(int i)
    {
        Mission mission = Mission.missionList.Find(m => m.dbData.ID == levelItems[i].missionID);
        if (mission.station == CommonStation.Locked)
        {
            //724+i,计算多语言文本id,不合理,凑合用吧
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent((724 + i).ToString()), null,
           new PupupOption(null, Language.GetContent("702")));
        }
        else if (mission.station == CommonStation.Unlocked)
        {
            UnlockLevel(i);
        }
    }

    private void UnlockLevel(int i)
    {
        Mission mission = Mission.missionList.Find(m => m.dbData.ID == levelItems[i].missionID);
        //SpriteLoader.LoadIcon("kanguanggao", sp => {
        //    CommonPopup.Popup(Language.GetContent("701"), "看广告解锁", null,
        //        new PupupOption(() => {
        //            CommonPopup.Close();
        //        }, Language.GetContent("703")),
        //        new PupupOption(() => {
        //            CommonPopup.Close();
        //            mission.unlockIap.GetReward(res => {
        //                if (res.result == IapResultMessage.Success)
        //                {
        //                    for (int j = 0; j < levelItems[i].lockIcons.Length; j++)
        //                    {
        //                        levelItems[i].lockIcons[j].OnActive(false);
        //                    }
        //                    levelItems[i].levelIcon.SetAlpha(1f);
        //                    levelItems[i].startBtn.OnActive(true);
        //                    levelItems[i].unlockBtn.OnActive(false);
        //                }
        //            });
        //        }, Language.GetContent("702"), sp));
        //});
        mission.unlockIap.GetReward(res => {
            if (res.result == IapResultMessage.Success)
            {
                for (int j = 0; j < levelItems[i].lockIcons.Length; j++)
                {
                    levelItems[i].lockIcons[j].OnActive(false);
                }
                levelItems[i].levelIcon.SetAlpha(1f);
                levelItems[i].startBtn.OnActive(true);
                levelItems[i].unlockBtn.OnActive(false);
            }
        });
    }
    
    public void GMRefreshUnlocked()
    {
        var missons = Mission.missionList;
        for (int i = 0; i < missons.Count; i++)
        {
            if (missons[i].station == CommonStation.Unlocked)
            {
                var levelItem = levelItems.Find(v => v.missionID == missons[i].dbData.ID);
                levelItem.levelIcon.SetAlpha(1f);
                levelItem.startBtn.OnActive(true);
                levelItem.unlockBtn.OnActive(false);
            }
        }
    }

    private void OpenRankList(int i)
    {
        GameDebug.Log("OpenRankList:" + i.ToEnum<GameDifficulte>());
    }

    private void OnEnable()
    {
        index = 0;
        levelItems[index].selectObj.OnActive(true);
    }

    private void OnDisable()
    {
        levelItems[index].selectObj.OnActive(false);
        index = 0;
    }
}
