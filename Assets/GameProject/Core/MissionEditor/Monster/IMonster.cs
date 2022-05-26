using System.Collections.Generic;
using Module;
using UnityEngine;

public interface IMonster : ISee, ISensorTarget, IStationObject<MonsterStation>,IAgentObject
{
    FightState showUiState { get; }
    GameObject gameObject { get; }
    List<MonsterLeveEditor> defaultLevels { get; set; }
    MonsterLevel currentLevel { get; }
    Transform transform { get; }
    bool isAlive { get; }
    bool isSeePlayer { get; }
    bool canAss { get; }
    FightState fightState { get; }
    InitStation initStation { get; set; }
    bool isProgressComplete { get; }
    void ResetToBorn();
    void OnCreat(MonsterCreator creator);
    void Born();

    void StopAttack();
}