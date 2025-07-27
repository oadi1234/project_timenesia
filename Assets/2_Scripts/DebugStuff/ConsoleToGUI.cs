using UnityEngine;

namespace _2_Scripts.DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        // #if !UNITY_EDITOR
        private string myLog = "*begin log";
        private string output;
        private string stack;
        bool doShow = false;
        string filename = "";

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote)) //`
            {
                doShow = !doShow;
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            // for onscreen...
            myLog = myLog + "\n " + logString;
            if (myLog.Length > 700)
            {
                myLog = myLog.Substring(myLog.Length - 700);
            }

            // for the file ...
            if (filename == "")
            {
                string d = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
                System.IO.Directory.CreateDirectory(d);
                string r = Random.Range(1000, 9999).ToString();
                filename = d + "/log-" + r + ".txt";
            }

            try
            {
                System.IO.File.AppendAllText(filename, logString + "\n");
            }
            catch
            {
                // ignored
            }
        }

        void OnGUI()
        {
            if (!doShow) { return; }
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
                new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
        }
        // #endif
    }
}