using System.Text.Json;
#if ANDROID 
using Android.Content;
using Android.OS;
using Android.Util;
using ZebraDataWedge.Platforms.Android;
#endif

namespace ZebraDataWedge.Controllers
{
    public class DatawedgeLifecycleController
    {
        private static string ACTION_DATAWEDGE = "com.symbol.datawedge.api.ACTION"; // from datawedge 6.2.0 - current?
        private static string EXTRA_CREATE_PROFILE = "com.symbol.datawedge.api.CREATE_PROFILE";
        private static string EXTRA_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";
        private static string EXTRA_PROFILE_NAME;

#if ANDROID
        public static DatawedgeReceiver _broadcastReceiver;
        public static IntentFilter _intentFilter;
#endif
        public DatawedgeLifecycleController(string extraProfileName) 
        {
            EXTRA_PROFILE_NAME = extraProfileName;
        }
        public DatawedgeLifecycleController(
            string actionDatawedge,
            string extraCreateProfile,
            string extraSetConfig,
            string extraProfileName)
        {
            ACTION_DATAWEDGE = actionDatawedge;
            EXTRA_CREATE_PROFILE = extraCreateProfile;
            EXTRA_SET_CONFIG = extraSetConfig;
            EXTRA_PROFILE_NAME = extraProfileName;
        }

        public void OnCreate()
        {
#if ANDROID
            _broadcastReceiver = new DatawedgeReceiver();

            _intentFilter = new IntentFilter(DatawedgeReceiver.IntentAction);
            _intentFilter.AddCategory(DatawedgeReceiver.IntentCategory);
#endif
            CreateProfile();
        }

        public void OnResume()
        {
#if ANDROID
            Android.App.Application.Context.RegisterReceiver(_broadcastReceiver, _intentFilter);
            
            ScannerController scanner = ScannerController.Instance;

            _broadcastReceiver.scanDataReceived += (s, scanData) => 
            {
                scanner.ScannedData = scanData;
            };
#endif
        }

        public void OnPause()
        {
#if ANDROID

            Android.App.Application.Context.UnregisterReceiver(_broadcastReceiver);
#endif
        }

        private void CreateProfile()
        {
#if ANDROID
            Log.Debug("LOG", $"::::::::CONFIGURING PROFILE [{EXTRA_PROFILE_NAME}]");

            // Create a DataWedge profile on the device
            string profileName = EXTRA_PROFILE_NAME;
            SendDatawedgeIntentSimple(ACTION_DATAWEDGE, EXTRA_CREATE_PROFILE, profileName);

            // Init configuration object
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "true");
            profileConfig.PutString("CONFIG_MODE", "UPDATE");

            // Init barcode configuration
            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "true");

            Bundle barcodeProps = new Bundle();
            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);

            Bundle appConfig = new Bundle();            
            appConfig.PutString("PACKAGE_NAME", Android.App.Application.Context.PackageName); // This profile belongs to this app
            appConfig.PutStringArray("ACTIVITY_LIST", new string[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });

            // Blast all the barcode configuration to DataWedge
            SendDatawedgeIntentBundle(ACTION_DATAWEDGE, EXTRA_SET_CONFIG, profileConfig);

            // Init intent configuration
            profileConfig.Remove("PLUGIN_CONFIG");

            Bundle intentConfig = new Bundle();
            intentConfig.PutString("PLUGIN_NAME", "INTENT");
            intentConfig.PutString("RESET_CONFIG", "true");

            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true"); // Broadcast barcode scan intents 
            intentProps.PutString("intent_action", DatawedgeReceiver.IntentAction);
            intentProps.PutString("intent_delivery", "2");
            intentConfig.PutBundle("PARAM_LIST", intentProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);

            SendDatawedgeIntentBundle(ACTION_DATAWEDGE, EXTRA_SET_CONFIG, profileConfig);
#endif
        }

        private void SendDatawedgeIntentSimple(string action, string key, string val)
        {
#if ANDROID
            Intent intent = new Intent();
            intent.SetAction(action);
            intent.PutExtra(key, val);
            Android.App.Application.Context.SendBroadcast(intent);
#endif
        }

#if ANDROID
        private void SendDatawedgeIntentBundle(string action, string key, Bundle extras)
        {

            Intent intent = new Intent();
            intent.SetAction(action);
            intent.PutExtra(key, extras);
            Android.App.Application.Context.SendBroadcast(intent);
    }
#endif



    }
}
