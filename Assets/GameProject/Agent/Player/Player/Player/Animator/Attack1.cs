using Module;

namespace StationMachine
{
    public class Attack1 : StateCell
    {
        public Attack1(int layer, string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("attack");
            animator.SetInteger("attackStyle", 0);
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