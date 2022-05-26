using System.Collections.Generic;
using Module;
using UnityEngine;

public enum SensorMonsterType
{
    Shot,
    Walk,
    Attention,
    MonsterAttention,
}
public class MonsterCtrl: BattleSystem
{
    public RunTimeAction loadMonster;
    public List<IMonster> exitMonster = new List<IMonster>();
    private Dictionary<MonsterType, int> navProperty = new Dictionary<MonsterType, int>();
    public int DeadMonster;

    /// <summary>
    /// boss 1-10 精英11-50, 小怪51-100
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetMonsterProperty(MonsterType type)
    {
        int min = 0;
        int max = 0;
        if (type == MonsterType.Normal)
        {
            min = 51;
            max = 100;
        }
        else if (type == MonsterType.Elite)
        {
            min = 11;
            max = 50;
        }
        else if (type == MonsterType.Boss)
        {
            min = 0;
            max = 10;
        }
        int property = min;
        if (!navProperty.TryGetValue(type, out property))
        {
            property = min;
            navProperty.Add(type, property);
        }

        int next = min + (property - min + 1) % (max - min);
        
        navProperty[type] = next;
        return property;
    }

    public override void OnPlayerDead()
    {
        for (int i = 0; i < exitMonster.Count; i++)
        {
            if (exitMonster[i].isAlive && exitMonster[i] is AttackMonster)
            {
                exitMonster[i].ResetToBorn();
            }
        }
    }

    public override void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        base.OnNodeEnter(node, enterType);
        loadMonster = new RunTimeAction(() =>
        {
            if (node is TaskNode task)
            {
                Voter v = new Voter(MonsterCreator.creatList.Count, () =>
                {
                    BattleController.Instance.NextFinishAction("loadMonster");
                    loadMonster = null;
                });
                for (int i = 0; i < MonsterCreator.creatList.Count; i++)
                {
                    MonsterCreator.creatList[i].OnNodeEnter(task, monster =>
                    {
                        if (monster != null)
                        {
                            exitMonster.Add(monster);
                        }

                        v.Add();
                    });
                }
            }
        });
    }

    public void TrySensorMonster(Monster sensor,float radius)
    {
        List<AgentHear> hearTargets = AgentHear.TrySensor(Player.player.chasePoint, radius,tf => sensor.ear != tf);
        if (!hearTargets.IsNullOrEmpty())
        {
            for (int i = 0; i < hearTargets.Count; i++)
            {
                if (sensor.ear != hearTargets[i])
                {
                    hearTargets[i].Sensor(Player.player.chasePoint, "Chase");
                }
            }
        }
    }

    public void TrySensorMonster(SensorMonsterType type, Vector3 point, float radius)
    {
        List<AgentHear> hearTargets = AgentHear.TrySensor(point, radius);

        if (!hearTargets.IsNullOrEmpty())
        {
            for (int i = 0; i < hearTargets.Count; i++)
            {
                if (type == SensorMonsterType.Shot || type == SensorMonsterType.Walk)
                {
                    hearTargets[i].Sensor(Player.player.chasePoint, "Chase");
                }
                else
                {
                    IMonster nearMonster = exitMonster.Min((a, b) => a.transform.position.Distance(point).CompareTo(b.transform.position.Distance(point)));
                    if (nearMonster != null && nearMonster is Monster hearMonster)
                    {
                        if (hearMonster.ear == hearTargets[i])
                        {
                            //最近的怪查看
                            hearTargets[i].Sensor(point, "Check");
                        }
                        else
                        {
                            //其他的怪原地查看
                            hearTargets[i].Sensor(point, "yuandiCheck");
                        }
                    }
                }
            }
        }
    }

    public override void Save()
    {
        for (int i = 0; i < MonsterCreator.creatList.Count; i++)
        {
            MonsterCreator.creatList[i].Save();
        }
    }

    public void TryRemoveMonster(IMonster monster)
    {
        if (exitMonster.Contains(monster))
        {
            exitMonster.Remove(monster);
        }
    }
}