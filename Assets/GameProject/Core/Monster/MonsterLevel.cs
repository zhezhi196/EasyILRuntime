using System;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonsterLevel
{
    public int index;
    public MonsterAttribute attribute;
    public MonsterData dbData;
    public MonsterLeveEditor editorData;

    public MonsterLevel(int index, MonsterLeveEditor editorData, IMonster monster)
    {
        this.index = index;
        this.editorData = editorData;
        this.dbData = DataMgr.Instance.GetSqlService<MonsterData>().WhereID(this.editorData.dataId);
        if (this.dbData != null)
        {
            attribute = new MonsterAttribute(dbData);
        }

        if (monster is IAnimatorObject anim)
        {
            var animatorCtrl = anim.GetAgentCtrl<AnimatorCtrl>();
            if (anim.animator != null)
            {
                if (anim.animator.runtimeAnimatorController != editorData.animatorController && editorData.animatorController != null)
                {
                    anim.animator.runtimeAnimatorController = editorData.animatorController;
                    animatorCtrl.ChangeController(anim);
                }
            }
        }

    }
}