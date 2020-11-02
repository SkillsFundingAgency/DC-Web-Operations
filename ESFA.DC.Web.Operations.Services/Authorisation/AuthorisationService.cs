using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Authorisation;
using ESFA.DC.Web.Operations.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.Authorisation
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorisationService(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IsAuthorisedForReport(IReport report)
        {
            return (await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, report.Policy)).Succeeded;
        }
    }
}
