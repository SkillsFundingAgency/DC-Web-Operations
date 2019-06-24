using System;
using System.Net.Http;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class PeriodEndService : BaseHttpClientService, IPeriodEndService
    {
        private readonly string _baseUrl;

    //    private string mockData = @"[{
    //            'PathId': '1',
    //        'Name': 'Critical Path',
    //        'PathItems': [{
    //        'PathId': '1',
    //        'Name': 'Item1',
    //        'Ordinal': '1',
    //        'PathItemJobs': [{
    //            'JobId': '123',
    //            'Status': '1'
    //        },
    //        {
    //            'JobId': '124',
    //            'Status': '2'
    //        }
    //        ]
    //    },
    //    {
    //        'PathId': '2',
    //        'Name': 'Item2',
    //        'Ordinal': '2',
    //        'PathItemJobs': []
    //    },
    //    {
    //        'PathId': '3',
    //        'Name': 'Item3',
    //        'Ordinal': '3',
    //        'PathItemJobs': []
    //    }
    //    ]
    //}]";

        public PeriodEndService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task Proceed(int startIndex = 0)
        {
            await GetDataAsync(_baseUrl + "/api/periodend/proceed/" + startIndex);
        }

        public async Task<string> GetPathItemStates()
        {
            var data = await GetDataAsync(_baseUrl + "/api/periodend/getStates/");
            return data;
        }
    }
}