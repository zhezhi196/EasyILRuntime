using UnityEngine;
using UnityEngine.AI;

namespace Module
{
    public class NavmeshMoveCtrl : IMoveCtrl
    {        
        private IMoveAgent owner;
        public NavMeshAgent navMeshAgent;
        public MovePlay currPlay { get; set; }

        public NavmeshMoveCtrl(IMoveAgent owner, NavMeshAgent navMeshAgent)
        {
            this.navMeshAgent = navMeshAgent;
            this.owner = owner;
        }

        public MovePlay MoveTo(Vector3 position)
        {
            if (owner.canMove)
            {
                var target = new Vector3(position.x, navMeshAgent.transform.position.y, position.z);
                navMeshAgent.isStopped = false;
                if (currPlay != null && currPlay.isMoving)
                {
                    currPlay.SetTarget(target, null);
                    GameDebug.DrawLine(navMeshAgent.transform.position, position, Color.cyan);
                }
                else
                {
                    currPlay = new MovePlay((INavmeshMoveAgent) owner);
                    currPlay.StartCoroutine(target, () => { currPlay = null; });
                    GameDebug.DrawLine(navMeshAgent.transform.position, position, Color.cyan,1);
                }
                return currPlay;
            }

            return null;
        }
    }
}