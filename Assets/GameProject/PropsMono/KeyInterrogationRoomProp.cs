/// <summary>
/// 审讯室钥匙
/// </summary>
public class KeyInterrogationRoomProp : InteractiveToBag
{
    public override bool progressIsComplete
    {
        get
        {
            return creator.isGet || PropEntity.GetEntity(20012).isGet;
        }
    }
}