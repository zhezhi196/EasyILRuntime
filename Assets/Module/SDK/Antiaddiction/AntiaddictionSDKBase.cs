using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class AntiaddictionSDKBase
    {
        public Action<string> LoginState = null;
        public virtual void Init()
        {

        }


        /// <summary>
        /// 实名认证注册弹窗
        /// </summary>
        /// <param name="loginState"></param>
        public virtual void ShowRealNameDialog(Action<string> loginState)
        {
            if (loginState != null)
            {
                LoginState = loginState;
            }
        }

        /// <summary>
        /// 退出登录（该接口会重置本地userId，用户下次进入游戏需要重新注册）
        /// </summary>
        public virtual void LoginOut()
        {

        }

        /// <summary>
        /// 判断是否已注册 (查询当前是否是已进行了注册,结果为true表示已经注册过，false代表未注册)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRegistered()
        {
            return true;
        }

        /// <summary>
        /// 检查防沉迷系统启用状态 (调用本方法的结果回调接口为 checkSuccess(boolean) 与 checkError(String))
        /// </summary> 
        public virtual void CheckAbled()
        {

        }

        /// <summary>
        /// 当前金额是否可以支付
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public virtual bool CanPay(int money)
        {
            return false;
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="money"></param>
        public virtual void Pay(int money)
        {

        }

        /// <summary>
        /// 最大可支付额度
        /// </summary>
        /// <returns></returns>
        public virtual int MaxAmount()
        {
            return 0;
        }

        /// <summary>
        /// 获取单笔最大支付金额
        /// </summary>
        /// <returns></returns>
        public virtual int SignleAmount()
        {
            return 0;
        }

        /// <summary>
        /// 可用支付额度
        /// </summary>
        /// <returns></returns>
        public virtual int LeftAmount()
        {
            return 0;
        }

        /// <summary>
        /// 最大游戏时长
        /// </summary>
        /// <returns></returns>
        public virtual int MaxTime()
        {
            return 0;
        }

        /// <summary>
        /// 剩余时长
        /// </summary>
        /// <returns></returns>
        public virtual int LeftTime()
        {
            return 0;
        }

        /// <summary>
        /// 获取用户类型 (0.成年 1.青少年 2.少年 3.儿童 4.游客)
        /// </summary>
        /// <returns></returns>
        public virtual int GetUserType()
        {
            return 0;
        }

        /// <summary>
        /// 获取用户上次登录的用户类型 (0.成年 1.青少年 2.少年 3.儿童 )
        /// </summary>
        /// <returns></returns>
        public virtual int GetLastTimeUserType()
        {
            return 0;
        }

        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="state">0 -- 认证成功 1 -- 认证中 2 -- 认证失败</param>
        public void OnLoginState(string state)
        {
            SDKMgr.GetInstance().Log("AntiaddictionSDKBase   OnLoginState  --- state = " + state);
            if (LoginState != null)
            {
                LoginState(state);
                LoginState = null;
            }
        }


    }
}
