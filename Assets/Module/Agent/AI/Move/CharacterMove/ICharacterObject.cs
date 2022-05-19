using UnityEngine;

namespace Module
{
    public interface ICharacterObject : IMoveObject
    {
        CharacterController character { get; }
    }
}