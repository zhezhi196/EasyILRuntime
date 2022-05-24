//
//  AnalyticsAdjust.m
//  Unity-iPhone
//
//  Created by M.z.H on 2020/7/8.
//

#import <Foundation/Foundation.h>
#import "AnalyticsAdjust.h"
#import <Adjust/Adjust.h>

extern "C"{
void AnalyticsAdjustInit(){
    NSString *yourAppToken = @"bgeaj12meh34";
    NSString *environment = ADJEnvironmentProduction; // 正式环境
    // NSString *envitonment = ADJEnvironmentSandbox; // 测试环境
    ADJConfig *adjustCofig = [ADJConfig configWithAppToken:yourAppToken
                                               environment:environment
                                     allowSuppressLogLevel:YES];
    [Adjust appDidLaunch:adjustCofig];
}

void AnalyticsAdjustOnEnent(char* eventName){
    NSString* eventNameStr = [NSString stringWithUTF8String:eventName];
    
    if ([eventNameStr isEqualToString:@"tcuz9z"]) {
        eventNameStr = @"r9wdmt";
    }
    
    if ([eventNameStr isEqualToString:@"lh0k06"]) {
        eventNameStr = @"wjtr6v";
    }
    if ([eventNameStr isEqualToString:@"lrpwm8"]) {
        eventNameStr = @"nict3d";
    }
    if ([eventNameStr isEqualToString:@"t4uqk2"]) {
        eventNameStr = @"cl9in7";
    }
    // 创建plist文件  adjustData
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *plistPath = [paths objectAtIndex:0];
    NSString *fileName = [plistPath stringByAppendingPathComponent:@"adjustData.plist"];

    // 读取数据
    NSDictionary * dic = [NSDictionary dictionaryWithContentsOfFile:fileName];
    if ([eventNameStr isEqualToString:[dic objectForKey:@"wjtr6v"]]) {
        NSLog(@"Analytics  --- OnEnent_Adjust 去重 = %@",[dic objectForKey:@"wjtr6v"]);
        return;
    }
    if ([eventNameStr isEqualToString:@"wjtr6v"]) {
        // 写入数据
        NSLog(@"Analytics  --- OnEnent_Adjust 写入数据");
        NSDictionary *dic = @{@"wjtr6v":eventNameStr};
        [dic writeToFile:fileName atomically:YES];
    }
    
    NSLog(@"Analytics  --- OnEnent_Adjust   eventNameStr = %@",eventNameStr);
    ADJEvent *event = [ADJEvent eventWithEventToken:eventNameStr];
    [Adjust trackEvent:event];
}

}


