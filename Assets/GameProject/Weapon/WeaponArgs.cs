using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 武器参数配置
/// </summary>
[CreateAssetMenu(menuName ="Player/武器配置")]
public class WeaponArgs : ScriptableObject
{
    [LabelText("攻击距离(米)")] public float attckDistance = 2000f;
    [LabelText("首次拾取武器获得的子弹")] public int firstBulletCount = 1;
    [LabelText("拾取武器获得的子弹")] public int getBulletCount = 1;
    [LabelText("取消瞄准事件")] public float cancelAimTime = 3f;
    //----------------布娃娃设置-------------------
    [FoldoutGroup("布娃娃设置"), LabelText("命中冲击力")] 
    public float attackForce = 5000;
    [FoldoutGroup("布娃娃设置"), LabelText("冲击力延迟")] 
    public float forceDelay = 0.3f;
    [FoldoutGroup("布娃娃设置"), LabelText("动画到布娃娃过渡时间")] 
    public float animationToRagdollTime = 0.3f;
    //----------------精准度------------------------
    [FoldoutGroup("精准度"), LabelText("移动增长系数")]
    public float moveMul = 2f;
    [FoldoutGroup("精准度"), LabelText("下蹲移动系数")]
    public float crouchMoveMul = 1.5f;
    [FoldoutGroup("精准度"), LabelText("下蹲系数")]
    public float crouchMul = 0.5f;
    [FoldoutGroup("精准度"), LabelText("最大扩散")]
    public float maxAccurate = 70f;
    [FoldoutGroup("精准度"), LabelText("恢复速度")]
    public float fallAccurate = 80f;
    [FoldoutGroup("精准度"), LabelText("后坐力")]
    public float Recoil = 5;
    [FoldoutGroup("精准度"), LabelText("最大后坐力")]
    public float MaxRecoil = 30;
    [FoldoutGroup("精准度"), LabelText("扩散范围")]
    public float diffusionArea = 100;
    //----------------音效--------------------------
    [FoldoutGroup("音效设置"),LabelText("射击声音范围")]
    public float attackSoundRange = 100f;
    [FoldoutGroup("音效设置"),LabelText("击中声音范围")]
    public float hitSoundRange = 10f;
    [FoldoutGroup("音效设置"),LabelText("射击音效")]
    public string fireAudio;
    [FoldoutGroup("音效设置"),LabelText("瞄准音效")]
    public string toAimAudio;
    [FoldoutGroup("音效设置"),LabelText("取消瞄准音效")]
    public string toNoAimAudio;
    [FoldoutGroup("音效设置"),LabelText("无子弹射击音效")]
    public string emptyFireAuido;
    [FoldoutGroup("音效设置"),LabelText("换弹音效")]
    public string reloadAudio;
    [FoldoutGroup("音效设置"),LabelText("空仓换弹音效")]
    public string emptyReloadAudio;
}
