//
//  UnityBridge.h
//  Unity-iPhone
//
//  Created by chenguan on 2018/12/11.
//

#ifndef UnityBridge_h
#define UnityBridge_h


extern "C" {
void unityBridge_didFinishLaunchingWithOptions(NSDictionary *launchOptions);
void SetScreenBrightness();// 设置屏幕亮度
void RegaincurrentBrightness();// 恢复当前屏幕亮度

}


#endif /* UnityBridge_h */
