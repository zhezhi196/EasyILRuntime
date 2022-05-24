using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

public enum MonsterName
{
    Rat
}

public class MonsterCreator : MonoBehaviour, Identify<int>
{
    [SerializeField]
    private int id;
    public MonsterName monsterModel;
    
    public int ID
    {
        get => id;
        set => id = value;
    }

    public void LoadMonster(Action<Monster> callback)
    {
        AssetLoad.LoadGameObject<Monster>($"Agent/Monster/{monsterModel}/{monsterModel}.prefab", null,
            (monster, args) =>
            {
                monster.OnCreat(this);
                callback?.Invoke(monster);
            });
    }
}