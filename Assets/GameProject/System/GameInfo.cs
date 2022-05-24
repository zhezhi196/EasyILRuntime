using System;
using Module;

public static class GameInfo
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
        var playerSaveData = DataInit.Instance.GetSqlService<PlayerSaveData>();
        if (playerSaveData.tableList.Count == 0)
        {
            playerSaveData.Insert(new PlayerSaveData());
        }
        RecordLogin();
        process.SetComplete();
        return process;
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
        }

        onLogin?.Invoke(isNewDay, temp);
    }
}