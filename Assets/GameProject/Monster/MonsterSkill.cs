using Module;
using Module.SkillAction;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MonsterSkill: Skill, IActiveSkill
{
    public static Color minsDistanceColor = new Color(0.51f, 1, 1);
    public static Color maxDistanceColor = new Color(1f, 0.68f, 0.51f);
    public static Color minDamageDistanceColor = new Color(1, 0.5f, 0.5f);
    public MonsterStation addStation = MonsterStation.Attack;
    
    public AttackMonster monster
    {
        get { return owner as AttackMonster; }
    }
    
    [HideInInspector]
    public MonsterSkillData dbData;
    public abstract float stopMoveDistance { get; }

    public int dbId;

    private PlayerDamage[] damages;

    public virtual PlayerDamage GetDamage(int index = 0)
    {
        float att = monster != null && monster.buffCtrl != null && monster.buffCtrl.buffList.Count == 0
            ? 0
            : ((WeekBuff) monster.buffCtrl.buffList[0]).attackDown;
        if (damages == null)
        {
            string[] dam = dbData.damage.Split(ConstKey.Spite0);
            damages = new PlayerDamage[dam.Length];
            if (owner is IMonster mm)
            {
                for (int i = 0; i < damages.Length; i++)
                {
                    damages[i] = new PlayerDamage(mm);
                    damages[i].damage = mm.currentLevel.dbData.att * dam[i].ToFloat() * (1 - att);
                }
            }
        }
        return damages[index];
    }

    public int weight
    {
        get { return dbData.weight; }
    }

    public override void OnInit(ISkillObject owner)
    {
        if (dbData == null) dbData = DataMgr.Instance.GetSqlService<MonsterSkillData>().WhereID(dbId);
        if (dbData.cd != 0)
        {
            cd = new TimeAction(dbData.cd, owner);
        }
    }

    public override bool isReadyRelease
    {
        get
        {
            return monster.isSeePlayer && base.isReadyRelease;
        }
    }
    
    protected override void OnReleaseStart()
    {
        monster.Idle();
        monster.AddStation(addStation);
    }

    protected override void OnReleaseEnd(bool complete)
    {
        monster.RemoveStation(addStation);
    }

    public virtual bool isWanted
    {
        get { return true; }
    }

    public virtual void UpdateTry()
    {
        if (monster.target != null)
        {
            monster.MoveTo(MoveStyle.Walk, Player.player.chasePoint, null);
        }
    }
}