//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.VisualScripting;
//#if UNITY_ANDROID
//using Unity.Notifications.Android;
//#elif UNITY_IOS
//using Unity.Notifications.iOS;
//#endif

//public class NotificationManager : MonoBehaviour
//{

//    // �ȵ���̵� ���� �˸��� ������ �޼���
//    public static void SendAndroidNotification(string title, string message)
//    {
//#if UNITY_ANDROID
//        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
//        using (var notificationManager = new AndroidJavaClass("com.unity.notification.Android.NotificationManager"))
//        {
//            notificationManager.CallStatic("sendNotification", currentActivity, title, message);
//        }
//#endif
//        {
//        }

//    }
//}
