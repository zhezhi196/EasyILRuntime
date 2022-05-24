using Module;

namespace StationMachine
{
    public class Jump : StateCell
    {
        public Jump(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("jump");
        }
        public override void Stop(bool completge)
        {
        }
    }
}