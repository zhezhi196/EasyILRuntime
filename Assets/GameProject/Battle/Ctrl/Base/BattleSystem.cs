public class BattleSystem : IBattleProcedure
{
    public virtual void BattlePrepare()
    {
    }

    public virtual void StartBattle(Mission mission, MissionGraph editorGraph)
    {
    }

    public virtual void RestartBattle(MissionGraph editorGraph)
    {
    }

    public virtual void OnNodeEnter(TaskNode node)
    {
    }

    public virtual void OnNodeExit(TaskNode node)
    {
    }

    public void Fight()
    {
    }

    public virtual void OnTaskResult(bool result)
    {
    }

    public virtual void ExitBattle(OutGameStation outGame)
    {
    }

    public virtual void OnUpdate()
    {
    }
}