using System.Collections.Generic;

namespace Module
{
    public static class Track
    {
        public static List<IMoveTrack> track = new List<IMoveTrack>();

        public static void Update()
        {
            if (!track.IsNullOrEmpty())
            {
                for (int i = 0; i < track.Count; i++)
                {
                    
                }
            }
        }
    }
}