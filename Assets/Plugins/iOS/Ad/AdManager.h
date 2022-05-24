//
//  AdManager.h
//  SDKFrame
//
//  Created by M.z.H on 2020/5/13.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#ifndef AdManager_h
#define AdManager_h

extern "C"{
void Apple_InitAdSDK();
bool Apple_InterstitialAdReady();
void Apple_ShowInterstitialAd();
bool Apple_RewardedVideoAdReady();
void Apple_ShowRewardedVideoAd();
void Apple_EntryRewardVideoAdScene();


}

#endif /* AdManager_h */
