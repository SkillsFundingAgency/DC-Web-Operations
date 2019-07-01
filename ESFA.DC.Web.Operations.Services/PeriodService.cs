using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodService : BaseHttpClientService, IPeriodService
    {
        private readonly string _baseUrl;

        public PeriodService(
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<PathYearPeriod> ReturnPeriod(DateTime dateUtc, CancellationToken cancellationToken = default)
        {
            var collectionYear = GetCollectionYearFromDate(dateUtc);

            var collectionName = Constants.IlrCollectionNamePrefix + collectionYear;

            var dateString = DateHelper.GetUrlFriendlyDate(dateUtc);

            var returnPeriod = _jsonSerializationService.Deserialize<ReturnPeriod>(
                await GetDataAsync(
                    $"{_baseUrl}/api/returns-calendar/{collectionName}/{dateString}",
                    cancellationToken));

            var pathYearPeriod = new PathYearPeriod
            {
                Period = returnPeriod.PeriodNumber,
                Year = collectionYear
            };

            return pathYearPeriod;
        }

        private int GetCollectionYearFromDate(DateTime date)
        {
            int year = Convert.ToInt32(date.Year.ToString().Substring(2, 2));
            int month = date.Month;

            return month >= Constants.YearStartMonth ? Convert.ToInt32($"{year}{year + 1}") : Convert.ToInt32($"{year - 1}{year}");
        }
    }
}