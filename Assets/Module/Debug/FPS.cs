// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Module
// {
//     public static class FPS
//     {
//         private static float time;
//         private static int frameCount;
//         public static float fps;
//         public static int maxFps = 60;
//         public static float fluentK = 0.9f;
//         public static bool isFluent;
//         public static event Action onFluent;
//
//
//         public static void SetFps(int fps)
//         {
//             Application.targetFrameRate = fps;
//             maxFps = fps;
//         }
//
//         public static void Update()
//         {
//             time += Time.unscaledDeltaTime;
//             frameCount++;
//             if (time >= 0.5f && frameCount >= 1)
//             {
//                 fps = frameCount / time;
//                 time = time - 0.5f;
//                 frameCount = 0;
//                 if (fps >= fluentK * maxFps)
//                 {
//                     if (onFluent != null && !isFluent)
//                     {
//                         isFluent = true;
//                         onFluent();
//                     }
//                 }
//                 else
//                 {
//                     isFluent = false;
//                 }
//             }
//         }
//     }
// }