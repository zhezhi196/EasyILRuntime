using System;

public interface IBattleEvent
{
    void BattlePrepare(EnterNodeType enterType);
    void StartBattle(EnterNodeType enterType);
    void OnRestart(EnterNodeType enterType);
    void OnNodeExit(NodeBase node);
    void OnNodeEnter(NodeBase node, EnterNodeType enterType);
    void OnStartFight(NodeBase node);
    void OnPlayerDead();
    void OnTaskResult(bool result);
    void OnContinue();
    void OnPause();
    void ExitBattle(OutGameStation station);
    void Save();
    void OnUpdate();
}