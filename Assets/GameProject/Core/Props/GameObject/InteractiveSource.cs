using UnityEngine;

public interface InteractiveSource
{
    Camera evCamera { get; }
    Transform eyePoint { get; }
}