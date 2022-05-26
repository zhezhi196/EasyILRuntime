using Project.Data;
namespace GameGift
{
    /// <summary>
    /// 羽翼射手
    /// 手枪双枪
    /// </summary>
    public class DoublePistol : Gift
    {
        public DoublePistol(GiftData data, GiftSaveData saveData) : base(data, saveData)
        {
        }

        public override void ActivateGife()
        {
            Weapon weapon = Player.player?.weaponManager.FindWeapon(WeaponType.HandGun);
            //设置武器双枪
            if (weapon != null)
            {
                (weapon as WeaponHandgun).ChangeToDouble();
            }
        }
    }
}
