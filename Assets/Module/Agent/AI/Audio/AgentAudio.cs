using UnityEngine;

namespace Module
{
    public class AgentAudio : Audio
    {
        [SerializeField]
        private string _key;

        public string key
        {
            get { return _key; }
        }
    }
}