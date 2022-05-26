using Module;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 尸体，没啥用
/// </summary>
public class DeadBodyProp : OnlyReceiveEventProp
{
    [LabelText("是否显示血迹"),OnValueChanged("EditorSetBloodHide")]
    public bool bloodShow;
    public GameObject blood;


    private void EditorSetBloodHide()
    {
        if (blood != null)
        {
            blood.OnActive(bloodShow);
        }
    }
}