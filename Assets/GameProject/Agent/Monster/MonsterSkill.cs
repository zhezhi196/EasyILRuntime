using Module;
using Module.SkillAction;
using Project.Data;

public abstract class MonsterSkill : Skill, IHurtObject
{
    public Monster monster;
    public int dbId;
    public MonsterSkillData dbData;
    public float[] damagePlus;

    public override bool isReadyRelease
    {
        get { return monster.toTargetAngler <= 5 && base.isReadyRelease; }
    }

    public override void OnInit(ISkillObject owner)
    {
        monster = (Monster) owner;
        dbData = DataInit.Instance.GetSqlService<MonsterSkillData>().WhereID(dbId);
        if (dbData.cd != 0)
        {
            cd = new TimeAction(dbData.cd, owner);
        }

    }
    
    protected override void OnDispose()
    {
    }


    //todo 这个后期要删掉
    public virtual Damage OnHurt(Damage damage)
    {
        return damage;
    }
}