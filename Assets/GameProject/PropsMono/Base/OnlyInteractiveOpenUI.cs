using Module;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 只能交互打开一个UI界面
/// </summary>
public class OnlyInteractiveOpenUI : OnlyInteractive
{
    public override InterActiveStyle interactiveStyle => InterActiveStyle.Handle;

    [Space]
    [LabelText("打开的UI界面的名称")]public string OpenUIName;
    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (string.IsNullOrEmpty(OpenUIName))
        {
            GameDebug.Log($"{gameObject.name} 没有配置打开UI的名称");
            return false;
        }
        UIController.Instance.Open(OpenUIName, UITweenType.None, this);
        return true;
    }
}