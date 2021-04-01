using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleNaverMovieFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientID = "RyD4UcPGURVPP8_Ba01P";
            string clientSecret = "sDxeq4tCW6";

            string search = "starwars";
            string openApiUrl = $"https://openapi.naver.com/v1/search/movie?query={search}";

            string responseJson = GetRequestResult(openApiUrl, clientID, clientSecret);
            JObject parsedJson = JObject.Parse(responseJson);

            // Console.WriteLine(parsedJson);

            int total = Convert.ToInt32(parsedJson["total"]);
            int display = Convert.ToInt32(parsedJson["display"]);
            
            JArray json_array = (JArray) (parsedJson["items"]);

            for (int i = 0; i < display; i++)
                Console.WriteLine($"{json_array[i]}");
            for (int i = 0; i < display; i++)
            {
                Console.WriteLine($"{(json_array[i] as JObject)["title"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["image"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["subtitle"]}");
                Console.WriteLine($"{(json_array[i] as JObject)["actor"]}");
            }

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
                Console.WriteLine($"예외발생 : {ex}");
            }

            return result;
        }
    }
}
