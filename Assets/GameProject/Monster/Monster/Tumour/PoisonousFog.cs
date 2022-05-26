using Module;
using UnityEngine;

//王浩2022年5月9号说 毒雾释放为瞬间释放,释放完后肉瘤可以随意走动
[SkillDescript("肉瘤/毒雾")]
public class PoisonousFog : RemoteAttack
{
    public Tumour tumour
    {
        get
        {
            return (Tumour) owner;
        }
    }

    public override void OnReleaseGo(AnimationEvent @event, int index)
    {
        EffectPlay.Play("rouliu_duwu01", tumour.fogFirePoint.transform);
        AssetLoad.LoadGameObject<Poisionous>("Monster/Tumour/Poisionous.prefab", null, (pos, arg) =>
        {
            if (pos != null)
            {
                pos.transform.position = tumour.animator.transform.position;
                pos.Fire(monster.transform.forward, this);
            }
        });
    }

    protected override string[] animation { get; } = {"tumour@duwu"};
}