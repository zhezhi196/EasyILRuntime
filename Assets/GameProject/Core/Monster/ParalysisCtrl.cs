using System;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Project.Data;

public class ParalysisCtrl : IAgentCtrl
{
    private bool _isPause;
    public Monster monster;
    public readonly Dictionary<MonsterPartType, int> currentValue = new Dictionary<MonsterPartType, int>();

    public ParalysisCtrl(Monster monster)
    {
        this.monster = monster;
    }

    public T GetAgentCtrl<T>() where T : IAgentCtrl
    {
        return monster.GetAgentCtrl<T>();
    }

    public void EditorInit()
    {
        throw new NotImplementedException();
    }

    public void OnDrawGizmos()
    {
        
    }

    public bool isPause { get; }

    public void OnUpdate()
    {
    }

    public void Pause()
    {
        _isPause = true;
    }

    public void Continue()
    {
        _isPause = false;
    }

    public void OnAgentDead()
    {
        currentValue.Clear();
    }

    public void OnDestroy()
    {
        
    }

    public void AddParalysis(MonsterPartType type, int value,Damage damage)
    {
        if (!monster.canAddParalysis) return;
        if (currentValue.ContainsKey(type))
        {
            currentValue[type] += value;
        }
        else
        {
            currentValue.Add(type, value);
        }

        if (currentValue[type] >= GetStiffe(type))
        {
            ClearPara();
            monster.OnPartFull(type,damage);
        }
    }

    private float GetStiffe(MonsterPartType type)
    {
        if (type == MonsterPartType.Head) return monster.currentLevel.dbData.headStiff;
        if (type == MonsterPartType.Body) return monster.currentLevel.dbData.bodyStiff;
        if (type == MonsterPartType.Leg) return monster.currentLevel.dbData.kneelStiff;
        return 0;
    }


    public void ClearPara()
    {
        Dictionary<MonsterPartType, int> mirror = new Dictionary<MonsterPartType, int>(currentValue);

        foreach (KeyValuePair<MonsterPartType, int> keyValuePair in mirror)
        {
            currentValue[keyValuePair.Key] = 0;
        }
    }
}