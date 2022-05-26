using Sirenix.OdinInspector;

public enum AnalyticsType
{
    [LabelText("广告开始播放")] AdsBegin = 1,
    [LabelText("广告结束播放")] AdsEnd,
    [LabelText("天赋学习")] GiftStudy,
    [LabelText("武器升级")] WeaponUpdate,
    [LabelText("获得物品")] GetProps,
    [LabelText("使用物品")] UseProps,
    [LabelText("交互物品")] InterProps,
    [LabelText("门打开")] DoorOpen,
    [LabelText("成就领取")] AchievementGet,
    [LabelText("每日任务领取")] DailyTaskGet,
    [LabelText("玩家死亡")] PlayerDead,
    [LabelText("点击游戏开始")] GameStart,
    [LabelText("结局通关")] GameEnd,
    [LabelText("打开杰西卡商店")] OpenJessica,
    [LabelText("打开杰西卡隐藏商店")] OpenHideJessica,
    [LabelText("杰西卡商店购买")] BuyJessica,
    [LabelText("隐藏杰西卡购买")] BuyHideJessica,
    [LabelText("确定个人信息指引")] PersonIndex,
    [LabelText("完成实名认证")] Shiming,
    [LabelText("播放动画")] PlayAnimation,
    [LabelText("完成教学")] CompleteTeach,
    [LabelText("开始节点")] StartNode,
    [LabelText("杀死怪")] KillMonster,
    [LabelText("3天登陆")] ThreeDay,
    [LabelText("领取补给箱")] GetPropBox
}