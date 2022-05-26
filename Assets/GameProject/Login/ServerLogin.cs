using System;
using System.Collections.Generic;
using LitJson;
using Module;
using UnityEngine;

public class ServerLogin
{
    public const string loginUrl = "api/v1/user/login";

    // /// <summary>
    // /// 查询玩家的状态,是否作弊
    // /// </summary>
    // /// <param name="callback"></param>
    // public void QuereStatus(Action<string> callback)
    // {
    //     HttpArgs args = new HttpArgs();
    //     args.AddArgs("uid", GameInfo.serverUid);
    //     Dictionary<string, string> header = new Dictionary<string, string>();
    //     header.Add("Authorization", "Bearer " + GameInfo.token);
    //     NetWorkHttp.Instance.Get(HttpCache.Instance.url + "api/v1/player/info", args, str =>
    //     {
    //         if (str.IsNullOrEmpty())
    //         {
    //             callback?.Invoke(null);
    //         }
    //         else
    //         {
    //             JsonData data = JsonMapper.ToObject(str);
    //             string code = data["code"].ToString();
    //             if (code == "200")
    //             {
    //                 var d = data["data"];
    //                 string c = d["c"].ToString();
    //                 string w = d["w"].ToString();
    //                 var service = DataInit.Instance.GetSqlService<PlayerSaveData>();
    //                 PlayerSaveData newSave = service.tableList[0];
    //                 newSave.c = c;
    //                 newSave.w = w;
    //                 DataInit.Instance.GetSqlService<PlayerSaveData>().Update(newSave);
    //                 callback?.Invoke("200");
    //             }
    //             else
    //             {
    //                 callback?.Invoke(null);
    //             }
    //         }
    //     }, 30,header);
    // }

    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="callback"></param>
    public void Login(Action callback)
    {
        HttpArgs args = new HttpArgs();
        args.AddArgs("uid", GameInfo.serverUid);
        args.AddArgs("channel", "1");
        args.AddArgs("type", "default");
        HttpCache.Instance.Post(loginUrl, args, msg =>
            {
                JsonData jsonData = JsonMapper.ToObject(msg);
                string code = jsonData["code"].ToString();
                if (code == "200")
                {
                    long expire = jsonData["expire"].ToLong();
                    string c = jsonData["c"].ToString();
                    string w = jsonData["w"].ToString();
                    string token = jsonData["token"].ToString();
                    string playerId = jsonData["playerId"].ToString();
                    PlayerSaveData oldData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
                    oldData.expire = expire;
                    oldData.token = token;
                    oldData.c = c;
                    oldData.w = w;
                    oldData.playerId = playerId;
                    DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(oldData);
                    if (!GameInfo.isMod)
                    {
                        callback?.Invoke();
                    }
                }
                else if (code == "-1")
                {
                    //token验证没通过
                    PlayerSaveData oldData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
                    oldData.token = string.Empty;
                    DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(oldData);
                    Login(callback);
                }
            }, false, 0, 30
        );
    }

    public void GetTime()
    {
        
    }
}