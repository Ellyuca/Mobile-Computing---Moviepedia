﻿using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Android.Views.InputMethods;

namespace Movies
{
    [Activity(Label = "Movies", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private SearchResponse results;
        LinearLayout mLinearLayout;
        LinearLayout mTitleSectionLayout;
        LinearLayout mGetSectionLayout;
        ListView mListaLayout;
        ListView listaFilm;
        List<string> items;
        int pageNumber;
        EditText movieTitle;
        int scrollNumber;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);  
            
            mLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainView);
            mTitleSectionLayout = FindViewById<LinearLayout>(Resource.Id.titleSection);
            mGetSectionLayout = FindViewById<LinearLayout>(Resource.Id.getSection);
            mListaLayout = FindViewById<ListView>(Resource.Id.listaFilm);

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

            movieTitle = FindViewById<EditText>(Resource.Id.titleText);
            listaFilm = FindViewById<ListView>(Resource.Id.listaFilm);
            Button searchMoviesButton = FindViewById<Button>(Resource.Id.getMoviesButton);

            searchMoviesButton.Click += async (sender, e) =>
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);

                pageNumber = 1;
                if (isConnected())
                {
                    if (movieTitle.Length() > 1)
                    {

                        // Get the title text  entered by the user and create a query.
                        string url = "http://www.omdbapi.com/?s=" + movieTitle.Text + "&r=json" + "&page=" + pageNumber.ToString();
                        
                        // Fetch the weather information asynchronously, 
                        // parse the results, then update the screen:

                        results = await FetchDataMoviesAsync(url);
                        

                        if (results != null)
                        {
                            if (results.Response == "False")
                            {
                                listaFilm.SetAdapter(null);
                                DisplayAlert("Alert", "No results were found", "OK");
                            }
                            else
                            {
                                items = new List<string>();
                                items = results.Search.Select(x => x.Title + " (" + x.Year + ")").ToList();                              
                                if (Convert.ToInt16(results.totalResults) > 10)
                                    items.Add("Load more items...");

                               // Console.Out.WriteLine("vedere se funziona il parse : {0} {1} {2}", results.totalResults, results.Response, results.Error);

                                listaFilm.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
                                
                            }
                        }
                    }
                    else
                    {
                        DisplayAlert("Alert", "You need to insert more than 1 character", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Alert", "You need INTERNET CONNECTION", "OK");
                }

                

            };

            listaFilm.ItemClick += async (sender, e) =>
            {
                
                if (e.Position == (items.Count - 1) && pageNumber != (Convert.ToInt16(results.totalResults) % 10 == 0 ? Convert.ToInt16(results.totalResults) / 10 : (Convert.ToInt16(results.totalResults) / 10) + 1))
                {
                    if (pageNumber < (Convert.ToInt16(results.totalResults) % 10 == 0 ? Convert.ToInt16(results.totalResults) / 10 : (Convert.ToInt16(results.totalResults) / 10) + 1) - 1)
                    {
                        pageNumber++;
                        string url = "http://www.omdbapi.com/?s=" + movieTitle.Text + "&r=json" + "&page=" + pageNumber.ToString();
                        SearchResponse resultsNuovi = await FetchDataMoviesAsync(url);
                        results.Search.AddRange(resultsNuovi.Search);
                        //Console.Out.WriteLine("I nuovi risultati messi insieme {0}", JsonConvert.SerializeObject(resultsNuovi));
                     
                        List<string> itemsNew = new List<string>();
                        itemsNew = resultsNuovi.Search.Select(x => x.Title + " (" + x.Year + ")").ToList();
                     
                        items.RemoveAt(e.Position);
                        scrollNumber = items.Count();
                        items.AddRange(itemsNew);
                        items.Add("Load more items...");

                        listaFilm.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
                       
                        listaFilm.SetSelection(scrollNumber);


                    }
                    else
                    if (pageNumber < (Convert.ToInt16(results.totalResults) % 10 == 0 ? Convert.ToInt16(results.totalResults) / 10 : (Convert.ToInt16(results.totalResults) / 10) + 1))
                    {
                        pageNumber++;
                        string url = "http://www.omdbapi.com/?s=" + movieTitle.Text + "&r=json" + "&page=" + pageNumber.ToString();
                        SearchResponse resultsNuovi = await FetchDataMoviesAsync(url);
                        results.Search.AddRange(resultsNuovi.Search);
                        
                        List<string> itemsNew = new List<string>();
                        itemsNew = resultsNuovi.Search.Select(x => x.Title + " (" + x.Year + ")").ToList();
                      
                        items.RemoveAt(e.Position);
                        items.AddRange(itemsNew);

                        listaFilm.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);

                    }
                }
                else {

                    Intent intent = new Intent(this, typeof(MovieDetails));
                    Console.Out.WriteLine("lunghezza di results : {0}", results.Search.Count());
                    intent.PutExtra("MovieId", results.Search[e.Position].imdbID);
                    this.StartActivity(intent);
                    
                }
            };


        }
        public override void OnBackPressed()
        {
            MoveTaskToBack(true);
            
        }

        
        private void DisplayAlert(string v1, string v2, string v3)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(v1);
            alert.SetMessage(v2);
            alert.SetNeutralButton(v3, (senderAlert, args) =>
            {
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
            try
            {
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
                       // Console.Out.WriteLine("Reserialized: {0}", JsonConvert.SerializeObject(results));
                       // Console.Out.WriteLine("First movie: {0}", results.Search.GetRange(0, 1)[0].Title);

                        return results;
                    }
                }
            }
            catch (WebException ex)
            {

                DisplayAlert("Alert", "Something went wrong with the server. Please try again later", "OK");
                return null;
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Something went wrong. Please try again later", "OK");
                return null;
            }


        }
    }
}



