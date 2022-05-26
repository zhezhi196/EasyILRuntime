using Module;
using UnityEngine;

public class NormalTrigger: PropsBase
{
    public BoxCollider collider;

    public override string tips
    {
        get { return null; }
    }

    public override int ActiveLayer
    {
        get { return LayerMask.NameToLayer("Ignore Raycast"); }
    }

    public override string interactiveTips
    {
        get { return null; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canInteractive && other.gameObject.layer == MaskLayer.Playerlayer)
        {
            Interactive();
            DestroyWhileUnActive();
        }
    }
}