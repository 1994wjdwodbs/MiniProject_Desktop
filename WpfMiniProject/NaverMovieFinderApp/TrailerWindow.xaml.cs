using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls;
using NaverMovieFinderApp.Helper;
using NaverMovieFinderApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NaverMovieFinderApp
{
    /// <summary>
    /// TrailerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrailerWindow : MetroWindow
    {
        List<YoutubeItem> youtubes; // 유튜브 API 검색결과 리스트

        public TrailerWindow()
        {
            InitializeComponent();
        }

        public TrailerWindow(string movie) : this()
        {
            LblMovieName.Content = $"{movie} 예고편";
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            youtubes = new List<YoutubeItem>(); // 초기화
            ProcSearchYoutubeApi();
        }

        private async void ProcSearchYoutubeApi()
        {
            await LoadDataCollection();
            LsvYoutubeSearch.ItemsSource = youtubes;
        }

        // Task
        // 비동기 작업을 나타냅니다. 이 유형에 대한.NET Framework 소스 코드를 찾아보려면 참조는 Reference Source합니다.
        private async Task LoadDataCollection()
        {
            // key : AIzaSyCLYMpoH-7OqiCgujQUUwBUZ6rIDryld-E
            // Entity Framework 6.2.0  으로 다운그레이드 ...
            var youtubeService = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyCLYMpoH-7OqiCgujQUUwBUZ6rIDryld-E",
                    ApplicationName = this.GetType().ToString()
                }
            );

            var request = youtubeService.Search.List("snippet");
            request.Q = LblMovieName.Content.ToString(); // "{영화이름} 예고편" 으로 검색
            request.MaxResults = 10;

            var response = await request.ExecuteAsync(); // 유튜브 API 호출
            // youtubes = new List<YoutubeItem>(); // 초기화

            foreach (var item in response.Items)
            {
                if(item.Id.Kind.Equals("youtube#video"))
                {
                    YoutubeItem youtube = new YoutubeItem()
                    {
                        Title = item.Snippet.Title,
                        Author = item.Snippet.ChannelTitle,
                        URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}"
                    };

                    youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute));

                    youtubes.Add(youtube);
                }
            }
        }

        private void LsvYoutubeSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LsvYoutubeSearch.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유튜브보기", "예고편을 볼 영화를 선택하세요");
                return;
            }

            if (LsvYoutubeSearch.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("유튜브보기", "예고편을 하나만 선택하세요");
                return;
            }

            if(LsvYoutubeSearch.SelectedItem is YoutubeItem)
            {
                var video = LsvYoutubeSearch.SelectedItem as YoutubeItem;
                BrwYoutubeWatch.Source = new Uri(video.URL, UriKind.RelativeOrAbsolute);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BrwYoutubeWatch.Source = null;
            BrwYoutubeWatch.Dispose();
        }
    }
}
