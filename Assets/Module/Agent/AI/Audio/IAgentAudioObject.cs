namespace Module
{
    public interface IAgentAudioObject : ITimeCtrl, ISwitchStation, IAgentObject
    {
        AgentAudio[] allAudio { get; }
        AgentAudio GetAudio(string name);
        bool AudioIsMute(AgentAudio tar);
    }
}