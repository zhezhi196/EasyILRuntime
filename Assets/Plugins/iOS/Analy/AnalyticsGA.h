//
//  AnalyticsGA.h
//  Unity-iPhone
//
//  Created by M.z.H on 2020/8/27.
//

#ifndef AnalyticsGA_h
#define AnalyticsGA_h

extern "C" {

void AnalyticsGAInit();
void AnalyticsGAOnEvent(char* eventName);

}
#endif /* AnalyticsGA_h */
