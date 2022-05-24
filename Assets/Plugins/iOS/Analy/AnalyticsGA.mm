//
//  AnalyticsGA.m
//  Unity-iPhone
//
//  Created by M.z.H on 2020/8/27.
//

#import <Foundation/Foundation.h>
#import <GameAnalytics/GameAnalytics.h>
#import "AnalyticsGA.h"
#import "CommonUtil.h"

extern "C" {

void AnalyticsGAInit(){
    NSString* app_Version = [NSString stringWithUTF8String:Apple_GetVersionName()];
    NSLog(@"AnalyticsGA ---  AnalyticsGAInit  app_Version = %@",app_Version);
    [GameAnalytics configureBuild:app_Version];
    // 开启日志
    [GameAnalytics setEnabledInfoLog:YES];
    // 初始化
    [GameAnalytics initializeWithGameKey:@"2ae98971748ca97b9f261777a4727767" gameSecret:@"a5928644693ea96b812396b209c91bd9a8ca0c09"];
}

void AnalyticsGAOnEvent(char* eventName){
    NSLog(@"AnalyticsGA ---  AnalyticsGAOnEvent  eventName = %s",eventName);
    [GameAnalytics addDesignEventWithEventId:[NSString stringWithUTF8String:eventName]];
}

}
