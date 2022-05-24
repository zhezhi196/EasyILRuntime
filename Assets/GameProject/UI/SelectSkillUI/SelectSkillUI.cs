using System.Collections.Generic;
using GameProject.UI.SelectSkillUI;
using Module;
using UnityEngine;

public class SelectSkillUI : UIViewBase
{
    public Transform skillContent;
    public List<SkillItem> items = new List<SkillItem>();
    public UIBtnBase getRewardButton;
    public UIBtnBase back;
    public List<SlotItem> slot = new List<SlotItem>();

    protected override void OnChildStart()
    {
        base.OnChildStart();
        getRewardButton.AddListener(OnGetReward);
        back.AddListener(OnBack);
    }

    private void OnBack()
    {
        OnExit();
    }

    private void OnGetReward()
    {
        // var skill = PlayerSkill.RandomSkill(0);
        // skill.GetReward(1, 0);
        for (int i = 0; i < PlayerSkill.allSkill.Count; i++)
        {
            PlayerSkill.allSkill[i].GetReward(1, 0);
        }
    }

    public override void Refresh(params object[] args)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ObjectPool.ReturnToPool(items[i]);
        }

        items.Clear();
        Voter voter = new Voter(PlayerSkill.allSkill.Count, () => { skillContent.Sort((a, b) =>
            {
                return a._skill.dbData.ID.CompareTo(b._skill.dbData.ID);
            },items.ToArray()); });
        for (int i = 0; i < PlayerSkill.allSkill.Count; i++)
        {
            var temp = PlayerSkill.allSkill[i];
            LoadPrefab<SkillItem>("SkillItem", skillContent, item =>
            {
                item.SetItem(temp);
                items.Add(item);
                voter.Add();
            });
        }

        for (int i = 0; i < slot.Count; i++)
        {
            slot[i].Refresh();
        }
    }
}