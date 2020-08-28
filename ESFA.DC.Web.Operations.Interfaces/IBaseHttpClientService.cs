    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Auditing;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IBaseHttpClientService
    {
        Task<string> SendDataAsync(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task<HttpRawResponse> SendDataAsyncRawResponse(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task<string> SendAsync(string url, CancellationToken cancellationToken);

        Task<string> GetDataAsync(string url, CancellationToken cancellationToken);

        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken);

        Task<string> PutDataAsync(string url, object data, CancellationToken cancellationToken);

        Task DeleteAsync(string url, CancellationToken cancellationToken);

        Task<TResult> PostAsync<TContent, TResult>(string baseUrl, TContent content, IEnumerable<string> segments = null, CancellationToken cancellationToken = default);

        Task<TResult> GetAsync<TResult>(string baseUrl, IEnumerable<string> segments = null, CancellationToken cancellationToken = default);
    }
}
