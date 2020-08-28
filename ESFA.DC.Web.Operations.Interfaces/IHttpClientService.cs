using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Auditing;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpRawResponse> SendDataAsyncRawResponse(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task<string> SendAsync(string url, CancellationToken cancellationToken);

        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken);

        Task<string> GetDataAsync(string url, CancellationToken cancellationToken);

        Task<string> PutDataAsync(string url, object data, CancellationToken cancellationToken);

        Task<string> SendDataAsync(string url, object data, CancellationToken cancellationToken, string username = null, DifferentiatorPath? differentiator = null);

        Task DeleteAsync(string url, CancellationToken cancellationToken);
    }
}
