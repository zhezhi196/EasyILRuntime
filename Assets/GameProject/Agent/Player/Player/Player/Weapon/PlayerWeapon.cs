using System;
using Module;
using Project.Data;
using UnityEngine;

public abstract class PlayerWeapon: ScriptableObject
{
    public int id;
    public AgentType agentType;
    public Skill normalAttack;
    public WeaponData dbData;
    public GameAttribute attribute;

    public void Init()
    {
        if (this.dbData == null)
        {
            this.dbData = DataInit.Instance.GetSqlService<WeaponData>().WhereID(id);
            attribute = AttributeHelper.GetAttributeByType(dbData)[0];
        }
    }

    public static void CreatWeapon(int weapon, Action<PlayerWeapon> callback)
    {
        AssetLoad.PreloadAsset<PlayerWeapon>($"Agent/Player/Weapon/{weapon}.asset", res =>
        {
            callback?.Invoke(res.Result);
        });
    }
}