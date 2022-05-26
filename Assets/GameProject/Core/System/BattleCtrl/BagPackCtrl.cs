using System;
using System.Collections.Generic;
using Module;
using UnityEngine;
[Flags]
public enum PutBagFlag
{
    NotAnalystic = 1,
    NotAudio = 2,
}
public class BagPackCtrl : BattleSystem
{
    public List<BagItem> bagList = new List<BagItem>();

    public override void OnRestart(EnterNodeType enterType)
    {
        if (enterType == EnterNodeType.FromSave)
        {
            string[] bagStr = LocalSave.ReadGroup(LocalSave.savePath, "BagItem");
            for (int i = 0; i < bagStr.Length; i++)
            {
                string data = bagStr[i];
                BagItem item = new BagItem(data);
                bagList.Add(item);
            }
        }
        else if(enterType == EnterNodeType.Restart)
        {
            //部分道具永远不会删除，需要在重新开始的时候加入到背包中
            var array = LocalFileMgr.GetStringArray("PermanentEntities");
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var saveId = array[i].Split(ConstKey.Spite1)[0].ToInt();
                    if (!bagList.Exists(v => v.entity.dbData.ID == saveId))
                    {
                        BagItem item = new BagItem(array[i]);
                        bagList.Add(item);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 例如 传过来一个门,找到门对应的钥匙对应的格子
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public BagItem Match(IMatch target)
    {
        for (int i = 0; i < bagList.Count; i++)
        {
            if (bagList[i].CanMatch(target))
            {
                return bagList[i];
            }
        }

        return null;
    }

    public int PutToBag(PropEntity entity, int count, int editorid,string[] matchStr, PutBagFlag flag = 0)
    {
        if (count <= 0 && !GamePlay.Instance.GMUI) return 0;
        if (entity.dbData.putBag == 1)
        {
            if (bagList.Contains(item => item.entity == entity) && entity.dbData.bagShowCount == 1)
            {
                for (int i = 0; i < bagList.Count; i++)
                {
                    if (bagList[i].entity == entity)
                    {
                        var result = bagList[i].count + count;
                        bagList[i].SetCount(result);
                    }
                }
            }
            else
            {
                BagItem newItem = new BagItem(entity, count, editorid, matchStr);
                bagList.Add(newItem);
            }
        }

        if (count > 0 && (entity is MoneyInfo || entity is BulletEntity))
        {
            entity.GetIcon(TypeList.Normal, sp =>
            {
                if (sp != null)
                {
                    EventCenter.Dispatch<Sprite, float>(EventKey.DropProps, sp, count);
                }
            });
        }

        if ((flag & PutBagFlag.NotAnalystic) == 0)
        {
            AnalyticsEvent.SendEvent(AnalyticsType.GetProps, editorid.ToString());
        }

        return count;
    }
    
    public int ConsumeItem(PropEntity entity, int count)
    {
        if (entity.dbData.putBag != 1) return 0;
        int ree = 0;

        for (int i = 0; i < bagList.Count; i++)
        {
            if (bagList[i].entity == entity)
            {
                if (bagList[i].count > count)
                {
                    var result = bagList[i].count - count;
                    bagList[i].SetCount(result);
                    ree = count;
                }
                else
                {
                    int lastCount = bagList[i].count;
                    bagList[i].SetCount(0);
                    bagList.RemoveAt(i);
                    ree = lastCount;
                }
            }
        }

        return ree;
    }

    public int ConsumeItem(BagItem item, int count)
    {
        return ConsumeItem(item.entity, count);
    }

    public int ConsumeItem(int id, int count)
    {
        BagItem find = null;
        find = bagList.Find((v) => v.entity.dbData.ID == id);

        if (find != null)
        {
            return ConsumeItem(find, count);
        }
        else
        {
            return 0;
        }
    }

    public override void Save()
    {
        LocalSave.DeleteGroup(LocalSave.savePath ,"BagItem");
        for (int i = 0; i < bagList.Count; i++)
        {
            LocalSave.Write(bagList[i]);
        }
    }

    /// <summary>
    /// 根据id获取到
    /// </summary>
    /// <param name="entityId"></param> 
    /// <returns></returns>
    public int GetBagItemNum(int entityId)
    {
        if (!bagList.IsNullOrEmpty())
        {
            for (int i = 0; i < bagList.Count; i++)
            {
                if (bagList[i].entity.dbData.ID == entityId)
                {
                    return bagList[i].count;    
                }
            }
        }
        return 0;
    }
    
    public int GetBagItemNum(PropEntity entity)
    {
        return GetBagItemNum(entity.dbData.ID);
    }
}