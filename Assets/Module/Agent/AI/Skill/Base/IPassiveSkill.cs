namespace Module
{
    /// <summary>
    /// 被动技能
    /// </summary>
    public interface IPassiveSkill: ISkill
    {
        bool Trigger(params object[] args);
    }
}