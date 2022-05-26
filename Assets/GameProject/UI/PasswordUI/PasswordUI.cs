using System;
using System.Collections.Generic;
using System.Text;
using Chapter2.UI;
using Module;
using UnityEngine;
using UnityEngine.UI;

  public class PasswordUI : UIViewBase
    {
        public List<char> inputPassword = new List<char>();
        public UIBtnBase[] numberButton;
        public UIBtnBase Confirm;
        public UIBtnBase exit;
        public PasswordShower passwordShower;
        public UIBtnBase back;
        public Text title;
        public RectTransform keybord;

        private PasswordBoxProp _prop;
        protected override void OnChildStart()
        {
            base.OnChildStart();
            for (int i = 0; i < numberButton.Length; i++)
            {
                char index = char.Parse(i.ToString());
                numberButton[i].AddListener(OnNumber,index);
            }

            Confirm.AddListener(OnConfirm);
            exit.AddListener(ExitUi);
            back.AddListener(OnBack);
            keybord.localScale = Tools.GetScreenScale();
        }

        private void OnBack()
        {
            if (inputPassword.Count > 0)
            {
                inputPassword.RemoveAt(inputPassword.Count - 1);
                string builder = new string(inputPassword.ToArray());
                passwordShower.Input(builder);
            }
        }

        public override void Refresh(params object[] args)
        {
            _prop = (PasswordBoxProp) args[0];
        }

        public override void OnCloseComplete()
        {
            base.OnCloseComplete();
            BattleController.Instance.Continue(winName);
        }

        private void ExitUi()
        {
            UIController.Instance.Back();
            inputPassword.Clear();
            passwordShower.Input(String.Empty);
        }

        private void OnConfirm()
        {
            string temp = new string(inputPassword.ToArray());
            if (temp == _prop.password)
            {
                // AudioPlay.PlayOneShot("mima_right").SetIgnorePause(true);
                _prop.InputSuccess();
                UIController.Instance.Back();
            }
            else
            {
                // AudioPlay.PlayOneShot("mima_wrong").SetIgnorePause(true);
                title.gameObject.OnActive(true);
            }

            inputPassword.Clear();
            passwordShower.Input(String.Empty);
        }


        private void OnNumber(char obj)
        {
            if (inputPassword.Count >= passwordShower.inputText.Length) return;
            inputPassword.Add(obj);
            string builder = new string(inputPassword.ToArray());
            title.gameObject.OnActive(false);
            passwordShower.Input(builder);
            GameDebug.LogFormat("当前输入:{0}" , builder);
            //AudioPlay.PlayOneShot("mima_shuru");

        }
    }