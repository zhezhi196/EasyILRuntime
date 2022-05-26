using System;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectUI
{
    [Serializable]
    public class SettingCommon
    {
        public Toggle[] qulityToggle;
        public Slider volumeSlider;
        public Slider senestivitySlider;
        public Slider aimSenstivitySlider;
        public GameObject[] qulityBtnBag;

        [Space]
        [ReadOnly]
        public int enterMode = 1; //进入的模式1、主页面 2、战斗Ui

        public void Init()
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
        }


        public void Refresh(int enterMode)
        {
            qulityToggle[(int) Setting.graphicSetting.quality.value].isOn = true;
            volumeSlider.value = Setting.audioSetting.audioVolume.value;
            senestivitySlider.value = Setting.controllerSetting.sensitivity.value;
            aimSenstivitySlider.value = Setting.controllerSetting.aimSensitivity.value;
            this.enterMode = enterMode;
        }

        private void OnSelectQulity(int i)
        {
            Setting.graphicSetting.quality.WriteData((QualityType) i);
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
    }
}