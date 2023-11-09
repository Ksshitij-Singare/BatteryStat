using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Xamarin.Essentials;

namespace BatteryStat
{
    [Service]
    public class BatteryNotificationService : Service
    {
        private const int NotificationId = 1;
        private Handler handler;
        private Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();
            handler = new Handler();
            runnable = new Action(() =>
            {
                // Retrieve battery information
                var batteryStatus = Application.Context.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
                int level = batteryStatus.GetIntExtra(BatteryManager.ExtraLevel, -1);
                int scale = batteryStatus.GetIntExtra(BatteryManager.ExtraScale, -1);
                float batteryPct = (level / (float)scale) * 100;

                // Check if the device is charging
                bool isCharging = batteryStatus.GetIntExtra(BatteryManager.ExtraPlugged, -1) > 0;

                // Determine the charging status
                string chargingStatus = isCharging ? "Charging" : "Not Charging";

                // Create a notification to display battery status
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                var notification = new Notification.Builder(this, "BatteryChannel")
                    .SetContentTitle("Battery Status")
                    .SetContentText($"Battery: {batteryPct}% ({chargingStatus})")
                    .SetSmallIcon(Resource.Drawable.icon)
                    .Build();

                notificationManager.Notify(NotificationId, notification);

                // Schedule the next battery check in 1 hour
                handler.PostDelayed(runnable, 60 * 60 * 1000);
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            handler.Post(runnable);
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            handler.RemoveCallbacks(runnable);
        }
    }
}
