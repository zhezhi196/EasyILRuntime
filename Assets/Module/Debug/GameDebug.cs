// /*
//  * 脚本名称：GameDebug
//  * 项目名称：FrameWork
//  * 脚本作者：黄哲智
//  * 创建时间：2017-12-26 11:47:14
//  * 脚本作用：
// */
//
// using System;
// using System.Diagnostics;
// using System.IO;
// using System.Text;
// using UnityEngine;
// using Debug = UnityEngine.Debug;
// using Object = UnityEngine.Object;
//
// namespace Module
// {
//     public class GameDebug
//     {
//         private static string logPath = Application.persistentDataPath + "/Error.txt";
//         
//         [Conditional("LOG_ENABLE")]
//         public static void Log(object msg)
//         {
//             UnityEngine.Debug.Log(msg);
//         }
//         
//         public static void LogError(object msg)
//         {
//             UnityEngine.Debug.LogError(msg);
// #if LOG_ERROR
//             if (!File.Exists(logPath))
//             {
//                 using (StreamWriter writer = File.CreateText(logPath))
//                 {
//                     StringBuilder str = new StringBuilder();
//                     str.Append("IdentifierID: ");
//                     str.Append(SystemInfo.deviceUniqueIdentifier);
//                     str.Append("\n");
//                     str.Append("deviceName: ");
//                     str.Append(SystemInfo.deviceName);
//                     str.Append("\n");
//                     str.Append("operatingSystem: ");
//                     str.Append(SystemInfo.operatingSystem);
//                     str.Append("\n\n");
//                     str.Append(DateTime.Now);
//                     str.Append("  : ");
//                     str.Append(msg);
//                     writer.WriteLine(str);
//                 }
//             }
//             else
//             {
//                 using (StreamWriter writer = new StreamWriter(logPath, true))
//                 {
//                     StringBuilder str = new StringBuilder();
//                     str.Append(DateTime.Now);
//                     str.Append("  : ");
//                     str.Append(msg);
//                     writer.WriteLine(str);
//                 }
//             }
// #endif
//         }
//
//         public static void LogErrorFormat(string msg, params object[] param)
//         {
//             UnityEngine.Debug.LogErrorFormat(msg, param);
// #if LOG_ERROR
//             if (!File.Exists(logPath))
//             {
//                 using (StreamWriter writer = File.CreateText(logPath))
//                 {
//                     StringBuilder str = new StringBuilder();
//                     str.Append("IdentifierID: ");
//                     str.Append(SystemInfo.deviceUniqueIdentifier);
//                     str.Append("\n");
//                     str.Append("deviceName: ");
//                     str.Append(SystemInfo.deviceName);
//                     str.Append("\n");
//                     str.Append("operatingSystem: ");
//                     str.Append(SystemInfo.operatingSystem);
//                     str.Append("\n\n");
//                     str.Append(DateTime.Now);
//                     str.Append("  : ");
//                     str.Append(string.Format(msg,param));
//                     writer.WriteLine(str);
//                 }
//             }
//             else
//             {
//                 using (StreamWriter writer = new StreamWriter(logPath, true))
//                 {
//                     StringBuilder str = new StringBuilder();
//                     str.Append(DateTime.Now);
//                     str.Append("  : ");
//                     str.Append(string.Format(msg,param));
//                     writer.WriteLine(str);
//                 }
//             }
// #endif
//         }
//
//         [Conditional("LOG_ENABLE")]
//         public static void LogWarn(object msg)
//         {
//             UnityEngine.Debug.LogWarning(msg);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
//         {
//             UnityEngine.Debug.DrawLine(start, end, color, duration);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawLine(Vector3 start, Vector3 end, Color color)
//         {
//             UnityEngine.Debug.DrawLine(start, end, color);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
//         {
//             UnityEngine.Debug.DrawRay(start, dir, color, duration);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color)
//         {
//             UnityEngine.Debug.DrawRay(start, dir, color);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawRay(Vector3 start, Vector3 dir)
//         {
//             UnityEngine.Debug.DrawRay(start, dir);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
//         {
//             UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void Assert(bool condition)
//         {
//             UnityEngine.Debug.Assert(condition);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void Assert(bool condition, Object context)
//         {
//             UnityEngine.Debug.Assert(condition, context);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void Assert(bool condition, object message)
//         {
//             UnityEngine.Debug.Assert(condition, message);
//         }
//         [Conditional("LOG_ENABLE")]
//         public static void Break()
//         {
//             UnityEngine.Debug.Break();
//         }
//
//         private static int VertexCount = 50;
//         public static void DrawCircle(float radius,Vector3 center,Color color)
//         {
//             float deltaTheta = (2f * Mathf.PI) / VertexCount;
//             float theta = 0f;
//             Vector3 oldPos = center;
//             for (int j = 0; j < VertexCount + 1; j++)
//             {
//                 
//                 Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
//                 Gizmos.DrawLine(oldPos, center + pos);
//                 Gizmos.color = color;
//                 oldPos = center + pos;
//                 theta += deltaTheta;
//             }
//         }
//         
//         [Conditional("LOG_ENABLE")]
//         public static void LogFormat(string msg, params object[] args)
//         {
//             Debug.Log(string.Format(msg,args));
//         }
//     }
// }