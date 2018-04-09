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
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(FileImplementation))]
namespace WebViewSample.Droid
{
    public class FileImplementation : IFile
    {
        public FileImplementation() { }

        public string GetPath(string path, string fileName)
        {
            string path1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // app directory
            string path2 = path + "/" + fileName; // custom folder and file name
            string finalPath = Path.Combine(path1, path2);

            Console.WriteLine(finalPath);

            return finalPath;
        }

        public string GetPath(string fileName)
        {
            string path1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // app directory
            string path2 = fileName; // custom folder and file name
            string finalPath = Path.Combine(path1, path2);

            Console.WriteLine(finalPath);

            return finalPath;
        }

        public async void WriteFile(string path, string fileName, string data)
        {
            if (!Directory.Exists(GetPath(path)))
            {
                Directory.CreateDirectory(GetPath(path));
            }

            File.WriteAllText(GetPath(path, fileName), data); // writes to local storage
        }

        public string ReadFile(string file, int lineCount)
        {
            StreamReader fileReader = File.OpenText(file);

            if (fileReader == null)
                return null;
            else
            {
                List<string> result = new List<string>();
                string line = string.Empty;
                int ctr = 0;
                while ((line = fileReader.ReadLine()) != null)
                {
                    result.Add(line);
                    ctr++;
                    if (ctr >= lineCount) break;
                }
                if (line == null)
                {
                    if (fileReader != null)
                    {
                        fileReader.Dispose();
                    }
                }

                fileReader.Close();
                return line;
            }
        }

        public List<string> ReadFile(string file)
        {
            StreamReader fileReader = File.OpenText(file);

            if (fileReader == null)
                return null;
            else
            {
                List<string> result = new List<string>();
                string line = string.Empty;
                while ((line = fileReader.ReadLine()) != null)
                {
                    result.Add(line);
                }
                if (line == null)
                {
                    if (fileReader != null)
                    {
                        fileReader.Dispose();
                    }
                }

                fileReader.Close();
                return result;
            }
        }

        public bool FileExists(string path, string fileName)
        {
            return File.Exists(GetPath(path, fileName));
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(GetPath(fileName));
        }

        public void SaveImage(string path, string fileName, string imageURL)
        {
            var webClient = new System.Net.WebClient();

            webClient.DownloadDataCompleted += (s, e) => {
                try
                {
                    var bytes = e.Result; // get the downloaded data
                    string localPath = GetPath(path, fileName);
                    File.WriteAllBytes(localPath, bytes); // writes to local storage
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException.Message);
                }
                
            };

            var url = new Uri(imageURL);

            webClient.DownloadDataAsync(url);
        }

        public void SaveImage(string fileName, string imageURL)
        {
            var webClient = new System.Net.WebClient();

            webClient.DownloadDataCompleted += (s, e) => {
                try
                {
                    var bytes = e.Result; // get the downloaded data
                    string localPath = GetPath("images", fileName);
                    File.WriteAllBytes(localPath, bytes); // writes to local storage
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException.Message);
                }

            };

            var url = new Uri(imageURL);

            webClient.DownloadDataAsync(url);
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

