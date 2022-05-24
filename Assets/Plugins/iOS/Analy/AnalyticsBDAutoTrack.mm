//
//  AnalyticsBDAutoTrack.m
//  Unity-iPhone
//
//  Created by M.z.H on 2021/7/19.
//

#import <Foundation/Foundation.h>
#import "AnalyticsBDAutoTrack.h"
#import <RangersAppLog/BDAutoTrack.h>
#import <RangersAppLog/BDAutoTrackURLHostItemCN.h>
#import <RangersAppLog/BDAutoTrackSchemeHandler.h>

extern "C" {
NSString *appId = @"243547";

void AnalyticsBDAutoTrackInit(NSDictionary *launchOptions){
    NSLog(@"AnalyticsBDAutoTrack --- AnalyticsBDAutoTrackInit");
    BDAutoTrackConfig *config= [BDAutoTrackConfig configWithAppID:appId launchOptions:launchOptions]; // 初始化开始
    config.serviceVendor = BDAutoTrackServiceVendorCN; // 数据上报
    config.appName = @"无尽噩梦：诡医院"; // 和申请APPID时的app_name一致
    config.appName = @"EN2_douyin_ios"; // iOS一般默认App Store
    config.showDebugLog = NO; // 是否开启日志
    config.logNeedEncrypt = YES; // 是的日志加密，默认加密
    config.gameModeEnable = YES; // 是否开启游戏模式，游戏APP建议设置为YES
    [BDAutoTrack startTrackWithConfig:config]; // 初始化结束
    
    
}

}

