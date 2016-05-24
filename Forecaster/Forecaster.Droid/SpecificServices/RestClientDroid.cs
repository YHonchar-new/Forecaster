using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Forecaster.Droid.SpecificServices;
using Forecaster.Models;
using Forecaster.Resources;
using RestSharp;
using RestClientBase = Forecaster.Services.RestClient;

[assembly: Xamarin.Forms.Dependency(typeof(RestClientDroid))]
namespace Forecaster.Droid.SpecificServices
{
    public class RestClientDroid : RestClientBase
    {
        private readonly RestClient _client;

        public RestClientDroid()
        {
            _client = new RestClient(BaseUrl);
        }

        public override async Task<IEnumerable<City>> FindCitiesAsync(string cityName)
        {
            var tcs = new TaskCompletionSource<IEnumerable<City>>();

            RestRequest request = new RestRequest(string.Format(RestApiConstants.FindManyCities, cityName, AppId));
            request.AddQueryParameter(RestApiConstants.UnitsParam, UnitsType);
            request.RequestFormat = DataFormat.Json;
            var response = await _client.ExecuteTaskAsync<string>(request);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("No cities with such name were not found");
                var cities = ExtractCities(response.Data);
                tcs.SetResult(cities);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            return await tcs.Task;
        }
    }
}