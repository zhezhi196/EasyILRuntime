using UnityEngine;
/// <summary>
/// 军装（动画交互）
/// </summary>
public class MilitaryUniformProp : OnlyInteractive
{
    protected override bool OnInteractive(bool fromMonster = false)
    {
        //TODO 播放动画
        return true;
    }
}