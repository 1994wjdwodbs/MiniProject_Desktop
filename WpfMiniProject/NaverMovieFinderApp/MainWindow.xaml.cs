using MahApps.Metro.Controls;
using NaverMovieFinderApp.Helper;
using NaverMovieFinderApp.Model;
using Newtonsoft.Json.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NaverMovieFinderApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public bool btnFlag = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            StsResult.Content = string.Empty;

            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                StsResult.Content = "검색할 영화명을 입력 후, 검색 버튼을 눌러주세요.";
                return;
            }

            // Commons.ShowMessageAsync("결과", $"{TxtMovieName.Text}");
            try
            {
                btnFlag = true;
                ProSearchNaverApi(TxtMovieName.Text);
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외 발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void ProSearchNaverApi(string movieName)
        {
            string clientID = "RyD4UcPGURVPP8_Ba01P";
            string clientSecret = "sDxeq4tCW6";
            string openApiUrl = $"https://openapi.naver.com/v1/search/movie?query={movieName}";

            string responseJson = GetRequestResult(openApiUrl, clientID, clientSecret);
            // JObject : Represents a JSON object.
            // string -> JSON (JObject)
            JObject parsedJson = JObject.Parse(responseJson);

            // Console.WriteLine(parsedJson);

            int total = Convert.ToInt32(parsedJson["total"]);
            int display = Convert.ToInt32(parsedJson["display"]);

            // JArray :  Represents a JSON array.
            // JObject -> JArray
            JArray json_array = (JArray)(parsedJson["items"]);
            
            List<MovieItem> movieItems = new List<Model.MovieItem>();

            // JToken : Represents an abstract JSON token. 
            foreach (JToken item in json_array)
            {
                MovieItem movie = new Model.MovieItem(
                    // item["title"].ToString().Replace("<b>", "").Replace("</b>", ""),
                    Commons.StripHtmlTag(item["title"].ToString()),
                    item["link"].ToString(),
                    item["image"].ToString(),
                    item["subtitle"].ToString(),
                    item["pubDate"].ToString(),
                    Commons.StripPipe(item["director"].ToString()),
                    Commons.StripPipe(item["actor"].ToString()),
                    item["userRating"].ToString()
                );

                movieItems.Add(movie);
            }

            GrdData.DataContext = movieItems;

            /*for (int i = 0; i < display; i++)
                Console.WriteLine($"{json_array[i]}");
            for (int i = 0; i < display; i++)
            {
                Console.WriteLine($"{(json_array[i] as JObject)["title"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["image"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["subtitle"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["actor"]}");
            }*/
        }

        private static string GetRequestResult(string openApiUrl, string clientID, string clientSecret)
        {
            var result = string.Empty;

            try
            {
                WebRequest request = WebRequest.Create(openApiUrl);
                request.Headers.Add("X-Naver-Client-Id", clientID);
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                result = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"예외발생 : {ex}");
            }

            return result;
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnSearch_Click(sender, e);
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ImgPoster.Source = new BitmapImage(new Uri("../../No_Picture.jpg", UriKind.RelativeOrAbsolute));

            if(btnFlag)
            {
                btnFlag = false;
                return;
            }

            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("오류", "영화를 선택하세요");
                return;
            }

            if(GrdData.SelectedItem is MovieItem)
            {
                var movie = GrdData.SelectedItem as MovieItem;

                if (string.IsNullOrEmpty(movie.Image))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("../../No_Picture.jpg", UriKind.RelativeOrAbsolute));
                    return;
                }
                BitmapImage Poster = new BitmapImage();
                Poster.BeginInit();
                Poster.UriSource = new Uri(movie.Image, UriKind.RelativeOrAbsolute);
                Poster.EndInit();

                
                ImgPoster.Source = Poster;

            }
        }

        private void BtnAddWatchList_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능)");
                return;
            }

            List<NaverFavoriteMovies> favoriteMovies = new List<NaverFavoriteMovies>();

            foreach(MovieItem item in GrdData.SelectedItems)
            {
                NaverFavoriteMovies temp = new NaverFavoriteMovies()
                {
                    Title = item.Title,
                    Link = item.Link,
                    Image = item.Image,
                    SubTitle = item.SubTitle,
                    PubDate = item.PubDate,
                    Director = item.Director,
                    Actor = item.Actor,
                    UserRating = item.UserRating,
                    RegDate = DateTime.Now
                };

                favoriteMovies.Add(temp);
            }

            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    // AddRange
                    //     Adds the given collection of entities into context underlying the set with each
                    //     entity being put into the Added state such that it will be inserted into the
                    //     database when SaveChanges is called.
                    ctx.Set<NaverFavoriteMovies>().AddRange(favoriteMovies);
                    ctx.SaveChanges();
                }

                Commons.ShowMessageAsync("저장", "즐겨찾기에 추가했습니다.");
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void BtnViewWatchList_Click(object sender, RoutedEventArgs e)
        {
            btnFlag = true;

            GrdData.DataContext = null;
            TxtMovieName.Text = string.Empty;

            List<MovieItem> listData = new List<MovieItem>();
            List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    list = ctx.NaverFavoriteMovies.ToList();
                }
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }

            foreach (var item in list)
            {
                listData.Add(new MovieItem(
                    item.Title,
                    item.Link,
                    item.Image,
                    item.SubTitle,
                    item.PubDate,
                    item.Director,
                    item.Actor,
                    item.UserRating
                ));
            }

            GrdData.DataContext = listData;
        }
    }
}
