using UnityEngine;

public interface IProgressOption
{
    int key { get; }
    GameObject gameObject { get; }
    ProgressOption progressOption { get; }
    bool progressIsComplete { get; }
    Vector3 GetTipsPos();
}