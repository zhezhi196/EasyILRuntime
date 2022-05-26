public class WeaponPrisoner: Prisoner
{
    public override string GetLayerDefaultAnimation(int layer)
    {
        if (layer == 0)
        {
            return "Normal";
        }
        else
        {
            return "Wprisoner@fightIdle";
        }
    }
}