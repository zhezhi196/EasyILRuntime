using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module
{
    public enum CharacterMoveType
    {
        World,
        Relative
    }
    [Flags]
    public enum MoveFlag
    {
        RotateToTarget = 1,
    }
    
    public class CharacterMoveCtrl: IAgentCtrl
    {
        public bool isPause { get; }
        public void OnUpdate()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void OnAgentDead()
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }

        public T GetAgentCtrl<T>() where T : IAgentCtrl
        {
            throw new NotImplementedException();
        }

        public void EditorInit()
        {
            throw new NotImplementedException();
        }

        public void OnDrawGizmos()
        {
            
        }
    }
}