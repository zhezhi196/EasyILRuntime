using Module;
using UnityEngine;
using UnityEngine.UI;

namespace GameProject.UI.SelectSkillUI
{
    public class BlackItem : UIBtnBase
    {
        public Image icon;

        public int index
        {
            get { return tar.SelectIndex; }
        }
        public PlayerSkill tar;

        public void Select(PlayerSkill skill)
        {
            this.tar = skill;
            skill.GetIcon(DataType.Normal, sp =>
            {
                icon.gameObject.OnActive(true);
                icon.sprite = sp;
            });
        }

        public void UnSelect()
        {
            icon.gameObject.OnActive(false);
        }
    }
}