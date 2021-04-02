namespace Module
{
    public class TimeEffect: NormalEffect
    {
        public float duation;
        public override void Simulate(float time)
        {
            if (time >= duation && duation != -1) Stop();
        }
    }
}