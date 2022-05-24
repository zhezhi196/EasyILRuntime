using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSkillShow: MonoBehaviour
{
    public PlayerSkillInstance skillInstance;
    public Image cd;
    public Image image;

    private void Update()
    {
        if (skillInstance != null)
        {
            if (skillInstance.station != Module.SkillStation.Ready)
            {
                cd.fillAmount = skillInstance.cdTime / skillInstance.cdTotalTime;
            }
            else
            {
                cd.fillAmount = 0;
            }
        }
    }

    public void SetSkill(PlayerSkillInstance temSKi)
    {
        this.skillInstance = temSKi;
        temSKi.skillModle.GetIcon(DataType.Normal, sp =>
        {
            image.sprite = sp;
            cd.sprite = sp;
        });
    }
}