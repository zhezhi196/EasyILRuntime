using Module;

namespace Module
{
    public static class SystemUpdate
    {
        public static void Update()
        {
            Setting.Update();
            UIController.Instance.Update();
            Async.Update();
            Clock.Update();
            LocalSaveFile.Update();
            QueueRunMethod.Update();
            PredicateCallback.Update();
            TimeHelper.Update();
        }
    }
}
