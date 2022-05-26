using System.Collections.Generic;
using Module;
using UnityEngine;

public class DropCtrl: BattleSystem
{
    public List<DropBag> drops = new List<DropBag>();
    private string saveKey = "DropData";
    public override void OnRestart(EnterNodeType enterType)
    {
        if (enterType == EnterNodeType.FromSave)
        {
            string data = LocalSave.Read(LocalSave.savePath , saveKey);
            if (!data.IsNullOrEmpty())
            {
                string[] dropInfo = data.Split(ConstKey.Spite0);
                for (int i = 0; i < dropInfo.Length; i++)
                {
                    string[] drop = dropInfo[i].Split(ConstKey.Spite1);
                    int id = drop[0].ToInt();
                    string[] posStr = drop[1].Split(ConstKey.Spite2);
                    Vector3 pos = new Vector3(posStr[0].ToFloat(), posStr[1].ToFloat(), posStr[2].ToFloat());
                    string[] match = drop[2].Split(ConstKey.Spite2);
                    Drop newDrop = new Drop(id);
                    newDrop.GetDropBag(match, bag => bag.transform.position = pos);
                }
            }
        }
    }

    public override void Save()
    {
        List<string> dropInfo = new List<string>();
        for (int i = 0; i < drops.Count; i++)
        {
            Vector3 pos = drops[i].transform.position;
            string posStr = string.Join(ConstKey.Spite2.ToString(), pos.x, pos.y, pos.z);
            string mathc = string.Join(ConstKey.Spite2.ToString(), drops[i].drop.match);
            dropInfo.Add(string.Join(ConstKey.Spite1.ToString(), drops[i].drop.dbData.ID, posStr, mathc));
        }

        LocalSave.Write(LocalSave.savePath , saveKey, string.Join(ConstKey.Spite0.ToString(), dropInfo), "Drop");
    }
}