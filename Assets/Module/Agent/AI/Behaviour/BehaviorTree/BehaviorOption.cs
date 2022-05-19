namespace Module
{
    public struct BehaviorOption
    {
        public string behaviorName;
        public object[] behaviorArg;

        public BehaviorOption(string name, object[] args)
        {
            this.behaviorName = name;
            this.behaviorArg = args;
        }
    }
}