using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using PCLStorage;
using WebViewSample.Droid;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileImplementation))]
namespace WebViewSample.Droid
{
    public class FileImplementation : FileInterface
    {
        public FileImplementation() { }

        public string GetPath(string path, string fileName)
        {
            string path1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // app directory
            string path2 = path + fileName; // custom folder and file name
            string finalPath = Path.Combine(path1, path2);

            Console.WriteLine(finalPath);

            return finalPath;
        }

        public async void WriteFile(string path, string fileName, string data)
        {
            File.WriteAllText(GetPath(path, fileName), data); // writes to local storage
        }

        public async void ReadFile(string text)
        {

        }

        public bool FileExists(string path, string fileName)
        {
            return File.Exists(GetPath(path, fileName));
        }
    }

    [Activity(Label = "WebViewSample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

