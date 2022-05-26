using Module;
using UnityEngine.UI;

public class AdsFail : UIViewBase
{
    public UIBtnBase ok;
    protected override void OnChildStart()
    {
        ok.AddListener(OnOk);
    }

    private void OnOk()
    {
        OnExit();
    }
}