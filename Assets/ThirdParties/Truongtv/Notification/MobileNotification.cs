using System;
using System.Collections.Generic;
using NotificationSamples;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThirdParties.Truongtv.Notification
{
    public class MobileNotification : GameNotificationsManager
    {
        private static MobileNotification _instance;
        public static MobileNotification Instance => _instance;
        private readonly List<IGameNotification> _listNotifications = new List<IGameNotification>();
        private GameNotificationChannel[] _defaultNotificationChannels;
#region Private

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
        }

        private void Start()
        {
            if (!Initialized)
            {
                InitChannels();
            }
        }

        private void InitChannels()
        {
            _defaultNotificationChannels = new[]
            {
                new GameNotificationChannel(NotificationChannel.DefaultChannel.ToString(),
                    "Default Game Channel", "Generic notifications"),
                new GameNotificationChannel(NotificationChannel.ReminderChannel.ToString(),
                    "Reminder Channel", "Reminder notifications"),
                new GameNotificationChannel(NotificationChannel.GameChannel.ToString(),
                    "Game Channel", "Game notifications")
            };
            Initialize(_defaultNotificationChannels);
            CancelAllNotifications();

#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllScheduledNotifications();

            void ReceivedNotificationHandler(AndroidNotificationIntentData data)
            {
                var msg = "Notification received : " + data.Id + "\n";
                msg += "\n Notification received: ";
                msg += "\n .Title: " + data.Notification.Title;
                msg += "\n .Body: " + data.Notification.Text;
                msg += "\n .Channel: " + data.Channel;
                Debug.Log(msg);
            }
            AndroidNotificationCenter.OnNotificationReceived += ReceivedNotificationHandler;

#endif
            PlayGameReminder(1);
            PlayGameReminder(3);
        }
        private void SendNotification(string title, string body, DateTime deliveryTime,
            bool reschedule = false, string channelId = null)
        {
            var notification = CreateNotification();

            if (notification == null)
            {
                return;
            }

            notification.Title = title;
            notification.Body = body;
            notification.Group = !string.IsNullOrEmpty(channelId)
                ? channelId
                : NotificationChannel.DefaultChannel.ToString();
            notification.DeliveryTime = deliveryTime;
            notification.Id = (int) NotificationChannel.DefaultChannel;
            if (Enum.TryParse<NotificationChannel>(channelId, out var id))
            {
                notification.Id = (int) id;
            }


            var match = _listNotifications.Find((noti) => noti.Id == notification.Id);
            if (match != null) return;
            _listNotifications.Add(notification);
            var notificationToDisplay = ScheduleNotification(notification);
            notificationToDisplay.Reschedule = reschedule;
        }
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                if (!Initialized)
                {
                    InitChannels();
                }
            }
        }
        private void PlayGameReminder(int day)
        {
            DateTime deliveryTime = DateTime.Now.ToLocalTime().AddDays(day);
            var data = new[]
            {
                "🎈🎈 More and more level are coming. Have you beat'em all? 🎈🎈",
                "🔥 You win so much level yesterday! How many can you beat today? 🔥",
                "🏆️🥁 We are missing you! Come back with us! ️🎵️🍒"
            };
            SendNotification("", data[Random.Range(0, 3)]
                , deliveryTime, true, channelId: NotificationChannel.GameChannel.ToString());
        }
        public void SetLuckySpinReminder()
        {
            var minReminderHour = 8;
            var maxReminderHours = 21;
            var deliveryTime = DateTime.Now;
            var hourNow = deliveryTime.Hour;
            if (hourNow + 8 < minReminderHour || hourNow + 8 > maxReminderHours)
            {
                deliveryTime = deliveryTime.AddDays(1).Date;
            }
            deliveryTime = deliveryTime.AddHours(8);
            var data = new[]
            {
                "🎁 You got 1 free lucky spin. Try it now 🎉",
                "💘️ Free spin available now. Roll and receive special reward!🎊️🎶",
                "☀ Today is a beautiful day, our Hero has just sent you a lucky spin, take a chance to check it out ️🎈"
            };
            SendNotification("Free Spin await! 🎁 ", data[Random.Range(0, 3)]
                , deliveryTime, channelId: NotificationChannel.ReminderChannel.ToString());
        }
        
        public void DailyRewardResetReminder(bool receive)
        {
            int playReminderHour = 8;
            // Schedule a reminder to play the game. Schedule it for the next day.
            DateTime deliveryTime = DateTime.Now.ToLocalTime();
            int hourNow = DateTime.Now.Hour;
            if (playReminderHour <= hourNow || receive)
            {
                deliveryTime = DateTime.Now.ToLocalTime().AddDays(1);
            }

            var newDeliveryTime = new DateTime(deliveryTime.Year, deliveryTime.Month, deliveryTime.Day,
                playReminderHour, 0, 0,
                DateTimeKind.Local);
            var data = new[]
            {
                "🎁 Come grab your HUGE daily prize! You've earned it! 🎉",
                "💘️ Did you receive the gift today? 🎊️🎶",
                "☀️ Today is a beautiful day, our Hero has just sent you a special gift, take a chance to check it out ️🎈"
            };
            SendNotification("Gift await! 🎁 ", data[Random.Range(0, 3)]
                , newDeliveryTime, reschedule: true, channelId: NotificationChannel.DefaultChannel.ToString());
        }
#endregion
    }
    public enum NotificationChannel
    {
        DefaultChannel = 0,
        ReminderChannel = 1,
        GameChannel = 2
        
    }
}