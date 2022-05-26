using Module;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameProject.Core.Monster
{
    public class ResetPuppMuster: MonoBehaviour
    {
        public PuppetMaster master;
        public Transform ori;
        public Transform mirror;
        [Button]
        public void Set()
        {
            master = transform.parent.GetComponentInChildren<PuppetMaster>();
            ori = transform.GetChild(0);
            mirror = transform.GetChild(1);
            
            for (int i = 0; i < master.muscles.Length; i++)
            {
                var path = master.muscles[i].target.GetPathInScene(ori);
                master.muscles[i].target = mirror.Find(path);
            }

            DestroyImmediate(this);
        }
    }
}