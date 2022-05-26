using UnityEngine;

public interface InteractiveObject
{
    bool isActive { get; }
    bool canInteractive { get; }
    InterActiveStyle interactiveStyle { get; }
    bool isButtonActive { get; }
    string tips { get; }
    string interactiveTips { get; }
    GameObject gameObject { get; }
    bool Interactive(bool fromMonster = false);
}