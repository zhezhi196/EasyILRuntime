using Module;

namespace HotFix
{
    public static partial class UIConfig
    {
	    public static UIType UIMain = new UIType(typeof(UIMainCtrl), typeof(UIMainModul), typeof(UIMainView),"UIMain");
	}
}
