using System.Collections;
using System.Collections.Generic;
using Module;
using Project.Data;
using UnityEngine;

/// <summary>
/// 零件
/// </summary>
public class PartsInfo : MoneyInfo
{
    public override MoneyType moneyType => MoneyType.Parts;

    public PartsInfo(PropData dbData) : base(dbData)
    {
        int curr = EncryptionHelper
            .AesDecrypt(DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0].parts, PlayerInfo.pid).ToInt();
        _count = new IntField(curr, FieldAesType.Aes);
    }

    public override float GetReward(float rewardCount, int editorId, string[] match, RewardFlag flag)
    {
        var baseCount = base.GetReward(rewardCount, editorId, match, flag);
        var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
        lastData.parts = saveData;
        lastData.totalParts = lastData.totalParts + (int) baseCount;
        DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
        if ((flag & RewardFlag.NoRecord) == 0)
        {
            if (maxGetCount < baseCount)
            {
                maxGetCount = (int) baseCount;
            }
        }
        return baseCount;
    }
    
     public override BoolField SpendMoney(IntField spendCount)
     {
         int ood = this.count;
         if (base.SpendMoney(spendCount))
         {
             int odd2 = this.count;
             var lastData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
             lastData.parts = saveData;
             lastData.consumParts = lastData.consumParts + ood - odd2;
             DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(lastData);
             return new BoolField(true);
         }
         return new BoolField(false);
     }
}