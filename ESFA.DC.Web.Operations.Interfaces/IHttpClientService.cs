using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Auditing;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpRawResponse> SendDataAsyncRawResponse<T>(string url, T data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task<string> SendAsync(string url, CancellationToken cancellationToken);

        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken);

        Task<string> GetDataAsync(string url, CancellationToken cancellationToken);

        Task<string> PutDataAsync<T>(string url, T data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task<string> SendDataAsync<T>(string url, T data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task DeleteAsync(string url, CancellationToken cancellationToken);
    }
}
