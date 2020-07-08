using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public abstract class BaseHttpClientService
    {
        protected readonly IJsonSerializationService _jsonSerializationService;
        protected readonly HttpClient _httpClient;

        public BaseHttpClientService(
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient)
        {
            _jsonSerializationService = jsonSerializationService;
            _httpClient = httpClient;
        }

        public async Task<string> SendDataAsync(string url, object data, CancellationToken cancellationToken)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpRawResponse> SendDataAsyncRawResponse(string url, object data, CancellationToken cancellationToken)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            var rawResponse = new HttpRawResponse()
            {
                StatusCode = (int)response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };

            return rawResponse;
        }

        public async Task<string> SendAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDataAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(new Uri(url), cancellationToken);
            response.EnsureSuccessStatusCode();

            return response.StatusCode == HttpStatusCode.NoContent ? null : await response.Content.ReadAsStringAsync();
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync(url, cancellationToken);

            return string.IsNullOrWhiteSpace(data) ? default(T) : _jsonSerializationService.Deserialize<T>(data);
        }

        public async Task<string> PutDataAsync(string url, object data, CancellationToken cancellationToken)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}