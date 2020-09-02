using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Auditing;

namespace ESFA.DC.Web.Operations.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly HttpClient _httpClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public HttpClientService(
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
        {
            _jsonSerializationService = jsonSerializationService;
            _dateTimeProvider = dateTimeProvider;
            _httpClient = httpClient;
        }

        public async Task<string> SendDataAsync<T>(string url, T data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null)
        {
            var json = _jsonSerializationService.Serialize(data);

            using (var content = BuildContent(json, username, differentiator))
            {
                var response = await _httpClient.PostAsync(url, content, cancellationToken);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<HttpRawResponse> SendDataAsyncRawResponse<T>(string url, T data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null)
        {
            var json = _jsonSerializationService.Serialize(data);

            using (var content = BuildContent(json, username, differentiator))
            {
                var response = await _httpClient.PostAsync(url, content, cancellationToken);

                var rawResponse = new HttpRawResponse
                {
                    StatusCode = (int)response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode,
                    Content = await response.Content.ReadAsStringAsync()
                };

                return rawResponse;
            }
        }

        public async Task<string> SendAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsync(url, null, cancellationToken);
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

        public async Task<string> PutDataAsync<T>(string url, T data, CancellationToken cancellationToken, string userName = null, DifferentiatorPath? differentiator = null)
        {
            var json = _jsonSerializationService.Serialize(data);
            using (var content = BuildContent(json, userName, differentiator))
            {
                var response = await _httpClient.PutAsync(url, content, cancellationToken);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        private HttpContent BuildContent(string content, string userName, DifferentiatorPath? differentiator)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            if (!string.IsNullOrWhiteSpace(userName))
            {
                httpContent.Headers.Add("AuditUsername", userName);
            }

            if (differentiator != null)
            {
                var differentiatorInt = (int)differentiator;
                httpContent.Headers.Add("AuditDifferentiator", differentiatorInt.ToString());
            }

            httpContent.Headers.Add("AuditDateTime", _dateTimeProvider.GetNowUtc().ToString());

            return httpContent;
        }
    }
}