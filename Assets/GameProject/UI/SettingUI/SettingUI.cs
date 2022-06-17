using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using UnityEngine.UI;
using LitJson;

namespace ProjectUI
{
    public class SettingUI : UIViewBase
    {
        public Toggle[] qulityToggle;
        public Slider volumeSlider;
        public Slider senestivitySlider;
        public Slider aimSenstivitySlider;

        public UIBtnBase ysBtn;
        public UIBtnBase yhBtn;
        public UIBtnBase cdkeyBtn;
        public UIBtnBase uiSetBtn;
        public UIBtnBase backBtn;

        public GameObject[] qulityBtnBag;

        protected override void OnChildStart()
        {
            for (int i = 0; i < qulityToggle.Length; i++)
            {
                int index = i;
                qulityBtnBag[index].GetComponent<Image>().enabled = (qulityToggle[i].isOn == false);
                qulityToggle[i].onValueChanged.AddListener((b) =>
                {
                    if (b)
                    {
                        OnSelectQulity(index);
                    }
                });
            }
           
            volumeSlider.onValueChanged.AddListener(OnChangeVolume);
            senestivitySlider.onValueChanged.AddListener(OnSenstivityChange);
            aimSenstivitySlider.onValueChanged.AddListener(OnAimSenstivityChagne);
            uiSetBtn.AddListener(OnUISetting);
            backBtn.AddListener(OnClickBackBtn);
            cdkeyBtn.AddListener(OnClickCDKeyBtn);
            if (Channel.isChina)
            {
                ysBtn.OnActive(true);
                yhBtn.OnActive(true);
                ysBtn.AddListener(OnClickYSBtn);
                yhBtn.AddListener(OnClickYHBtn);
            }
          
            SetCDK();
        }

        private void OnUISetting()
        {
            if (winName == "BattleSettingUI")
            {
                UIController.Instance.Open("DIYGameUI", UITweenType.None,0.9f);
            }
            else
            {
                UIController.Instance.Open("DIYGameUI", UITweenType.None,1f);
            }
        }

        public override void Refresh(params object[] args)
        {
            qulityToggle[(int)Setting.graphicSetting.quality.value].isOn = true;
            volumeSlider.value = Setting.audioSetting.audioVolume.value;
            senestivitySlider.value = Setting.controllerSetting.sensitivity.value;
            aimSenstivitySlider.value = Setting.controllerSetting.aimSensitivity.value;
        }

        protected void SetCDK()
        {
            if (Channel.channel == ChannelType.AppStore || Channel.channel == ChannelType.AppStoreCN)
            {
                HttpArgs args = new HttpArgs();
                args.AddArgs("playerId", GameInfo.serverUid);
                if (Channel.channel == ChannelType.AppStore)
                {
                    args.AddArgs("channel", 2);//102
                }
                else if (Channel.channel== ChannelType.AppStoreCN)
                {
                    args.AddArgs("channel", 102);//102
                }
                args.AddArgs("activity", 1);
                HttpCache.Instance.Get("api/v1/activity/config", args, str =>
                {
                    //打开cdk
                    JsonData data = JsonMapper.ToObject(str);
                    var code = data["code"].ToString();
                    if (code == "200")
                    {
                        var rewardData = data["data"]["open"];
                        cdkeyBtn.OnActive(rewardData.ToBool());
                    }
                    else
                    {
                        cdkeyBtn.OnActive(false);
                    }
                }, 0, 30);
                
            }
            else {
                cdkeyBtn.OnActive(true);
            }
        }

        private void OnSelectQulity(int i)
        {
            Setting.graphicSetting.quality.WriteData((QualityType)i);
            for (int index = 0; index < qulityToggle.Length; index++)
            {
                qulityBtnBag[index].GetComponent<Image>().enabled = (i != index);
            }
        }

        private void OnChangeVolume(float f)
        {
            Setting.audioSetting.SetAudioVolume(f);
            Setting.audioSetting.SetMusicVolume(f);
        }

        private void OnSenstivityChange(float f)
        {
            Setting.controllerSetting.SetSensitivity(f);
        }

        private void OnAimSenstivityChagne(float f)
        {
            Setting.controllerSetting.SetAimSensitivity(f);
        }

        private void OnClickYSBtn()
        {
            SDK.SDKMgr.GetInstance().MyCommon.ShowPrivacyPolicy();
            GameDebug.Log("通过SDK显示隐私协议界面");
        }

        private void OnClickYHBtn()
        {
            Application.OpenURL("https://baidu.com");
        }

        private void OnClickCDKeyBtn()
        {
            UIController.Instance.Popup("CDKeyPopUI", UITweenType.None);
        }

        private void OnClickUISetBtn()
        { 
        
        }

        private void OnClickBackBtn()
        {
            GameDebug.LogError("当前页面为空,无法关闭当前界面");
            UIController.Instance.Back();
        }
    }
}