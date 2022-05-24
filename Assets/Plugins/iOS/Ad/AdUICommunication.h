//
//  AdUICommunication.h
//  Unity-iPhone
//
//  Created by M.z.H on 2021/4/23.
//

#ifndef AdUICommunication_h
#define AdUICommunication_h

extern "C" {

void InterstitialAdChange(NSString* state);
void RewardVideoAdChange(NSString* state);
void BannerAdChange(NSString* state);
void SplashAdChange(NSString* state);
}

#endif /* AdUICommunication_h */
