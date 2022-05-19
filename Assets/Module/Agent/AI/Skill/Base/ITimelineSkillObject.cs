using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Module
{
    public interface ITimelineSkillObject : ISkillObject
    {
        PlayableDirector timeLinePlayer { get; }
    }
}