using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectUI;
using Module;

public class AgeTipPopup : UIViewBase
{
    public UIBtnBase closeBtn;

    protected override void OnChildStart()
    {
        closeBtn.AddListener(OnClose);
    }

    private void OnClose()
    {
        UIController.Instance.Back();   
    }
}
