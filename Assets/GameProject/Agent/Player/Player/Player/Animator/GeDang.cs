using Module;

namespace StationMachine
{
    public class GeDang : StateCell
    {
        public GeDang(int layer, string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("gedang");
            animator.SetInteger("posture", 1);
        }

        public override void Stop(bool completge)
        {
        }
    }
}