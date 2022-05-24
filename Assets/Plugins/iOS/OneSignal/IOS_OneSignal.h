//
//  IOS_OneSignal.h
//  Unity-iPhone
//
//  Created by M.z.H on 2020/9/9.
//

#ifndef IOS_OneSignal_h
#define IOS_OneSignal_h


extern "C"{

// 初始化
void Apple_OneSignalInit(NSDictionary* launchOptions);
// 发送通知
void Apple_PostNotification(const char* language,const char* message,const char* title, long delay);
// 取消通知
void Apple_CancelNotification();
}


#endif /* IOS_OneSignal_h */
