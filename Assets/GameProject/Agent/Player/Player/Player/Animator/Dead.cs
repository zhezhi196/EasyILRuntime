using Module;

namespace StationMachine
{
    public class Dead: StateCell
    {
        public Dead(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetBool("isDead", true);
        }

        public override void Stop(bool completge)
        {
        }
    }
}