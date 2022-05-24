public enum OutGameStation
{
    Win, //通关推出
    Fail, //失败退出 
    Break, //手动退出
}
public interface IBattleProcedure
{
    void BattlePrepare();
    void StartBattle(Mission mission, MissionGraph editorGraph);
    void RestartBattle(MissionGraph editorGraph);
    void OnNodeEnter(TaskNode node);
    void OnNodeExit(TaskNode node);
    void Fight();
    void OnTaskResult(bool result);
    void ExitBattle(OutGameStation outGame);
    void OnUpdate();
}