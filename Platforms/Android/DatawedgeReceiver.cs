using Android.Content;
using Android.Util;

namespace ZebraDataWedge.Platforms.Android
{
    [BroadcastReceiver]
    public class DatawedgeReceiver : BroadcastReceiver
    {
        // Intent data source
        private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
        // Barcode symbology
        private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
        // Capture our data as a string
        private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";

        // Intent
        public static string IntentAction = "com.example.INTENTACTION"; // Edit this to whatever you'd like :)
        public static string IntentCategory = "android.intent.category.DEFAULT";

        //ScannerController scanner = ScannerController.Init();

        public event EventHandler<string> scanDataReceived;
        public override void OnReceive(Context? context, Intent? intent)
        {
            // Check that this is the correct intent
            if (intent.Action.Equals(IntentAction))
            {
                // Get source tag
                string srcTag = intent.GetStringExtra(SOURCE_TAG);

                // Get data symbology
                string labelType = intent.GetStringExtra(LABEL_TYPE_TAG);

                // Get data as a string from intent
                string dataStr = intent.GetStringExtra(DATA_STRING_TAG);

                Log.Debug("LOG", $"::::::::RECEIVED INTENT [{IntentAction}] FROM [{srcTag}] LABEL TYPE [{labelType}] DATA [{dataStr}]");

                scanDataReceived(this, dataStr.Trim());
            }
        }

    }
}
