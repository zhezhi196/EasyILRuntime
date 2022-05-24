//
//  AdUICommunication.m
//  Unity-iPhone
//
//  Created by M.z.H on 2021/4/23.
//

#import <Foundation/Foundation.h>
#import "AdUICommunication.h"

extern "C" {
    
    void InterstitialAdChange(NSString* state){
        NSLog(@"AdUICommunication --- InterstitialAdChange  state=%@",state);
        const char* charValue = [state UTF8String];
        char* returnValue = (char*)malloc(strlen(charValue) + 1);
        strcpy(returnValue, charValue);
        UnitySendMessage("UACommunication", "OnAdStateChange",[state UTF8String]);
        
    }
    
    void RewardVideoAdChange(NSString* state){
        NSLog(@"AdUICommunication --- RewardVideoAdChange  state=%@",state);
        const char* charValue = [state UTF8String];
        char* returnValue = (char*)malloc(strlen(charValue) + 1);
        strcpy(returnValue, charValue);
        NSLog(@"AdUICommunication --- RewardVideoAdChange  returnValue=%s",returnValue);
        NSLog(@"AdUICommunication --- RewardVideoAdChange  charValue=%s",charValue);
        UnitySendMessage("UACommunication", "OnAdStateChange",returnValue);
    }
    
  
}
