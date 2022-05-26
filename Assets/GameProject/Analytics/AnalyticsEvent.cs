using Module;

public class AnalyticsEvent
{
    public static void SendEvent(AnalyticsType type, string targetId, bool mission = true)
    {
        if (mission)
        {
            int missionid = BattleController.Instance.missionId;
            Analytics.SendEvent((int) type, targetId, missionid);
        }
        else
        {
            Analytics.SendEvent((int) type, targetId, 0);
        }
        //GameDebug.LogFormat("尝试打点{0} targetid={1}", type, targetId);
    }
}