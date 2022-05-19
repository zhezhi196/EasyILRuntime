namespace Module
{
    public interface IAgentAudioObject : IAgentObject, ITarget
    {
        AgentAudio GetAudio(string name);
        float GetAudioVolume(AgentAudio tar);
        bool GetAudioActive(AgentAudio tar);
    }
}