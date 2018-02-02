using System;
using System.Net.Http;
using Newtonsoft.Json;

using Xamarin.Forms;
using PCLStorage;
using System.Collections.Generic;

namespace WebViewSample
{
    public class InAppBrowserCode : ContentPage
    {

        //this needs to be defined at class level for use within methods.
        private WebView webView;
        private RootObject result;
        private string updateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewSample.InAppBrowserXaml"/> class.
        /// Takes a URL indicating the starting page for the browser control.
        /// </summary>
        /// <param name="URL">URL to display in the browser.</param>
        public InAppBrowserCode(string URL)
        {
            this.Title = "Browser";
            var layout = new StackLayout();
            var controlBar = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var backButton = new Button { Text = "Back", HorizontalOptions = LayoutOptions.Start };
            backButton.Clicked += backButtonClicked;

            var forwardButton = new Button { Text = "Forward", HorizontalOptions = LayoutOptions.EndAndExpand };
            forwardButton.Clicked += forwardButtonClicked;

            getJSON(layout);

            controlBar.Children.Add(backButton);
            controlBar.Children.Add(forwardButton);

            layout.Children.Add(controlBar);

            Content = layout;
        }

        //get JSON data, parse it, and generate the webview
        async void getJSON(StackLayout layout)
        {
            HtmlWebViewSource htmlSource = new HtmlWebViewSource();

            using (var client = new HttpClient())
            {
                //download JSON
                HttpResponseMessage response = await client.GetAsync("http://codechameleon.com/dev/fwt/siteContents.php");
                HttpResponseMessage response2 = await client.GetAsync("http://codechameleon.com/dev/fwt/lastUpdate.php");

                response.EnsureSuccessStatusCode();
                response2.EnsureSuccessStatusCode();

                using (HttpContent content = response2.Content)
                {
                    //convert data to string
                    string responseBody = await response.Content.ReadAsStringAsync();
                    string responseBody2 = await response2.Content.ReadAsStringAsync();

                    Update updateObject = JsonConvert.DeserializeObject<Update>(responseBody2);
                    updateTime = updateObject.updateDate;
                    DependencyService.Get<FileInterface>().WriteFile("data", "update.txt", updateTime);
                }

                if (DependencyService.Get<FileInterface>().FileExists("data", "update.txt"))
                {
                    string updateFile = DependencyService.Get<FileInterface>().GetPath("data", "update.txt");

                    List<string> lines = DependencyService.Get<FileInterface>().ReadFile(updateFile);

                    string line = lines.ToString();
                }

                using (HttpContent content = response.Content)
                {
                    //convert data to string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<RootObject>(responseBody);
                    
                    string thing = ""; //stores JSON objects as HTML

                    foreach (var pages in result.contents.pages) // converts JSON objects to HTML used in the webview
                    {
                        thing += "<h1>" + pages.name + "</h1>";
                        thing += "<h3>" + pages.slug + "</h3>";
                        thing += "<h6>" + pages.coverImage + "</h6>";
                        thing += "<p>" + pages.body + "</p>";
                        thing += "<br />";
                    }

                    htmlSource.Html = @"<html><body>" + thing + "</body></html>"; //HTML source for the webview

                    //WebView needs to be given a height and width request within layouts to render
                    webView = new WebView() { WidthRequest = 1000, HeightRequest = 1000, Source = htmlSource }; //creates webview
                    layout.Children.Add(webView); //adds webview to layout
                }

                
                /*
                response = await client.GetAsync("http://layerseven.net/images/logo.png");

                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();

                string folderPath = Environment.ExternalStorageDirectory.AbsolutePath; //Android  
                string folderPath = Environment.ExternalStorageDirectory.AbsolutePath;

                DependencyService.Get<IWriteFile>().WriteFile(responseData);
                */
                
            }
        }

        /// <summary>
        /// fired when the back button is clicked. If the browser can go back, navigate back.
        /// If the browser can't go back, leave the in-app browser page.
        /// </summary>	
        void backButtonClicked(object sender, EventArgs e)
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }
            else
            {
                this.Navigation.PopAsync(); // closes the in-app browser view.
            }

        }


        /// <summary>
        /// Navigates forward
        /// </summary>
        void forwardButtonClicked(object sender, EventArgs e)
        {
            if (webView.CanGoForward)
            {
                webView.GoForward();
            }
        }
    }
}


