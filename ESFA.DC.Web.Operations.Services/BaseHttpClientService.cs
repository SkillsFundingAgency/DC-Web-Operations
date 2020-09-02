using System;
using System.Collections.Generic;
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
using Flurl.Http;

namespace ESFA.DC.Web.Operations.Services
{
    public class BaseHttpClientService : IHttpClientService
    {
        protected readonly IJsonSerializationService _jsonSerializationService;
        protected readonly HttpClient _httpClient;
        private readonly IRouteFactory _routeFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BaseHttpClientService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
        {
            _routeFactory = routeFactory;
            _jsonSerializationService = jsonSerializationService;
            _dateTimeProvider = dateTimeProvider;
            _httpClient = httpClient;
        }

         public async Task<string> SendDataAsync(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null)
        {
            var json = _jsonSerializationService.Serialize(data);

            var content = BuildContent(json, username, differentiator);

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpRawResponse> SendDataAsyncRawResponse(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = BuildContent(json, username, differentiator);

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            var rawResponse = new HttpRawResponse
            {
                StatusCode = (int)response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };

            return rawResponse;
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

        public async Task<string> PutDataAsync(string url, object data, CancellationToken cancellationToken)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DeleteAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task<TResult> PostAsync<TContent, TResult>(
            string baseUrl,
            TContent content,
            IEnumerable<string> segments = null,
            CancellationToken cancellationToken = default)
        {
            var clientUrl = _routeFactory.BuildRoute(baseUrl, segments);

            return await clientUrl
                .PostJsonAsync(content, cancellationToken)
                .ReceiveJson<TResult>();
        }

        public async Task<TResult> GetAsync<TResult>(
            string baseUrl,
            IEnumerable<string> segments = null,
            CancellationToken cancellationToken = default)
        {
            var clientUrl = _routeFactory.BuildRoute(baseUrl, segments);

            return await clientUrl.GetJsonAsync<TResult>(cancellationToken);
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