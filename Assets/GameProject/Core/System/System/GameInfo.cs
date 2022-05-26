using System;
using Module;
using UnityEngine;
//using UnityEngine.iOS;

public class GameInfo
{
    public static int LoginDay
    {
        get { return LocalFileMgr.GetInt("LoginDay"); }
        set { LocalFileMgr.SetInt("LoginDay", value); }
    }

    public static int ContinueLoginDay
    {
        get { return LocalFileMgr.GetInt("ContinueLoginDay"); }
        set { LocalFileMgr.SetInt("ContinueLoginDay", value); }
    }

    public static int LoginCount
    {
        get { return LocalFileMgr.GetInt("LoginCountInfo"); }
        set { LocalFileMgr.SetInt("LoginCountInfo", value); }
    }

    public static DateTime LoginTime
    {
        get { return LocalFileMgr.GetDateTime("LoginTime"); }
        set { LocalFileMgr.SetDateTime("LoginTime", value); }
    }

    public static string PlayerName
    {
        get
        {
            var tab = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList;
            if (tab.IsNullOrEmpty()) return null;
            return tab[0].playerName; 
        }
        set
        {
            if (!PlayerName.IsNullOrEmpty())
            {
                DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(new PlayerSaveData() {playerName = value});
            }
        }
    }

    /// <summary>
    /// 进游戏的次数
    /// </summary>
    public static int enterGameCount
    {
        get { return LocalFileMgr.GetInt("EnterGameCount"); }
    }

    public static event Action<bool, TimeSpan> onLogin;

    public static AsyncLoadProcess Init(AsyncLoadProcess process)
    {
        process.IsDone = false;
        LocalFileMgr.SetInt("EnterGameCount", enterGameCount + 1);
        var playerSaveData = DataMgr.Instance.GetSqlService<PlayerSaveData>();
        if (playerSaveData.tableList.Count == 0)
        {
            playerSaveData.Insert(new PlayerSaveData());
        }
        var playerSaveDataAdd = DataMgr.Instance.GetSqlService<PlayerSaveDataAdd>();
        if (playerSaveDataAdd.tableList.Count == 0)
        {
            playerSaveDataAdd.Insert(new PlayerSaveDataAdd());
        }
        RecordLogin();
        process.SetComplete();
        return process;
    }

    public static bool isMod
    {
        get
        {
            return false;
#if !UNITY_IOS
            var saveData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            if (!HttpCache.Instance.IsBlackOrWhite(1, saveData.w) && HttpCache.Instance.IsBlackOrWhite(0, saveData.c)) return true;
            return false;
#else
            return false;
#endif
        }
    }
    
    public static string serverUid
    {
        get { return DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0].playerId; }
    }
    
    public static string token
    {
        get { return DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0].token; }
    }



    public static void RecordLogin()
    {
        LoginCount++;
        DateTime lastLogin = LoginTime;
        DateTime nowLogin = TimeHelper.now;
        LoginTime = nowLogin;
        bool isNewDay = nowLogin.IsNewDay(lastLogin);
        TimeSpan temp = nowLogin - lastLogin;
        if (isNewDay)
        {
            if (temp.TotalDays <= 1)
            {
                ContinueLoginDay++;
            }
            else
            {
                ContinueLoginDay = 1;
            }

            LoginDay++;
            if (LoginDay == 3)
            {
                AnalyticsEvent.SendEvent(AnalyticsType.ThreeDay, null);
            }
        }

        onLogin?.Invoke(isNewDay, temp);
    }
}
