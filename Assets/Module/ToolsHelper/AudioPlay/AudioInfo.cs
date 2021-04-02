using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Module
{
    public class AudioInfo
    {
        public string name;
        public string path;
        public AudioPlay play;
        public AudioClip clip;

        public AudioInfo(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

    }
}