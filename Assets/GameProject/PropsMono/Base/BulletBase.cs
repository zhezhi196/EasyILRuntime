using Module;

public class BulletBase: InteractiveToBag
{
    public override InterActiveStyle interactiveStyle
    {
        get { return isMaxCount ? InterActiveStyle.noPickUp : InterActiveStyle.Handle; }
    }

    protected override bool OnInteractive(bool fromMonster = false)
    {
        if (isMaxCount)
        {
            return false;
        }
        else
        {
            return base.OnInteractive();
        }
    }


    public bool isMaxCount
    {
        get
        {
            BulletEntity tempEntity = (BulletEntity) entity;
            return tempEntity.bagCount >= tempEntity.maxCount;
        }
    }
}