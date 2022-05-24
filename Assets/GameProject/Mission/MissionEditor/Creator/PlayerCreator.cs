using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum PlayerType
{
    Player,
    HumanBase,
    Aide,
    Base1,
    Base2,
}
public class PlayerCreator : MonoBehaviour
{
    public PlayerType playerType;
    private void OnDrawGizmos()
    {
        if (playerType == PlayerType.Player)
        {
            Gizmos.color = Color.green;
        }
        else if (playerType == PlayerType.Aide)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = new Color(0.7735849f, 0.2736739f, 0.609565f, 1);
        }
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
