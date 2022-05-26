using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module;

public class CDKeyPopUI : UIViewBase
{
    public Button okBtn;
    public Button cancelBtn;
    public InputField inputField;
    private string cdkey;

    protected override void OnChildStart()
    {
        okBtn.onClick.AddListener(OnOkBtn);
        cancelBtn.onClick.AddListener(OnCancelBtn);
        inputField.onValueChanged.AddListener(OnInputChange);
    }

    public override void Refresh(params object[] args)
    {
        cdkey = "";
    }

    private void OnInputChange(string input)
    {
        cdkey = input;
    }

    private void OnOkBtn()
    {
        //提交兑换
        CDKey.TryGet(cdkey, key =>
        {
            UISequence seq = new UISequence();
            key.GetReward();
            for (int i = 0; i < key.rewards.Length; i++)
            {
                seq.Add("RewardUI", UITweenType.None, OpenFlag.Inorder, key.rewards[i]);
            }

            seq.Popup();

        }, str => 
        {
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("1124"), null);
        });
    }

    private void OnCancelBtn()
    {
        OnExit();
    }
}
