// using System;
// using UnityEngine;
// using xasset;
// using Object = UnityEngine.Object;
//
// namespace Module
// {
//     public class GameDebug
//     {
//         public static void Log(object msg)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.Log(msg);
//         }
//
//         public static void LogError(object msg)
//         {
//             UnityEngine.Debug.LogError(msg);
//         }
//
//         public static void LogWarn(object msg)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.LogWarning(msg);
//         }
//
//         public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawLine(start, end, color, duration);
//         }
//
//         public static void DrawLine(Vector3 start, Vector3 end, Color color)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawLine(start, end, color);
//         }
//
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawRay(start, dir, color, duration);
//         }
//
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawRay(start, dir, color);
//         }
//
//         public static void DrawRay(Vector3 start, Vector3 dir)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawRay(start, dir);
//         }
//
//         public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
//         }
//
//         public static void Assert(bool condition)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.Assert(condition);
//         }
//
//         public static void Assert(bool condition, Object context)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.Assert(condition, context);
//         }
//
//         public static void Assert(bool condition, object message)
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.Assert(condition, message);
//         }
//
//         public static void Break()
//         {
//             if (Configuration.isLog)
//                 UnityEngine.Debug.Break();
//         }
//     }
// }