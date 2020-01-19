using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Widget
{
    [BroadcastReceiver(Label = "HelloApp Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    class AppWidget : AppWidgetProvider
    {
        /// <summary>
        /// A list of structs containing image ids and names from the Drawable resource, in order according to times of day. 
        /// </summary>
        List<TimeContainer> timeContainers = new List<TimeContainer>(8);

        //static string UpdateImageAndTextEvent = "UpdateImageAndTextEvent";

        //called on update since update is called before OnEnabled
        public void Setup(Context context)
        {
            TimeContainer midnight = new TimeContainer("Midnight", Resource.Drawable.AsianNight);
            TimeContainer beforeDawn = new TimeContainer("Before Dawn", Resource.Drawable.BeforeDawn);
            TimeContainer earlyMorning = new TimeContainer("Early Morning", Resource.Drawable.CoolMorning);
            TimeContainer morning = new TimeContainer("Morning", Resource.Drawable.MorningTown);
            TimeContainer midday = new TimeContainer("Midday", Resource.Drawable.Afternoon);
            TimeContainer afternoon = new TimeContainer("Afternoon", Resource.Drawable.SunriseCity);
            TimeContainer evening = new TimeContainer("Evening", Resource.Drawable.Evening);
            TimeContainer night = new TimeContainer("Night", Resource.Drawable.CyberpunkNight);

            timeContainers.Add(midnight);
            timeContainers.Add(beforeDawn);
            timeContainers.Add(earlyMorning);
            timeContainers.Add(morning);
            timeContainers.Add(midday);
            timeContainers.Add(afternoon);
            timeContainers.Add(evening);
            timeContainers.Add(night);
        }

        struct TimeContainer
        {
            public string timeText;
            public int imageId;

            public TimeContainer(string timeText, int imageId)
            {
                this.timeText = timeText;
                this.imageId = imageId;
            }
        }

        //Only one updatePeriodMillis is used for all instances of the widget
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            Setup(context);

            //Toast.MakeText(context, "Updated", ToastLength.Short).Show();

            int numOfIds = appWidgetIds.Length;

            for (int i=0; i < numOfIds; i++)
            {
                int appWidgetId = appWidgetIds[i];

                //Get the layout for the App Widget using RemoteViews and attach a clicking listener to the widget
                RemoteViews widgetUI = new RemoteViews(context.PackageName, Resource.Layout.activity_main);

                // Create an Intent to update the widget
                Intent intent = new Intent(context, typeof(AppWidget));

                //This gives the intent an action string which is supposed to call the UpdateTextAndImage method.
                //intent.SetAction(UpdateImageAndTextEvent);

                intent.PutExtra("widgetUI", widgetUI);

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);

                widgetUI.SetOnClickPendingIntent(Resource.Id.widgetBackground, pendingIntent);

                UpdateTextAndImage( context, widgetUI);

                //Update the current app widget (attaches UI to widget?)
                appWidgetManager.UpdateAppWidget(appWidgetId, BuildRemoteViews(context, appWidgetIds));
            }
        }

        private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetViews = new RemoteViews(context.PackageName, Resource.Layout.activity_main);

            UpdateTextAndImage(context, widgetViews);

            //make it so if the widget it clicked OnUpdate() can be called
            RegisterClicks(context, appWidgetIds, widgetViews);

            return widgetViews;
        }

        void UpdateTextAndImage( Context context, RemoteViews viewsToUpdate)
        {
            int hour = DateTime.Now.Hour;

            if (0 <= hour && hour < 3)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[0].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[0].timeText);
            }
            else if (3 <= hour && hour < 6)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[1].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[1].timeText);
            }
            else if (6 <= hour && hour < 9)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[2].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[2].timeText);
            }
            else if (9 <= hour && hour < 12)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[3].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[3].timeText);
            }
            else if (12 <= hour && hour < 15)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[4].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[4].timeText);
            }
            else if (15 <= hour && hour < 18)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[5].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[5].timeText); 
            }
            else if (18 <= hour && hour < 21)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[6].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[6].timeText);
            }
            else if (21 <= hour)
            {
                viewsToUpdate.SetImageViewResource(Resource.Id.timeOfDayImage, timeContainers[7].imageId);
                viewsToUpdate.SetTextViewText(Resource.Id.timeOfDayText, timeContainers[7].timeText);
            }
        }

        private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetUI)
        {
            var intent = new Intent(context, typeof(AppWidget));

            //this intent will call OnUpdate
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);

            //... for all app widget instances?
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

            //setup a broadcast-sending pending intent
            var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            //when the background is clicked send the pending intent
            widgetUI.SetOnClickPendingIntent(Resource.Id.widgetBackground, pendingIntent);
        }

        /// <summary>
        /// Called when receiving any broadcast and before calling a callback method like OnUpdate. Broadcasts contain intents, which contain actions (string tags) that can be used to call methods for example.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
           
            // Check if the given intent has an UpdateImageAndTextEvent string
            //if (UpdateImageAndTextEvent.Equals(intent.Action))
            //{
            //    RemoteViews widgetUI = (RemoteViews)intent.GetParcelableExtra("widgetUI");
            //    UpdateTextAndImage( context, widgetUI);
            //}
        }
    }
}