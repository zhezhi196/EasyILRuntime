
//
//  AdManager.m
//  SDKFrame
//
//  Created by M.z.H on 2020/5/13.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AdManager.h"
#import "AdControllerBase.h"
#import "AdController.h"

AdControllerBase* adController = nil;

extern "C" {


void Apple_InitAdSDK(){
    NSLog(@"AdManager --- Apple_InitAd");
    adController = [[AdController alloc]init];
    [adController InitAd];
}

bool Apple_InterstitialAdReady(){
    bool isReady = [adController InterstitialAdReady];
    NSLog(@"AdManager --- Apple_InterstitialAdReady  isReady = %d",isReady);
    return isReady;
}

void Apple_ShowInterstitialAd(){
    NSLog(@"AdManager --- Apple_ShowInterstitialAd");
    [adController ShowInterstitialAd:@""];
}

bool Apple_RewardedVideoAdReady(){
    bool isReady = [adController RewardVideoAdReady];
    NSLog(@"AdManager --- Apple_RewardedVideoAdReady  isReady = %d",isReady);
    return isReady;
}

void Apple_ShowRewardedVideoAd(){
    NSLog(@"AdManager --- Apple_ShowRewardedVideoAd");
    [adController ShowRewardVideoAd:@""];
}


void Apple_EntryRewardVideoAdScene(){
    NSLog(@"AdManager --- Apple_EntryRewardVideoAdScene");
    [adController RewardVideoAdEntryScenario];
}

}
