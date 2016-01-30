using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using System.Json;
using System.Collections;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Android.Views.InputMethods;

namespace Movies
{
    [Activity(Label = "Movies",  Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private SearchResponse results;
        LinearLayout mLinearLayout;
        LinearLayout mTitleSectionLayout;
        LinearLayout mGetSectionLayout;
      
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            // Get the Title EditBox and button resources:
            mLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainView);
            mTitleSectionLayout = FindViewById<LinearLayout>(Resource.Id.titleSection);
            mGetSectionLayout = FindViewById<LinearLayout>(Resource.Id.getSection);
            
            mLinearLayout.Click += (sender, e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            };

            mTitleSectionLayout.Click += (sender, e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            };
            mGetSectionLayout.Click += (sender, e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            };
           
            EditText movieTitle = FindViewById<EditText>(Resource.Id.titleText);
            ListView listaFilm = FindViewById<ListView>(Resource.Id.listaFilm);
            Button searchMoviesButton = FindViewById<Button>(Resource.Id.getMoviesButton);

            searchMoviesButton.Click += async (sender, e) => {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);

                if (isConnected())
                {


                    // Get the title text  entered by the user and create a query.
                    string url = "http://www.omdbapi.com/?s=" + movieTitle.Text + "&r=json" + "&page=3";

                    // Fetch the weather information asynchronously, 
                    // parse the results, then update the screen:

                    results = await FetchDataMoviesAsync(url);
                    // int size = results.Search.Count;
                    // string[] items = new string[size];
                    /* results.Search.ForEach(delegate (Movie current_Movie)
                     {

                         Console.Out.WriteLine("elenco film trovati: {0}", current_Movie.Title);
                     });*/

                    string[] items = results.Search.Select(x => x.Title + " (" + x.Year + ")").ToArray();

                    //items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };

                    Console.Out.WriteLine("vedere se funziona il parse : {0} {1} {2}", results.totalResults, results.Response, results.Error);

                    listaFilm.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);



                    //ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);

                    // ParseAndDisplay (json);
                }
                else
                {
                     DisplayAlert("Alert", "You need INTERNET CONNECTION", "OK");
                }
                
                

            };

            listaFilm.ItemClick += (sender, e) => {
                //Console.Out.WriteLine("detagli e: {0}", e);
                // var t = e.Position.ToString();

                // Android.Widget.Toast.MakeText(this,results.Search[e.Position].imdbID  , Android.Widget.ToastLength.Short).Show();

                Intent intent = new Intent(this, typeof(MovieDetails));
                intent.PutExtra("MovieId", results.Search[e.Position].imdbID);
                this.StartActivity(intent);
                //this.OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            };

            
        }

        private void DisplayAlert(string v1, string v2, string v3)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(v1);
            alert.SetMessage(v2);
            alert.SetNeutralButton(v3, (senderAlert, args) => {
                  alert.Dispose();
            });
                
            Dialog dialog = alert.Create();
            dialog.Show();
        }
        public bool isConnected()
        {      
            
                try
                {
                    Android.Net.ConnectivityManager connectivityManager = (Android.Net.ConnectivityManager)GetSystemService(ConnectivityService);
                    Android.Net.NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                    bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
                    return isOnline;
                }
                catch (Exception e)
                {

                    Console.Out.WriteLine(e.ToString());
                    return false;
                }
            
        }

        private async Task<SearchResponse> FetchDataMoviesAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response: 
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    StreamReader reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();
                    Console.Out.WriteLine("Response string: {0}", text);

                    SearchResponse results = JsonConvert.DeserializeObject<SearchResponse>(text);

                    /* Per test */
                    Console.Out.WriteLine("Reserialized: {0}", JsonConvert.SerializeObject(results));
                    Console.Out.WriteLine("First movie: {0}", results.Search.GetRange(0, 1)[0].Title);

                    return results;
                }
            }
        }

        
        }
}


