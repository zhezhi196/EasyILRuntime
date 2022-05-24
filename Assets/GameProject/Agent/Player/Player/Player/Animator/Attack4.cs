using Module;

namespace StationMachine
{
    public class Attack4: StateCell
    {
        public override void Play()
        {
            animator.SetTrigger("attack");
            animator.SetInteger("attackStyle", 3);
        }
        public override void Stop(bool completge)
        {
            if (!completge)
            {
                animator.SetTrigger("move");
            }
        }

        public Attack4(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }
    }
}