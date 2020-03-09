using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobProcessingDetailService : BaseHttpClientService, IJobProcessingDetailService
    {
        private readonly string _baseUrl;

        public JobProcessingDetailService(ApiSettings apiSettings, IJsonSerializationService jsonSerializationService, HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetJobsThatAreProcessing(CancellationToken cancellationToken = default)
        {
            //return await GetDataAsync($"{_baseUrl}/api/job/jobsprocessingdetail", cancellationToken);

            var jobsList = new JobsList()
            {
                Jobs = new List<JobProcessingDetail>()
                {
                    new JobProcessingDetail()
                    {
                        ProviderName = "P1",
                        UkPrn = 1234,
                        Filename = "abc.xml",
                        ProcessingTime = 10
                    },
                    new JobProcessingDetail()
                    {
                        ProviderName = "P2",
                        UkPrn = 12342,
                        Filename = "abc2.xml",
                        ProcessingTime = 12
                    },
                    new JobProcessingDetail()
                    {
                        ProviderName = "P3",
                        UkPrn = 12342,
                        Filename = "abc2.xml",
                        ProcessingTime = 12
                    },
                    new JobProcessingDetail()
                    {
                        ProviderName = "P4",
                        UkPrn = 12342,
                        Filename = "abc2.xml",
                        ProcessingTime = 12
                    },
                    new JobProcessingDetail()
                    {
                        ProviderName = "P5",
                        UkPrn = 12342,
                        Filename = "abc2.xml",
                        ProcessingTime = 12
                    },
                    new JobProcessingDetail()
                    {
                        ProviderName = "P6",
                        UkPrn = 12342,
                        Filename = "abc2.xml",
                        ProcessingTime = 12
                    },
                },
            };
            return JsonConvert.SerializeObject(jobsList, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }

    public class JobsList
    {
        public IEnumerable<JobProcessingDetail> Jobs { get; set; }
    }

    public class JobProcessingDetail
    {
        public string ProviderName { get; set; }

        public int UkPrn { get; set; }

        public string Filename { get; set; }

        public int ProcessingTime { get; set; }
    }
}
