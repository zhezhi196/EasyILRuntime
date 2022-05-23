using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDK
{
    public class AntiaddictionSDKAndroid : AntiaddictionSDKBase
    {
        AndroidJavaClass jc;
        AndroidJavaObject jo;

        public override void Init()
        {
            jc = new AndroidJavaClass("com.chenguan.antiaddiction.AntiaddictionManager");
            jo = jc.GetStatic<AndroidJavaObject>("Instance");
            
        }

        public override void ShowRealNameDialog(Action<string> loginState)
        {
            base.ShowRealNameDialog(loginState);
            jo.Call("ShowRealNameDialog");
        }

        public override void LoginOut()
        {
            jo.Call("LoginOut");
        }

        public override bool IsRegistered()
        {
            return jo.Call<bool>("IsRegistered") ;
        }

        public override void CheckAbled()
        {
            jo.Call("CheckAbled");
        }

        public override bool CanPay(int money)
        {
            return jo.Call<bool>("CanPay");
        }

        public override void Pay(int money)
        {
            jo.Call("Pay");
        }

        public override int MaxAmount()
        {
            return jo.Call<int>("MaxAmount");
        }

        public override int SignleAmount()
        {
            return jo.Call<int>("SignleAmount");
        }
        public override int LeftAmount()
        {
            return jo.Call<int>("LeftAmount");
        }

        public override int MaxTime()
        {
            return jo.Call<int>("MaxTime");
        }

        public override int LeftTime()
        {
            return jo.Call<int>("LeftTime");
        }

        public override int GetUserType()
        {
            return jo.Call<int>("GetUserType");
        }

        public override int GetLastTimeUserType()
        {
            return jo.Call<int>("GetLastTimeUserType");
        }


    }
}
