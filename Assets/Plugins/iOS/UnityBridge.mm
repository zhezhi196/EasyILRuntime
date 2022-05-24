
//
//  IOS_Umeng.mm
//  Unity-iPhone
//
//  Created by chenguan on 2018/12/11.
//


#import <Foundation/Foundation.h>
#import "UnityBridge.h"
#import "Ad/AdManager.h"
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <UIKit/UIKit.h>
#import <Firebase/Firebase.h>
#import "AnalyticsBDAutoTrack.h"

float currentBrightness; // 当前屏幕亮度（记录进入游戏前的亮度）

extern "C" {

void unityBridge_didFinishLaunchingWithOptions(NSDictionary *launchOptions) {
   // currentBrightness = [[UIScreen mainScreen]brightness];
   // NSLog(@"获取屏幕亮度 currentBrightness = %f",currentBrightness);
    
    // 使用Firebase配置API
    [FIRApp configure];
    
    SetScreenBrightness();
    if (@available(iOS 14,*)) {
        // IOS 14
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
        }];
    }
    
    // 火山引擎
    AnalyticsBDAutoTrackInit(launchOptions);

}

// 设置屏幕亮度
void SetScreenBrightness(){
  //  [[UIScreen mainScreen] setBrightness:1.0];
    // 设置屏幕常亮
  //  [[UIApplication sharedApplication]setIdleTimerDisabled:YES];
}

// 恢复当前屏幕亮度
void RegaincurrentBrightness(){
   // [[UIScreen mainScreen] setBrightness:currentBrightness];
  //  [[UIApplication sharedApplication]setIdleTimerDisabled:NO];
}

}

