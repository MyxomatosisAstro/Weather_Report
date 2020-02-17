using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weather_Report
{
	// This is our generic API service, returning data from the SMHI API with a specified endpoint
    static class ApiService
    {
		public static async Task<string> GetDataAsync(string endPoint)
		{
			
			try
			{

				using (var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
				using (HttpResponseMessage response =
				    await httpClient.GetAsync(new Uri("http://opendata-download-metobs.smhi.se/api/" + endPoint)))
				{					
					return await response.Content.ReadAsStringAsync();
				}


			}

			// If the request fails prints the exception
			catch (HttpRequestException e)


			{
				Console.WriteLine(e);
				return string.Empty;
			}


		}
	}
}
