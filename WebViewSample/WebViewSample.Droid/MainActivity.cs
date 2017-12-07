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

[assembly: Xamarin.Forms.Dependency(typeof(WriteFileImplementation))]
namespace WebViewSample.Droid
{
    public class WriteFileImplementation : IWriteFile
    {
        public WriteFileImplementation() { }

        public async void WriteFile(string text)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            Console.WriteLine(documentsPath);

            string localFilename = "image.png";
            string localPath = Path.Combine(documentsPath, localFilename);
            File.WriteAllText(localPath, text); // writes to local storage

            /*
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder, if one does not exist already
            IFolder folder = await rootFolder.CreateFolderAsync("MySubFolder", CreationCollisionOption.OpenIfExists);

            // create a file, overwriting any existing file
            IFile file = await folder.CreateFileAsync("MyFile.png", CreationCollisionOption.ReplaceExisting);

            // populate the file with some text
            await file.WriteAllTextAsync(text);
            */
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

