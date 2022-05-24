//
//  CommonUtil.h
//  SDKFrame
//
//  Created by M.z.H on 2020/5/15.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#ifndef CommonUtil_h
#define CommonUtil_h

extern "C"{

char* Apple_GetVersionName();
void Apple_OpenAPPMarket();
void Apple_ShowPrivacyPolicy();
bool Apple_IsSDKLog();
char* GetPlatformInfo();
char* GetServerInfo();



void InstantiateFeedbackGenerators();
void ReleaseFeedbackGenerators();
void SelectionHaptic();
void SuccessHaptic();
void WarningHaptic();
void FailureHaptic();
void LightImpactHaptic();
void MediumImpactHaptic();
void HeavyImpactHaptic();

}

#endif /* CommonUtil_h */
