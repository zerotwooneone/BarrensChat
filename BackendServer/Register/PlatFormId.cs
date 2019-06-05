using System;

namespace BackendServer.Register
{
    public enum PlatFormId
    {
        /// <summary>
        /// MPNS
        /// </summary>
        [Obsolete("Not used anymore")]
        MicrosoftPushNotificationService = 1,

        /// <summary>
        /// WNS
        /// </summary>
        WindowsPushNotificationService = 2,

        /// <summary>
        /// APNS
        /// </summary>
        [Obsolete("Not yet supported")]
        ApplePushNotificationService = 3,

        /// <summary>
        /// FCM, formerly GCM - Google Cloud Messaging
        /// </summary>
        FirebaseCloudMessaging = 4
    }
}