using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Content;
using Android.Widget;

namespace BatteryStat
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            // Retrieve battery information
            var batteryStatus = Application.Context.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
            int level = batteryStatus.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = batteryStatus.GetIntExtra(BatteryManager.ExtraScale, -1);
            float batteryPct = (level / (float)scale) * 100;

            // Find the TextView element by its ID (batteryStatusTextView)
            var batteryStatusTextView = FindViewById<TextView>(Resource.Id.batteryStatusTextView);

            // Check if the device is charging
            bool isCharging = batteryStatus.GetIntExtra(BatteryManager.ExtraPlugged, -1) > 0;

            // Determine the charging status
            string chargingStatus = isCharging ? "Charging" : "Not Charging";

            // Update the TextView to display the battery percentage and charging status
            batteryStatusTextView.Text = $"Battery Status: {batteryPct}% ({chargingStatus})";

            // Find the ImageView element
            var batteryImageView = FindViewById<ImageView>(Resource.Id.batteryImageView);

            // Update the ImageView based on the battery level
            if (level >= 75)
            {
                batteryImageView.SetImageResource(Resource.Drawable.full_battery_image);
            }
            else if (level >= 40)
            {
                batteryImageView.SetImageResource(Resource.Drawable.medium_battery_image);
            }
            else
            {
                batteryImageView.SetImageResource(Resource.Drawable.low_battery_image);
            }

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }


        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
