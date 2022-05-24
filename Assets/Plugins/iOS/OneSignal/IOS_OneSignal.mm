//
//  IOS_OneSignal.m
//  Unity-iPhone
//
//  Created by M.z.H on 2020/9/9.
//

#import <Foundation/Foundation.h>
#import "IOS_OneSignal.h"
#import <OneSignal/OneSignal.h>

extern "C"{
// 初始化
void Apple_OneSignalInit(NSDictionary* launchOptions){
    NSLog(@"IOS_OneSignal --- Apple_OneSignalInit");
//    [OneSignal initWithLaunchOptions:launchOptions];
//    [OneSignal setAppId:@"5df467b3-db38-4ac9-bfb9-91b5bc0febcb"];
//    [OneSignal promptForPushNotificationsWithUserResponse:^(BOOL accepted) {
//        NSLog(@"IOS_OneSignal --- OneSignal_Init  accepted : %d", accepted);
//        NSString* userId = [OneSignal getDeviceState].userId;
//        NSLog(@"IOS_OneSignal --- OneSignal_Init userid=%@", userId);
//    }];
}
// 发送通知
void Apple_PostNotification(const char* language, const char* message, const char* title, long delay){
    NSLog(@"IOS_OneSignal --- Apple_PostNotification");
    NSString* userId = [OneSignal getDeviceState].userId;
    if(nil == userId) {
        NSLog(@"IOS_OneSignal --- Apple_PostNotification Error: userid is null");
        return;
    }
    
    NSDate* curDate = [NSDate date];
  
    NSTimeZone* timeZone = [NSTimeZone defaultTimeZone];
    NSString* sendAfter;
  
    delay = delay;
    NSDate* sendDate = [[NSDate alloc] initWithTimeIntervalSinceNow:delay];
    NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
    [dateFormat setTimeZone:timeZone];
    [dateFormat setDateFormat:@"yyyy-MM-dd HH:mm:ss 'GMT'Z"];
    sendAfter = [dateFormat stringFromDate:sendDate];
    NSLog(@"IOS_OneSignal --- Apple_PostNotification  current: %@ delay:%ld", [dateFormat stringFromDate:curDate], delay);
    NSLog(@"IOS_OneSignal --- Apple_PostNotification  sendAfter: %@", sendAfter);
    
    NSDictionary* content = @{
        @"contents" : @{[NSString stringWithUTF8String:language]: [NSString stringWithUTF8String:message]},
        @"headings" : @{[NSString stringWithUTF8String:language]: [NSString stringWithUTF8String:title]},
        @"include_player_ids": @[[OneSignal getDeviceState].userId],
        @"send_after":sendAfter,};
    
    
    NSLog(@"IOS_OneSignal --- Apple_PostNotification  content:%@", content);
    [OneSignal postNotification: content onSuccess:^(NSDictionary* result){
        NSError* parseError = nil;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:&parseError];
        
        if(nil == parseError) {
            NSLog(@"IOS_OneSignal --- Apple_PostNotification  Send Suc:%@", [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding]);
        } else {
            NSLog(@"IOS_OneSignal --- Apple_PostNotification  Send Suc but format error:%@", parseError.localizedDescription);
        }
    } onFailure:^(NSError *error) {
        NSLog(@"IOS_OneSignal --- Apple_PostNotification   Pos Notification Error:%@", error.localizedDescription);
    }];
    
}
// 取消通知
void Apple_CancelNotification(){
}



}
