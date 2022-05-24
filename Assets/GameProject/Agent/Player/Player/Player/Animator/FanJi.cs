using Module;

namespace StationMachine
{
    public class FanJi : StateCell
    {
        public FanJi(int layer, string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetInteger("posture", 2);
        }

        public override void Stop(bool completge)
        {
        }
    }
}