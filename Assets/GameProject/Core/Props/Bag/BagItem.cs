using System.Collections.Generic;
using Module;

public class BagItem : ILocalSave, IMatch
{
    public static string BagSaveId(PropEntity entity)
    {
        return "Bag_" + entity.dbData.ID;
    }

    private IntField _count;
    public PropEntity entity;
    public int count => _count.value;
    public Match[] matchObject { get;}
    public int editorId;

    public bool showCount
    {
        get { return entity.dbData.bagShowCount == 1; }
    }

    public bool isShowInbag
    {
        get { return entity.dbData.putBag == 1; }
    }

    public BagItem(string saveData)
    {
        string[] spiteCount = saveData.Split(ConstKey.Spite1);
        entity = PropEntity.GetEntity(spiteCount[0].ToInt());
        _count = new IntField(spiteCount[1].ToInt());
        string[] match = spiteCount[2].Split(ConstKey.Spite0);
        matchObject = new Match[match.Length];
        for (int i = 0; i < matchObject.Length; i++)
        {
            matchObject[i] = Match.GetMatch(match[i], "Battle");
            matchObject[i].TryAddToStore(this);
        }

        editorId = spiteCount[3].ToInt();
    }

    public BagItem(PropEntity entity, int count, int editorid,params string[] match)
    {
        this.entity = entity;
        this._count = new IntField(count);
        if (!match.IsNullOrEmpty())
        {
            matchObject = new Match[match.Length];
            for (int i = 0; i < matchObject.Length; i++)
            {
                matchObject[i] = Match.GetMatch(match[i], "Battle");
                matchObject[i].TryAddToStore(this);
            }
        }

        this.editorId = editorid;
    }

    public string localFileName => LocalSave.savePath;
    public string localGroup => "BagItem";

    public string localUid
    {
        get { return BagSaveId(entity); }
    }
    
    public string GetWriteDate()
    {
        List<string> mtstr = new List<string>();
        if (!matchObject.IsNullOrEmpty())
        {
            for (int i = 0; i < matchObject.Length; i++)
            {
                mtstr.Add(matchObject[i].matchKey.ToString());
            }
        }

        return string.Join(ConstKey.Spite1.ToString(), entity.dbData.ID, count, string.Join(ConstKey.Spite0.ToString(), mtstr.ToArray()), editorId);
    }


    public void OnMatchSuccess(IMatch[] target)
    {
    }

    public bool CanMatch(IMatch target)
    {
        if (matchObject.IsNullOrEmpty()) return false;
        for (int i = 0; i < matchObject.Length; i++)
        {
            if (matchObject[i].CanMatch(target))
            {
                return true;
            }
        }

        return false;
    }

    public void SetCount(int count)
    {
        this._count = new IntField(count);
    }
}