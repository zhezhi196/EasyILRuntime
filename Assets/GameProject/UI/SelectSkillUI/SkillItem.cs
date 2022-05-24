using Module;
using UnityEngine;
using UnityEngine.UI;

namespace GameProject.UI.SelectSkillUI
{
    public class SkillItem : UIBtnBase
    {
        public bool isSelect
        {
            get { return _skill.SelectIndex > 0; }
        }

        public Image icon;
        public ObjectPool pool { get; set; }
        public PlayerSkill _skill;
        public GameObject offGraphic;

        protected override void DefaultListener()
        {
            if (_skill.canUse)
            {
                if (_skill.SelectIndex == 0)
                {
                    SkillSlot.SetSlot(_skill);
                }
                else
                {
                    SkillSlot.RemoveSlot(_skill);
                }
            }
            else
            {
                UIController.Instance.Popup("ShowSkillUI", UITweenType.None, _skill);
            }
            offGraphic.OnActive(!isSelect);
        }

        public void SetItem(PlayerSkill skill)
        {
            this._skill = skill;
            skill.GetIcon("Normal", sp => icon.sprite = sp);
            offGraphic.OnActive(!isSelect);
        }

        public void ReturnToPool()
        {
            icon.sprite = null;
        }

        public void OnGetObjectFromPool()
        {
        }
    }
}