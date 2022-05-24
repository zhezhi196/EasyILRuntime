//
//  AdController.h
//  Unity-iPhone
//
//  Created by M.z.H on 2021/4/23.
//

#ifndef AdController_h
#define AdController_h

#import "AdControllerBase.h"
#import <TradPlusAds/MsRewardedVideoAd.h>

@interface AdController : AdControllerBase<MsRewardedVideoAdDelegate>

@property (strong,nonatomic)MsRewardedVideoAd *rewardedVideoAd;

@end


#endif /* AdController_h */
