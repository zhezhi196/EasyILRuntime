using Module;
using UnityEngine;
namespace ProjectUI
{
    public class MainUI : UIViewBase
    {
        public UIBtnBase startGame;
        public UIBtnBase skillSelect;
        protected override void OnChildStart()
        {
            base.OnChildStart();
            startGame.AddListener(OnStartGame);
            skillSelect.AddListener(OnSelectSkill);
        }

        private void OnSelectSkill()
        {
            UIController.Instance.Open("SelectSkillUI", UITweenType.None);
        }

        private void OnStartGame()
        {
            BattleController.Instance.EnterBattle(Mission.missions[0], PlayMode.Day);
        }
    }
}
