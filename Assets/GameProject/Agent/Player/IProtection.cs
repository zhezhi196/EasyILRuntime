using Module;
using UnityEngine;

public interface IProtection : ITarget,ISensorTarget
{
    void Creat(PlayerCreator creator);
    void Born();
}