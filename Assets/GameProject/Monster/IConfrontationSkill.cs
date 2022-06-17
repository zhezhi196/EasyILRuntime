using Module;

public interface IConfrontationSkill : IActiveSkill
{
    int moveToward { get; }
    Confrontation behavior { get; set; }
}