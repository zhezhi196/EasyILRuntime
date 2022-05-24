//
//  AnalyticsManager.h
//  Unity-iPhone
//
//  Created by M.z.H on 2021/3/30.
//

#ifndef AnalyticsManager_h
#define AnalyticsManager_h

extern "C"{

void Apple_AnalyticsInit();
void Apple_OnEvent(char* eventName,char* analyticsType);

}
#endif /* AnalyticsManager_h */
