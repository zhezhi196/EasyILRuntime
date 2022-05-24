using Module;

namespace StationMachine
{
    public class Transform:  StateCell
    {
        public Transform(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("transform");
        }
        public override void Stop(bool completge)
        {
        }
    }
}