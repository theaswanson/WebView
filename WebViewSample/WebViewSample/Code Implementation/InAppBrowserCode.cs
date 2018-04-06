using System;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

using Xamarin.Forms;
using PCLStorage;
using System.Collections.Generic;

namespace WebViewSample
{
    public class InAppBrowserCode : ContentPage
    {
        //this needs to be defined at class level for use within methods.
        private WebView myWebView;
        private RootObject JSONData;
        private string UpdateTime;

        bool LiveMode = false;
        string UpdateTimeLink;
        string SiteContentsLink;

        public string UpdateTimeLinkDev = "https://fwt.codechameleon.com/api-last-update/";
        public string SiteContentsLinkDev = "https://fwt.codechameleon.com/api-content/";

        public string UpdateTimeLinkLive = "https://fwtrails.org/api-last-update/";
        public string SiteContentsLinkLive = "https://fwtrails.org/api-content/";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewSample.InAppBrowserXaml"/> class.
        /// Takes a URL indicating the starting page for the browser control.
        /// </summary>
        /// <param name="URL">URL to display in the browser.</param>
        public InAppBrowserCode(string URL)
        {
            if (LiveMode == true)
            {
                UpdateTimeLink = UpdateTimeLinkLive;
                SiteContentsLink = SiteContentsLinkLive;
            }
            else
            {
                UpdateTimeLink = UpdateTimeLinkDev;
                SiteContentsLink = SiteContentsLinkDev;
            }

            this.Title = "Browser";
            var layout = new StackLayout();
            var controlBar = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var backButton = new Button { Text = "Back", HorizontalOptions = LayoutOptions.Start };
            backButton.Clicked += backButtonClicked;

            var forwardButton = new Button { Text = "Forward", HorizontalOptions = LayoutOptions.EndAndExpand };
            forwardButton.Clicked += forwardButtonClicked;

            GetJSON(layout);

            controlBar.Children.Add(backButton);
            controlBar.Children.Add(forwardButton);

            layout.Children.Add(controlBar);

            Content = layout;
        }

        async void GetJSON(StackLayout layout)
        {
            HtmlWebViewSource HTMLSource = new HtmlWebViewSource();
            
            var client = new HttpClient();
            HttpResponseMessage ContentsResponse = await client.GetAsync(SiteContentsLink);
            HttpResponseMessage LastUpdateResponse = await client.GetAsync(UpdateTimeLink);
            ContentsResponse.EnsureSuccessStatusCode();
            LastUpdateResponse.EnsureSuccessStatusCode();

            string UpdateFolder = "data";
            string UpdateFile = "update.txt";

            string StoredUpdateTimeRaw = "";
            string UpdateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime StoredUpdateTime;
            DateTime NewUpdateTime;

            string lastUpdateBody = await LastUpdateResponse.Content.ReadAsStringAsync();
            lastUpdateBody = lastUpdateBody.Substring(0, UpdateTimeFormat.Length);
            UpdateTime = lastUpdateBody;
            NewUpdateTime = DateTime.ParseExact(UpdateTime, UpdateTimeFormat, provider);

            if (DependencyService.Get<IFile>().FileExists(UpdateFolder, UpdateFile))
            {
                string UpdateFilePath = DependencyService.Get<IFile>().GetPath(UpdateFolder, UpdateFile);

                StoredUpdateTimeRaw = DependencyService.Get<IFile>().ReadFile(UpdateFilePath, 1);
                StoredUpdateTime = DateTime.ParseExact(StoredUpdateTimeRaw, UpdateTimeFormat, provider);

                if (DateTime.Compare(NewUpdateTime, StoredUpdateTime) > 0)
                {
                    DependencyService.Get<IFile>().WriteFile(UpdateFolder, UpdateFile, UpdateTime);
                }
            }
            else
            {
                DependencyService.Get<IFile>().WriteFile(UpdateFolder, UpdateFile, UpdateTime);
            }

            string JSONDataRaw = await ContentsResponse.Content.ReadAsStringAsync();
            JSONData = JsonConvert.DeserializeObject<RootObject>(JSONDataRaw);

            DependencyService.Get<IFile>().SaveImage(UpdateFolder, "image.jpg", "http://fwt.codechameleon.com/wp-content/uploads/2018/03/100-Miles-of-Trails-Event-42.jpg");

            string HTMLBody = "";

            foreach (var Page in JSONData.contents.pages)
            {
                HTMLBody += "<h1>" + Page.name + "</h1>";
                HTMLBody += "<h3>" + Page.slug + "</h3>";
                HTMLBody += "<h6>" + Page.coverImage + "</h6>";
                HTMLBody += "<p>" + Page.body + "</p>";
                HTMLBody += "<br />";
            }

            HTMLSource.Html = @"<html><body>" + HTMLBody + "</body></html>";

            myWebView = new WebView() { WidthRequest = 1000, HeightRequest = 1000, Source = HTMLSource };
            layout.Children.Add(myWebView);
        }

        /// <summary>
        /// fired when the back button is clicked. If the browser can go back, navigate back.
        /// If the browser can't go back, leave the in-app browser page.
        /// </summary>	
        void backButtonClicked(object sender, EventArgs e)
        {
            if (myWebView.CanGoBack)
            {
                myWebView.GoBack();
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
            if (myWebView.CanGoForward)
            {
                myWebView.GoForward();
            }
        }
    }
}


