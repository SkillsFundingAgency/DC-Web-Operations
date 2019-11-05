using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Provider;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.EntityFrameworkCore;
using Organisation = ESFA.DC.CollectionsManagement.Models.Organisation;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class AddNewProviderService : BaseHttpClientService, IAddNewProviderService
    {
        private readonly ILogger _logger;
        private readonly Func<IOrganisationsContext> _organisationsFactory;
        private readonly string _baseUrl;

        public AddNewProviderService(
                IJsonSerializationService jsonSerializationService,
                ILogger logger,
                ApiSettings apiSettings,
                Func<IOrganisationsContext> organisationsFactory,
                HttpClient httpClient)
                : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _organisationsFactory = organisationsFactory;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<ProviderSearchResult>> GetProviderSearchResults(string query)
        {
            var sw = new Stopwatch();
            sw.Start();

            _logger.LogVerbose("Entered GetProviderSearchResults");

            string alternativeQuery = null;
            query = query.ToLower();

            if (query.Contains(" ltd"))
            {
                alternativeQuery = query.Replace(" ltd", " limited");
            }
            else if (query.Contains(" limited"))
            {
                alternativeQuery = query.Replace(" limited", " ltd");
            }

            var results = new List<ProviderSearchResult>();

            using (var context = _organisationsFactory())
            {
                results = await context.MasterOrganisations
                    .Include(o => o.OrgDetail)
                    .ThenInclude(x => x.UkprnNavigation.OrgUkprnUpins)
                    .Where(o => o.OrgDetail != null
                                && o.OrgUkprnUpins.Any(upin => upin.Status.Equals("Active"))
                                && (o.OrgDetail.Name.Contains(query)
                                    || o.OrgDetail.Name.Contains(alternativeQuery)
                                    || o.OrgDetail.Ukprn.ToString().Contains(query)
                                    || o.OrgUkprnUpins.Any(up => up.Upin.ToString().Contains(query))))
                    .Select(o => new ProviderSearchResult(
                        o.OrgDetail.Name,
                        o.Ukprn,
                        o.OrgUkprnUpins == null ? 0 : o.OrgUkprnUpins.First(p => p.Status.Equals("Active")).Upin))
                    .Take(10)
                    .ToListAsync();
            }

            sw.Stop();
            _logger.LogVerbose($"Exit GetProviderSearchResults. Time Taken(ms): {sw.ElapsedMilliseconds}");

            return results;
        }

        public async Task<HttpRawResponse> SaveProvider(string providerName, long ukprn, int? upin, bool isMca, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organisationDto = new Organisation() { Name = providerName, Ukprn = ukprn, IsMca = isMca };
            return await SendDataAsyncRawResponse($"{_baseUrl}/api/org/add", organisationDto, cancellationToken);
        }
    }
}
