// using System;
// using LitJson;
// using Module;
// using SDK;
// using UnityEngine;
//
// /// <summary>
// /// 登录类型
// /// </summary>
// public enum LoginType
// {
//     Default = 0,
//     google,
//     apple,
//     facebook,
//     tourist,
// }
//
//
// /// <summary>
// /// 服务器登陆逻辑
// /// </summary>
// public class ServerSignInNode
// {
//     public LoginType type;
//     private int Retry = 0;
//     public string signUrl = "api/v1/user/login";
//     public bool ServerLoginSuccess = false;//服务器登录上没
//
//
//     public ServerSignInNode(LoginType type)
//     {
//         this.type = type;
//     }
//
//     public void Run()
//     {
//         // //没网调失败
//         // if (!HUtils.IsMobileData() && !HUtils.IsWifi())
//         // {
//         //     Failed(LanguageController.Instance.GetContent("download01"));
//         //     return;
//         // }
//
//         Post();
//     }
//     
//     public static DateTime ServerEndTime
//     {
//         get { return DataInit.Instance.GetSqlService<PlayerSaveData>().tableList[0].serverEndTime.ToDateTime(); }
//         set { DataInit.Instance.GetSqlService<PlayerSaveData>().Update(new PlayerSaveData() {ID = 1, serverEndTime = value.ToString()}); }
//     }    
//     public static string ServerToken
//     {
//         get { return DataInit.Instance.GetSqlService<PlayerSaveData>().tableList[0].serverToken; }
//         set { DataInit.Instance.GetSqlService<PlayerSaveData>().Update(new PlayerSaveData() {ID = 1, serverToken = value.ToString()}); }
//     }
//
//     public static string serverId
//     {
//         get { return DataInit.Instance.GetSqlService<PlayerSaveData>().tableList[0].serverId; }
//         set { DataInit.Instance.GetSqlService<PlayerSaveData>().Update(new PlayerSaveData() {ID = 1, serverId = value}); }
//     }
//     
//     public static string serverData
//     {
//         get { return DataInit.Instance.GetSqlService<PlayerSaveData>().tableList[0].serverMsg; }
//         set { DataInit.Instance.GetSqlService<PlayerSaveData>().Update(new PlayerSaveData() {ID = 1, serverMsg = value}); }
//     }
//
//     public void Post()
//     {
// #if UNITY_EDITOR //测试用
//         if (type == LoginType.Default)
//         {
//             //服务器给玩家的UID
//             if (!string.IsNullOrEmpty(serverId))
//             {
//                 if (CheckToken())
//                 {
//                     //服务器给的所有数据
//                     string msg = serverData;
//                     GameDebug.Log("未联网token默认登录成功:" + msg);
//                     GetServerMsg(msg, false);
//                     return;
//                 }
//             }
//
//             string url1 = HttpCache.Instance.url + signUrl;
//             HttpArgs args1 = GetPostArgs();
//             NetWorkHttp.Instance.Post(url1, args1,
//                 (string msg) =>
//                 {
//                     GameDebug.Log("请求登录返回成功:" + msg);
//                     GetServerMsg(msg, true);
//                 }, 30);
//             return;
//         }
// #endif
//         if (type == LoginType.Default)
//         {
//             if (string.IsNullOrEmpty(serverId))
//             {
//                 Failed("游客没有UID", false);
//                 return;
//             }
//
//             if (CheckToken())
//             {
//                 string msg = DataInit.Instance.GetSqlService<PlayerSaveData>().tableList[0].serverMsg;
//                 GameDebug.Log("免服务器登录成功:" + msg);
//                 GetServerMsg(msg, false);
//                 return;
//             }
//             else
//             {
//                 Failed("token过期", false);
//                 return;
//             }
//         }
//
//         string url = HttpCache.Instance.url + signUrl;
//         GameDebug.Log("登录地址:" + url);
//         HttpArgs args = GetPostArgs();
//         NetWorkHttp.Instance.Post(url, args,
//             (string msg) =>
//             {
//                 GameDebug.Log("登录成功:" + msg);
//                 GetServerMsg(msg, true);
//             }, 30);
//     }
//
//     private bool CheckToken()
//     {
//         if (!string.IsNullOrEmpty(ServerToken))
//         {
//             return TimeHelper.now < ServerEndTime;
//         }
//         return false;
//     }
//
//     public HttpArgs GetPostArgs()
//     {
//         HttpArgs args = new HttpArgs();
//         string uid = serverId;
//
//
// #if UNITY_EDITOR //测试用
//         args.AddArgs("channel", "1");
//         args.AddArgs("type", "visitor");
//         args.AddArgs("uid", uid);
//         return args;
// #endif
//
//         if (type == LoginType.tourist)
//         {
//             args.AddArgs("channel", "1");
//             args.AddArgs("type", "visitor");
//         }
//
//         if (!string.IsNullOrEmpty(uid))
//         {
//             GameDebug.Log("登录uid" + uid);
//             args.AddArgs("uid", uid);
//         }
//
//         return args;
//     }
//
//     private void GetServerMsg(string msg, bool setPlayerPrefs)
//     {
//         //服务器返回失败
//         GameDebug.Log("登录信息:" + msg);
//         if (msg == "")
//         {
//             Failed(Language.GetContent("download01"));
//             return;
//         }
//
//         JsonData data = JsonMapper.ToObject(msg);
//         if (data["code"].ToString() == "200")
//         {
//             if (setPlayerPrefs)
//             {
//                 //记录一下登陆状态
//                 SetSignInType(type, true);
//             }
//
//             serverData = msg;
//             Success(data);
//         }
//         else if (data["code"].ToString() == "-1")
//         {
//             GameDebug.Log("失败:" + msg);
//             //重置所有信息
//             ResetLoginAllInfo();
//             Failed(data["msg"].ToString() + ":" + data["code"].ToString());
//         }
//         else
//         {
//             // if (Retry <= 2)
//             // {
//             //     GameDebug.Log("重试登录第" + Retry + "次");
//             //     Retry++;
//             //     Run();
//             //     return;
//             // }
//             Failed(data["msg"].ToString() + ":" + data["code"].ToString());
// #if FACEBOOK_CLOUDGAME
//             GamePlay.Instance.ExitGame();
// #endif
//         }
//     }
//
//     private void Failed(string error, bool showtip = true)
//     {
//         GameDebug.LogError("登录失败:" + error);
//     }
//
//     private void Success(JsonData data)
//     {
//         //成功的消息
//         JsonData jsonInfo = data;
//         
//         ServerLoginSuccess = true;
//         ServerToken = (string) jsonInfo["token"];
//         
//          PlayerPrefs.SetString(ConstKey.PlayerServerToken, GamePlay.Instance.ServerToken);
//          long serverEndTime = (long) jsonInfo["expire"];
//          PlayerPrefs.SetString(ConstKey.PlayerServerEndTime, serverEndTime.ToString());
//          ServerEndTime = Util.ConvertUnixTimeToDateTime((serverEndTime).ToLong());
//          string w = (string) jsonInfo["w"];
//          GamePlay.Instance.WhiteList = w == "1";
//          string c = (string) jsonInfo["c"];
//          GamePlay.Instance.BlackList = c == "1";
//          string uid = (string) jsonInfo["userId"];
//          serverId = uid;
//
//         SDKMgr.GetInstance().MyAnalyticsNewManagerSDK.SuperPropertiesEvent("uid", uid, E_AnalyticsType.Thinking);
//
//          SDKMgr.GetInstance().MyAnalyticsNewManagerSDK
//              .SuperPropertiesEvent("is_login", "TRUE", E_AnalyticsType.Thinking);
//     }
//     
//     public void SetSignInType(LoginType loginType, bool value)
//     {
//         string str = GetLoginTypeConstKey(loginType);
//         if (str == "")
//         {
//             return;
//         }
//         GameDebug.Log(str + "|" + value);
//         GameDebug.Log(str + "|" + Convert.ToInt32(value));
//         PlayerPrefs.SetInt(str, Convert.ToInt32(value));
//     }
//     
//     public string GetLoginTypeConstKey(LoginType loginType)
//     {
//         string str = "";
//         switch (loginType)
//         {
//             case LoginType.apple:
//                 str = "appleLoginSuccess";
//                 break;
//             case LoginType.facebook:
//                 str = "facebookLoginSuccess";
//                 break;
//         }
//         return str;
//     }
//     public void ResetLoginAllInfo()
//     {
//         ServerLoginSuccess = false;
//         ServerToken = "";
//     }
// }
//
