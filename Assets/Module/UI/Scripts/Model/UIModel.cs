using System;

namespace Module
{
    public enum UIOpenDirection
    {
        Forward,
        Backward,
        Stay
    }
    [Serializable]
    public class UIModel
    {
        public UITweenType tweenType;
        public OpenFlag flag;
        public bool isPopup;
        public object[] args;
        public float time;
        public UIObject lastUI;
        public int visitCount;
        public UIOpenDirection direction;

        public void UpdateModel()
        {
            time += TimeHelper.unscaledDeltaTimeIgnorePause;
        }

        public void RefreshModel(bool reset, UITweenType tweentype, OpenFlag flag, bool ispopup,object[] args,UIOpenDirection dir)
        {
            this.tweenType = tweentype;
            this.flag = flag;
            this.isPopup = ispopup;
            this.args = args;
            if (reset) time = 0;
            this.direction = dir;
            visitCount++;
        }

        public static UITweenType Invert(UITweenType tweentype)
        {
            switch (tweentype)
            {
                case UITweenType.Down:
                    return UITweenType.Up;
                case UITweenType.Left:
                    return UITweenType.Right;
                case UITweenType.Right:
                    return UITweenType.Left;
                case UITweenType.Up:
                    return UITweenType.Down;
            }

            return tweentype;
        }
    }
}