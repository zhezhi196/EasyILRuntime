using System;
using Module;
using UnityEngine;

public enum CollectionStation
{
    /// <summary>
    /// 未解锁
    /// </summary>
    locked,

    /// <summary>
    /// 为获取
    /// </summary>
    UnGet,

    /// <summary>
    /// 已获取
    /// </summary>
    Get,

    /// <summary>
    /// 刚获取
    /// </summary>
    NewGet
}

public enum CollectionType
{
    /// <summary>
    /// 情报
    /// </summary>
    Prop,
    /// <summary>
    /// 素材
    /// </summary>
    Resources,
    /// <summary>
    /// 武器
    /// </summary>
    Weapon,
}

public interface ICollection : IModelObject
{
    bool isGet { get; }

    /// <summary>
    /// 这个道具的状态
    /// </summary>
    CollectionStation collectionStation { get; set; }

    /// <summary>
    /// 这个道具的类型
    /// </summary>
    CollectionType collectionType { get; }

    /// <summary>
    /// 这个道具的数据表,数据表继承ICollectionData
    /// </summary>
    ICollectionData collectionData { get; }

    event Action<ICollection> onGetCollection;

    /// <summary>
    /// 获得这个道具不同状态的icon
    /// </summary>
    /// <param name="station"></param>
    /// <param name="callback"></param>
    void GetIcon(CollectionStation station, Action<Sprite> callback);
}