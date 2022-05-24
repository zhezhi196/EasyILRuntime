//
//  CommonUtil.m
//  SDKFrame
//
//  Created by M.z.H on 2020/5/15.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CommonUtil.h"
#import <StoreKit/StoreKit.h>
#import <sys/utsname.h>

extern "C" {

// 获取版本号
char* Apple_GetVersionName(){
    
    NSDictionary *infoDictionary = [[NSBundle mainBundle]infoDictionary];
    NSString *app_Version =[infoDictionary objectForKey:@"CFBundleShortVersionString"];
    char* charValue = (char *)[app_Version UTF8String];
    return charValue;
}

// 跳转到应用商店详情页（评论）
void Apple_OpenAPPMarket(){
    NSString *version = [UIDevice currentDevice].systemVersion;
    NSString *appId = @"2M3Q2T5ZF3";
    
    if (version.doubleValue >= 10.3){
        // 防止键盘遮挡
        
        [[UIApplication sharedApplication].keyWindow endEditing:YES];
        [SKStoreReviewController requestReview];
    }else{
        NSString *appURL = [NSString stringWithFormat:@"itms-apps://itunes.apple.com/app/id%@?action=write-review",appId];
        NSURL *url = [NSURL URLWithString:appURL];
        [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        
        UnitySendMessage("UACommunication", "GoToMarketCallBack",[@"true" UTF8String]);
    }
}




// 显示隐私政策
void Apple_ShowPrivacyPolicy(){
    NSLog(@"显示隐私政策 --------- IOS");
    NSURL *url = [NSURL URLWithString:@"http://www.yunbu.me/service/mingya/privacy.html"];
    [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
}

bool Apple_IsSDKLog(){
    return true;
}

char* GetPlatformInfo(){
    
    NSMutableDictionary* platformInfo = [[NSMutableDictionary alloc] initWithCapacity:5];
    
    NSString *deviceName = [[UIDevice currentDevice] name];  //获取设备名称 例如：梓辰的手机
    NSString *sysName = [[UIDevice currentDevice] systemName]; //获取系统名称 例如：iPhone OS
    NSString *sysVersion = [[UIDevice currentDevice] systemVersion]; //获取系统版本 例如：9.2
    NSString *deviceUUID = [[[UIDevice currentDevice] identifierForVendor] UUIDString]; //获取设备唯一标识符 例如：FBF2306E-A0D8-4F4B-BDED-9333B627D3E6
    NSString *deviceModel = [[UIDevice currentDevice] model]; //获取设备的型号 例如：iPhone
    struct utsname systemInfo;
    uname(&systemInfo);
    NSString *deviceModel2 = [NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding];
    NSLocale *local = [NSLocale currentLocale];
    NSString* region = [local localeIdentifier];
    
    [platformInfo setObject:deviceModel2 forKey:@"model"];
    [platformInfo setObject:deviceModel forKey:@"manufacturer"];
    [platformInfo setObject:deviceName forKey:@"deviceName"];
    [platformInfo setObject:sysVersion forKey:@"osVersion"];
    [platformInfo setObject:sysName forKey:@"osName"];
    [platformInfo setObject:deviceUUID forKey:@"imei"];
    [platformInfo setObject:region forKey:@"region"];
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:platformInfo options:NSJSONWritingPrettyPrinted error:&error];
    NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    NSLog(@"GetPlatform===================");
    NSLog(@"%@", json);
    
    const char* charValue = [json UTF8String];
    char* returnValue = (char*)malloc(strlen(charValue) + 1);
    strcpy(returnValue, charValue);
    return returnValue;
}

char* GetServerInfo(){
    NSMutableDictionary* platformInfo = [[NSMutableDictionary alloc] initWithCapacity:5];
    
    [platformInfo setObject:@"35.163.131.168" forKey:@"server_ip"]; // 正式地址
//   [platformInfo setObject:@"47.92.89.131" forKey:@"server_ip"]; // 测试地址
//   [platformInfo setObject:@"192.168.0.38" forKey:@"server_ip"]; // 测试地址
    [platformInfo setObject:@"3563" forKey:@"server_port"];
    [platformInfo setObject:@"35.163.131.168" forKey:@"cdn_url"]; // 正式地址
//    [platformInfo setObject:@"47.92.89.131" forKey:@"cdn_url"]; // 测试地址
//     [platformInfo setObject:@"192.168.0.38" forKey:@"cdn_url"]; // 测试地址
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:platformInfo options:NSJSONWritingPrettyPrinted error:&error];
    NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    NSLog(@"GetServerInfo===================");
    NSLog(json);
    
    const char* charValue = [json UTF8String];
    char* returnValue = (char*)malloc(strlen(charValue) + 1);
    strcpy(returnValue, charValue);
   return returnValue;

}

void InstantiateFeedbackGenerators(){}
void ReleaseFeedbackGenerators(){}
void SelectionHaptic(){}
void SuccessHaptic(){}
void WarningHaptic(){}
void FailureHaptic(){}
void LightImpactHaptic(){}
void MediumImpactHaptic(){}
void HeavyImpactHaptic(){}

}
