using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Module
{
    public class AudioInfo
    {
        public string name;
        public string path;
        public AudioClip clip;

        public AudioInfo(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        public AudioInfo(AudioClip clip)
        {
            this.clip = clip;
        }
    }
}