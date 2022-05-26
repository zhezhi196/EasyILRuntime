using System;
using System.Collections.Generic;
using LitJson;
using Module;
using UnityEngine;

[Flags]
public enum HttpFlag
{
    Freeze=1,
}
public class HttpCache : Singleton<HttpCache>
{
#if UNITY_EDITOR
    public string url = "http://test-kb3.chenguanservice.com/";
    //public string url = "http://192.168.0.58:8100";
#else
#if LOG_ENABLE
    public string url = "http://test-kb3.chenguanservice.com/";
#else
    public string url = "https://kb3.chenguanservice.com/";
#endif
#endif

    public ServerLogin login = new ServerLogin();
    public PostPlayerData post = new PostPlayerData();

    public static bool NetAvailable
    {
        get { return Application.internetReachability != NetworkReachability.NotReachable; }
    }

    private static GameObject _netError;

    public static GameObject netError
    {
        get
        {
            if (_netError == null) _netError = UICommpont.UIRoot.Find("Canvas/NetError").gameObject;
            return _netError;
        }
    }

    public void Get(string url, HttpArgs args, Action<string> callBack, HttpFlag flag,int timeOut = 0)
    {
        return;
        if (NetAvailable)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Authorization", "Bearer " + GameInfo.token);

            url = this.url + url;
            if ((flag & HttpFlag.Freeze) != 0)
            {
                BattleController.Instance.Pause("waitHttp");
                netError.OnActive(true);
            }
            NetWorkHttp.Instance.Get(url, args, str =>
            {
                if ((flag & HttpFlag.Freeze) != 0)
                {
                    BattleController.Instance.Continue("waitHttp");
                    netError.OnActive(true);
                }
                
                if (str.IsNullOrEmpty())
                {
                    NetWorkError(flag, () => { Get(url, args, callBack, flag | HttpFlag.Freeze, timeOut); });
                }
                else
                {
                    callBack?.Invoke(str);
                    if (GameInfo.isMod)
                    {
                        ShowIsMod();
                    }
                }
            }, timeOut, header);
        }
        else
        {
            NetWorkError(flag,() =>
            {
                Get(url, args, callBack, flag | HttpFlag.Freeze,timeOut);
            });
        }
    }
    
    public void Post(string url, HttpArgs args, Action<string> callBack, bool hasHeader, HttpFlag flag, int timeOut = 0)
    {
        return;
        if (NetAvailable)
        {
            Dictionary<string, string> header = null;
            if (hasHeader)
            {
                header = new Dictionary<string, string>();
                header.Add("Authorization", "Bearer " + GameInfo.token);
            }

            if ((flag & HttpFlag.Freeze) != 0)
            {
                BattleController.Instance.Pause("WaitHttp");
                netError.OnActive(true);
            }

            url = this.url + url;
            NetWorkHttp.Instance.Post(url, args, str =>
            {
                if ((flag & HttpFlag.Freeze) != 0)
                {
                    BattleController.Instance.Continue("WaitHttp");
                    netError.OnActive(false);
                }
                
                if (str.IsNullOrEmpty())
                {
                    NetWorkError(flag,() => { Post(url, args, callBack, hasHeader, flag | HttpFlag.Freeze, timeOut); });
                }
                else
                {
                    callBack?.Invoke(str);
                    if (GameInfo.isMod)
                    {
                        ShowIsMod();
                    }
                }
            }, timeOut, header);
        }
        else
        {
            NetWorkError(flag,() =>
            {
                Post(url, args, callBack, hasHeader, flag | HttpFlag.Freeze, timeOut);
            });
        }
    }

    public void ShowIsMod()
    {
#if !UNITY_IOS
        UIController.Instance.canPhysiceback = false;
        string serverId = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0].playerId;
        UIController.Instance.Popup("ModUI", UITweenType.None, serverId.ToString());
#endif
    }

    public void NetWorkError(HttpFlag flag, Action callback)
    {
#if !UNITY_IOS
        UIController.Instance.canPhysiceback = false;
        CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("735"), null,
            new PupupOption(() => { Application.Quit(); }, Language.GetContent("1112")),
            new PupupOption(() =>
            {
                UIController.Instance.canPhysiceback = false;
                callback?.Invoke();
            }, Language.GetContent("736"))
        );
#endif
    }

    public bool IsBlackOrWhite(int type, string serverData)
    {
        string notC = type == 0 ? "c1" : "w1";
        string temp = EncryptionHelper.MD5Encrypt(notC + GameInfo.token);
        return temp == serverData;
    }

}