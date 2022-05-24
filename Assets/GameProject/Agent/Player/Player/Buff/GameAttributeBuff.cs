using Module;

namespace GameBuff
{
    public class GameAttributeBuff : PlayerBuff
    {
        public GameAttribute Attribute;

        public override string name
        {
            get { return "Attribute"; }
        }
        

        public override void OnInit(IBuffObject owner, BuffType type, object[] args)
        {
            base.OnInit(owner, type, args);
            Attribute = (GameAttribute) args[0];
        }
        
    }
}