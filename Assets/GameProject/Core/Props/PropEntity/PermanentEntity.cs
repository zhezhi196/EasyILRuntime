using Module;
using Project.Data;
/// <summary>
/// 不会删档的物品，会被保存在PlayerPrefs中
/// </summary>
public class PermanentEntity : PropEntity
{
    public PermanentEntity(PropData dbData) : base(dbData)
    {
    }
}