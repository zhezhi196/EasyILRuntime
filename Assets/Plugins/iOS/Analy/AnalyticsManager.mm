//
//  AnalyticsManager.m
//  Unity-iPhone
//
//  Created by M.z.H on 2021/3/30.
//

#import <Foundation/Foundation.h>
#import "AnalyticsManager.h"
#import "AnalyticsAdjust.h"
#import "AnalyticsGA.h"


extern "C"{

void Apple_AnalyticsInit(){
    AnalyticsGAInit();
    AnalyticsAdjustInit();
}

void Apple_OnEvent(char* eventName,char* analyticsType){
    NSString *analyStrType = [NSString stringWithUTF8String:analyticsType];
    NSLog(@"AnalyticsManager --- Apple_OnEvent  eventName = %s,analyStrType = %@",eventName,analyStrType);
    if ([analyStrType isEqualToString:@"GameAnalytics"]) {
        AnalyticsGAOnEvent(eventName);
    }
    if ([analyStrType isEqualToString:@"Adjust"]) {
        AnalyticsAdjustOnEnent(eventName);
    }
}

}
