using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Automater
{

  public static async Task<List<string>> GetCorrectAnswers(string[] potentialAnswers)
  {
    string apiKey = "";

    string prompt = "";

    HttpClient httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    var requestUri = new Uri($"https://api.openai.com/v1/completions");

    var requestBody = new
    {
      prompt = prompt,
      temperature = 0.5,
      max_tokens = 5,
      n = 1,
      stop = "."
    };

    var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync(requestUri, requestContent);

    if(response.IsSuccessStatusCode)
    {
      var responseContent = await response.Content.ReadAsStringAsync();

      dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);

      string? text = responseObject?.choices[0].text;

      return text; 
    }
    else
    {
      throw new Exception($"Error: {response.ReasonPhrase}");
    }
  }
}
