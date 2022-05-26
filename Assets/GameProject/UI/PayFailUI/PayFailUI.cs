using Module;

namespace UnityEngine.UI
{
    public class PayFailUI : UIViewBase
    {
        public Text des;
        public UIBtnBase close;

        protected override void OnChildStart()
        {
            base.OnChildStart();
            close.AddListener(OnClose);
        }

        public override void Refresh(params object[] args)
        {
            des.text = args[0].ToString();
        }

        private void OnClose()
        {
            OnExit();
        }
    }
}