using UnityEngine;

namespace Module
{
    public interface IMoveCtrl
    {
        MovePlay MoveTo(Vector3 position);
    }
}