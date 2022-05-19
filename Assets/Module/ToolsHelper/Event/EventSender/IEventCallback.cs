namespace Module
{
    public interface IEventCallback
    {
        void EventCallback(int eventID,IEventCallback receiver);
    }
}