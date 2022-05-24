using Module;

namespace StationMachine
{
    public class Attack2 : StateCell
    {
        public Attack2(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }

        public override void Play()
        {
            animator.SetTrigger("attack");
            animator.SetInteger("attackStyle", 1);
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