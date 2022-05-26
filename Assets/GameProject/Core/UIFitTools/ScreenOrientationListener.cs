using UnityEngine;
using Module;

public class ScreenOrientationListener : Singleton<ScreenOrientationListener>
{
    ScreenOrientation last;

    public void CheckScreenOrientation()
    {
        if (Screen.orientation != last)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                last = ScreenOrientation.LandscapeLeft;
                EventCenter.Dispatch(EventKey.ScreenOrientationChange, ScreenOrientation.LandscapeLeft);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                last = ScreenOrientation.LandscapeRight;
                EventCenter.Dispatch(EventKey.ScreenOrientationChange, ScreenOrientation.LandscapeRight);
            }
        }
    }

}
