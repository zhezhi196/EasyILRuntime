namespace Module
{
    public interface IEventCallback
    {
        void EventCallback(uint eventID,IEventCallback receiver);
    }
}