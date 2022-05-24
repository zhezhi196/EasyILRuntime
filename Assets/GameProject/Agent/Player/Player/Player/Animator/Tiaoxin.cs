using Module;

namespace StationMachine
{
    public class Tiaoxin : StateCell
    {
        public Tiaoxin(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("tiaoxin");
        }
        public override void Stop(bool completge)
        {
            if (!completge)
            {
                animator.SetTrigger("move");
            }
        }
    }
}