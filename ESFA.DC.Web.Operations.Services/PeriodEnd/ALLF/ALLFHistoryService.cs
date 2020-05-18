using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class ALLFHistoryService : AbstractHistoryService, IALLFHistoryService
    {
        public ALLFHistoryService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, apiSettings, httpClient, $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-allf")
        {
        }

        public Task<IEnumerable<ALLFHistoryDetail>> GetHistoryDetails(int year, CancellationToken cancellationToken)
        {
            var result = new List<ALLFHistoryDetail>
            {
                new ALLFHistoryDetail
                {
                    Date = new DateTime(2020, 1, 1, 1, 1, 1),
                    SubmittedBy = "Lynne Burdon",
                    File = "ALLF-20200217-123456.csv",
                    PeriodEnd = true,
                    Return = "A55",
                    Status = 0,
                    Records = 23189,
                    Warnings = 235,
                    ReportFile = "ALLFRD-202001011533-validation-report-20200101153322.csv"
                },
                new ALLFHistoryDetail
                {
                    Date = new DateTime(2020, 1, 1, 1, 1, 1),
                    SubmittedBy = "Lynne",
                    File = "some file.csv",
                    PeriodEnd = false,
                    Return = "A55",
                    Status = 1,
                    Records = 20,
                    Warnings = 0,
                    ReportFile = "some report file.csv"
                },
            };

            return Task.FromResult(result as IEnumerable<ALLFHistoryDetail>);
        }
    }
}