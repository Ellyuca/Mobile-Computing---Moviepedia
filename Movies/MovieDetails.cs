using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;

namespace Movies
{
    [Activity(Label = "MovieDetails")]
    public class MovieDetails : Activity
    {
        string movieId;
        TextView moviePlot;
        TextView moviePlot2;
        TextView movieDetails;
        ImageView movieImage;
        WebClient webClient;
       
        private MovieInfo all_Info;
        protected override  async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MovieDetails);
            //movieId = FindViewById<TextView>(Resource.Id.movieId);

            movieId = Intent.GetStringExtra("MovieId");

            string url = "http://www.omdbapi.com/?i=" + movieId + "&r=json";       

            all_Info = await GetInfoAsync(url);

            movieDetails = FindViewById<TextView>(Resource.Id.details);
            movieDetails.Text = all_Info.Title;

            moviePlot = FindViewById<TextView>(Resource.Id.moviePlot);
            moviePlot2 = FindViewById<TextView>(Resource.Id.moviePlot2);
            if (all_Info.Plot.ToLower().Equals("n/a"))
                
            {
                moviePlot.Text = "Plot currently not available";
              
            }
            else
            {
                moviePlot.Text = all_Info.Plot;

            }
   
            moviePlot2.Text = all_Info.Plot;
            movieImage = FindViewById<ImageView>(Resource.Id.movieImage);

            //Button clickButton = FindViewById<Button>(Resource.Id.downloadImage);
            //clickButton.Click += downloadAsync(this, this,all_Info.Plot.ToString());
            Uri uriResult;
            bool validUrl = Uri.TryCreate(all_Info.Poster, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (validUrl)
                downloadAsync(all_Info.Poster);
            else
                movieImage.SetImageResource(Resource.Drawable.No_poster);



        }

        private async Task<MovieInfo> GetInfoAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response: 
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    StreamReader reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();
                    Console.Out.WriteLine("Response string: {0}", text);

                    MovieInfo all_Info = JsonConvert.DeserializeObject<MovieInfo>(text);

                    /* Per test */
                    Console.Out.WriteLine("Reserialized: {0}", JsonConvert.SerializeObject(all_Info));
                    Console.Out.WriteLine("First movie: {0}", all_Info.Title);

                    return all_Info;
                }
            }
        }
        /* private Bitmap GetImageBitmapFromUrl(string url)
         {
             Bitmap imageBitmap = null;

             using (var webClient = new WebClient())
             {
                 var imageBytes = webClient.DownloadData(url);
                 if (imageBytes != null && imageBytes.Length > 0)
                 {
                     imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                 }
             }

             return imageBitmap;
         }*/
       /* private static  bool isUrl(this string str)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(str, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }*/
        private async void downloadAsync(string myUrl)
        {
            webClient = new WebClient();
            var url = new Uri(myUrl);
            byte[] bytes = null;            
            
            try
            {
                bytes = await webClient.DownloadDataTaskAsync(url);
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());           
                return;
            }
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localFilename = "downloaded.png";
            string localPath = System.IO.Path.Combine(documentsPath, localFilename);
            

            //Sive the Image using writeAsync
            FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
            await fs.WriteAsync(bytes, 0, bytes.Length);

            Console.WriteLine("localPath:" + localPath);
            fs.Close();

            
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            await BitmapFactory.DecodeFileAsync(localPath, options);

            options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / movieImage.Height : options.OutWidth / movieImage.Width;
            options.InJustDecodeBounds = false;

            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(localPath, options);

            Console.WriteLine("Loaded!");

            movieImage.SetImageBitmap(bitmap);
                     

            
        }


    }
}