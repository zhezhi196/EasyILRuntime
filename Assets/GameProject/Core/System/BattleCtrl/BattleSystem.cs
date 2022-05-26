using Module;

public class BattleSystem : IBattleEvent 
{
    public virtual void BattlePrepare(EnterNodeType enterType)
    {
    }

    public virtual void StartBattle(EnterNodeType enterType)
    {
    }

    public virtual void OnRestart(EnterNodeType enterType)
    {
    }

    public virtual void OnNodeExit(NodeBase node)
    {
    }

    public virtual void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
    }

    public virtual void OnStartFight(NodeBase node)
    {
    }

    public virtual void OnPlayerDead()
    {
    }

    public virtual void OnTaskResult(bool result)
    {
    }

    public virtual void OnContinue()
    {
    }

    public virtual void OnPause()
    {
    }

    public virtual void ExitBattle(OutGameStation station)
    {
    }

    public virtual void Save()
    {
    }

    public virtual void OnUpdate()
    {
        
    }

}