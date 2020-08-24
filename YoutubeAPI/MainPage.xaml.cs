using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Windows.UI.Popups;
using System.Net.NetworkInformation;
using Google.Apis.YouTube.v3.Data;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.ServiceModel.Channels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YoutubeAPI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = "AIzaSyDBf8bq5AKUSHfF_CF0eeZ2RCLzyfmOi5s",
            ApplicationName = "youtube"
        });
        List<Video> ListVideo = new List<Video>();
        private string TokenNextPage = null, TokenPriPage = null;
        public MainPage()
        {
            this.InitializeComponent();
            GetVideo();
        }

        public object NetworkInterFace { get; private set; }
        public string TokenPrivPage { get; private set; }

        private async void GetVideo(string PageToken = null)
        {
            try
            {
                if (NetworkInterFace.GetIsNetworkAvailable())
                {
                    var request = youtubeService.Search.List("snippet");
                    request.ChannelId = "UCsooa4yRKGN_zEE8iknghZA";
                    request.MaxResults = 25;
                    request.Type = "video";
                    request.Order = SearchResource.ListRequest.OrderEnum.Date;
                    request.PageToken = PageToken;
                    var Result = await request.ExecuteAsync();
                    if (Result.NextPageToken != null)
                        TokenNextPage = Result.NextPageToken;
                    if (Result.PrevPageToken != null)
                        TokenPrivPage = Result.PrevPageToken;
                    foreach(var item in Result.Items)
                    {
                        ListVideo.Add(new Video
                        {
                            Title = item.Snippet.Title,
                            Id = item.Id.VideoId,
                            Img = item.Snippet.Thumbnails.Default__.Url
                        });
                    }
                    lv.ItemsSource = null;
                    lv.ItemsSource = ListVideo;
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Check your internet connection");
                    await msg.ShowAsync();
                }
            }
            catch { }
        }
        private void lv_ItemClick(object sender, ItemClickEventArgs e)
        {
            Video video = e.ClickedItem as Video;
            Frame.Navigate(typeof(VideoPage), video);
        }

        
    }
}
