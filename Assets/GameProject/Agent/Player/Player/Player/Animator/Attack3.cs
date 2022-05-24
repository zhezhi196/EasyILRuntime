using Module;

namespace StationMachine
{
    public class Attack3: StateCell
    {
        public override void Play()
        {
            animator.SetTrigger("attack");
            animator.SetInteger("attackStyle", 2);
        }
        public override void Stop(bool completge)
        {
            if (!completge)
            {
                animator.SetTrigger("move");
            }
        }

        public Attack3(int layer,string firstStateName) : base(layer, firstStateName)
        {
        }
    }
}