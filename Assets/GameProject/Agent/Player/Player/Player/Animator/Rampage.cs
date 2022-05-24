using Module;

namespace StationMachine
{
    public class Rampage: StateCell
    {
        public Rampage(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("rampage");
        }

        public override void Stop(bool completge)
        {
        }
    }
}