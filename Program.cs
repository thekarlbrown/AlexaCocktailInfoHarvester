using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlexaCocktailInfoHarvester
{
    class MainClass
    {
        private static readonly string COCKTAILDB_API_URL_WITHOUT_ID = "http://www.thecocktaildb.com/api/json/v1/1/lookup.php?i=";
        private static readonly int COCKTAILDB_STARTING_IDDRINK = 11000;
        private static List<JObject> CocktailsSoFar = new List<JObject>();

        public static void Main(string[] args)
        {
            var currentCocktailIdDrink = COCKTAILDB_STARTING_IDDRINK;
            try 
            {
                // Utilize an arbitrarily large number to insure we iterate long enough to hit every single drink in the DB
                while (currentCocktailIdDrink < 100000000)
                {
                    var currentCocktailJson = GetJsonFromIdDrink(currentCocktailIdDrink);
					CocktailsSoFar.Add(currentCocktailJson);
                    currentCocktailIdDrink++;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Our current attempt to continually harvest Cocktail JSON has failed " +
                                  $"or the list of cocktails has run out. The current cocktailId is {currentCocktailIdDrink}" +
                                  " and the name of last drink harvested was " +
                                  $"{CocktailsSoFar[CocktailsSoFar.Count - 1]["drinks"].First["strDrink"]}. Error message: " +
                                  $"{ex.Message}");
                var fileName = $@"JsonOutput--CocktailDB--{DateTime.Now.ToString().Replace(' ', '-').Replace('/', '-')}.txt";
                var json = JsonConvert.SerializeObject(CocktailsSoFar);
                File.WriteAllText(fileName, json);
            }
        }

        public static JObject GetJsonFromIdDrink(int cocktailIdDrink)
        {
            JObject currentCocktail;
            using (var wc = new WebClient())
            {
                currentCocktail = JObject.Parse(wc.DownloadString($"{COCKTAILDB_API_URL_WITHOUT_ID}{cocktailIdDrink}"));
            }
            // Throw a breakpoint here, this is where you need to start checking (Cocktails begin with Y) because you are near the end of the list
            // Unfortunately, the values do not go from 11000 to the final value incrementing by 1, so it's hard to tell when it ends without manually checking
            // This would be a lot easier to automate if the creator of the Cocktail DB had been organized, you need to keep an eye on it and 
            // Save the final CocktailsSoFar to a file via the Console
            if (currentCocktail["drinks"].HasValues == true && currentCocktail["drinks"][0]["strDrink"].ToString()[0] == 'Y')
            {
                var waitHere = "Wait here";
            }
            return currentCocktail;
        }
    }
}
