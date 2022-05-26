using Module;
using SQLite.Attributes;

public class TimeRewardSaveData : ISqlData
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int targetID { get; set; }
    public string targetTime { get; set; }
}

public class DailyTaskSaveData : ISqlData
{
    public string getDes { get; set; }
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    public int targetID { get; set; }
    public int targetIndex { get; set; }
    public TaskStation targetStation { get; set; }
    public int targetCurrentCount { get; set; }
    public int targetDifficulte { get; set; }
    public int targetComleteCount { get; set; }
    public int targetRewardCount { get; set; }
    public string targetRewardID { get; set; }

    public void Record(int id, DailyTaskBase taskBase)
    {
        this.ID = id;
        this.targetID = taskBase.dbData.ID;
        this.targetStation = taskBase.station;
        this.targetCurrentCount = taskBase.currentCount;

        this.targetDifficulte = taskBase.taskReward.difficulte;
        this.targetComleteCount = taskBase.taskReward.completeCount;
        this.targetRewardCount = (int) taskBase.taskReward.rewardContent.finalCount.value;
        this.targetRewardID = taskBase.taskReward.rewardId;
        this.targetIndex = taskBase.index;
        this.getDes = taskBase.taskReward.getDes;
    }

    public static DailyTaskSaveData[] GetSaveData(params DailyTaskBase[] task)
    {
        DailyTaskSaveData[] result = new DailyTaskSaveData[task.Length];

        for (int i = 0; i < task.Length; i++)
        {
            result[i] = new DailyTaskSaveData();
            result[i].Record(i + 1, task[i]);
        }

        return result;
    }
}