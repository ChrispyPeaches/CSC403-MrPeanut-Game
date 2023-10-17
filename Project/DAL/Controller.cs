// imports
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fall2020_CSC403_Project
{
    // LOCAL models and other variables -- use
    // models to format any jsons from requests, etc

    public partial class Controller
    {
        public class HTTPRequests : Controller
        {
            public class OpenAI : HTTPRequests
            {
                private static readonly HttpClient httpClient = new HttpClient();

                public async Task<string> MakeOpenAIApiRequest(string userMessage = "", string bossName = "", string bossPreviousResponse = "")
                {
                    string url = "PUT THE URL HERE!!!!!";

                    httpClient.DefaultRequestHeaders.Add("Authorization", "PUT THE KEY HERE!!!!");
                    httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    var requestBody = new
                    {
                        text = userMessage
                    };

                    try
                    {
                        var response = await httpClient.PostAsJsonAsync(url, requestBody);
                        response.EnsureSuccessStatusCode();
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading JSON file: " + ex.Message);
                    }

                    return null; // You should return a value for all code paths
                }
            }
        }

        public class GameData : Controller
        {
            public void SaveData(string pathToFile = "Save Data Name Here", string newData = "")
            {
                try
                {
                    if (!File.Exists(pathToFile))
                    {
                        File.WriteAllText(pathToFile, newData);
                    }
                    else
                    {
                        Console.WriteLine("Error: File already exists at the specified path.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving JSON file: " + ex.Message);
                }
            }

            public void UpdateData(string pathToFile = "Save Data Name Here", string newData = "")
            {
                try
                {
                    File.WriteAllText(pathToFile, newData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating JSON file: " + ex.Message);
                }
            }

            public static string RetrieveData(string pathToFile = "Save Data Name Here")
            {
                try
                {
                    string jsonString = File.ReadAllText(pathToFile);
                    return jsonString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading JSON file: " + ex.Message);
                }

                return null;
            }
        }
    }
}