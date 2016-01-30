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

namespace Movies
{
   public class Movie
   {
        public string Poster { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string imdbID { get; set; }
       
    }

    public class SearchResponse {
        public List<Movie> Search;
        public string totalResults;
        public string Response;
        public string Error;
    }
}