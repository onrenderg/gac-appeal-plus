using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Essentials;
using Android.Content.Res;
using Java.IO;
using Java.Lang;
using Android.Widget;

namespace GACAppeal.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Preferences.Set("EncKey", "5gS2E70CA13935B9");
            Preferences.Set("BasicAuth", $"3O%2BMVC0RSgx0klSvZ4%2Fpcw%3D%3D:2uS7yDJy02QGCywWXsEx7aVd%2BjrDBhbERipESVk%2BRcU%3D");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
#if DEBUG
            LoadApplication(new App());
#else
            if (!RootUtil.IsDeviceRooted())
            {
                LoadApplication(new App());
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Not supported on Rooted Devices", ToastLength.Long).Show();
                FinishAffinity();
            }
#endif
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override Resources Resources
        {
            get
            {
                Resources res = base.Resources;
                Configuration config = new Configuration();
                config.SetToDefaults();
                res.UpdateConfiguration(config, res.DisplayMetrics);
                return res;
            }
        }
        public class RootUtil
        {
            public static bool IsDeviceRooted()
            {
                return checkRootMethod1() || checkRootMethod2() || checkRootMethod3();
            }

            private static bool checkRootMethod1()
            {
                string buildTags = Android.OS.Build.Tags;
                return buildTags != null && buildTags.Contains("test-keys");
            }

            private static bool checkRootMethod2()
            {
                string[] paths = { "/system/app/Superuser.apk", "/sbin/su", "/system/bin/su", "/system/xbin/su", "/data/local/xbin/su", "/data/local/bin/su", "/system/sd/xbin/su",
                "/system/bin/failsafe/su", "/data/local/su", "/su/bin/su"};
                foreach (string path in paths)
                {
                    if (new File(path).Exists()) return true;
                }
                return false;
            }

            private static bool checkRootMethod3()
            {
                Java.Lang.Process process = null;
                try
                {
                    process = Runtime.GetRuntime().Exec(new string[] { "/system/xbin/which", "su" });
                    BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(process.InputStream));
                    if (bufferedReader.ReadLine() != null) return true;
                    return false;
                }
                catch (Throwable t)
                {
                    return false;
                }
                finally
                {
                    if (process != null) process.Destroy();
                }
            }
        }
    }
}
