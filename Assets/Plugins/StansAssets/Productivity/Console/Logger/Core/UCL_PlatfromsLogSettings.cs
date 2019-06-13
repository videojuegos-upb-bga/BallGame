using SA.Foundation.Config;
using SA.Foundation.Patterns;

namespace SA.Productivity.Console
{
    public class UCL_PlatfromsLogSettings : SA_ScriptableSingleton<UCL_PlatfromsLogSettings>
    {

        public const string PLUGIN_NAME = "Ultimate Console";
        public const string DOCUMENTATION_URL = "https://unionassets.com/ultimate-logger/manual";
        public const string PLUGIN_FOLDER = SA_Config.STANS_ASSETS_PRODUCTIVITY_PLUGINS_PATH + "Console/";
        public bool iOS_LogsRecord = true;
        public bool iOS_OverrideLogsOutput = true;


        public bool Android_LogsRecord = true;
        public bool Android_OverrideLogsOutput = true;





        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------


        protected override string BasePath {
            get { return PLUGIN_FOLDER; }
        }


        public override string PluginName {
            get {
                return "Ultimate Console";
            }
        }

        public override string DocumentationURL {
            get {
                return DOCUMENTATION_URL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return SA_Config.EDITOR_PRODUCTIVITY_MENU_ROOT + "Console/Show";
            }
        }
    }
}