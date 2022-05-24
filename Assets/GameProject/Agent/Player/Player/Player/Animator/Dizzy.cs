using Module;

namespace StationMachine
{
    public class Dizzy: StateCell
    {
        public Dizzy(int layer, string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("dizzy");
        }

        public override void Stop(bool completge)
        {
        }
    }
}