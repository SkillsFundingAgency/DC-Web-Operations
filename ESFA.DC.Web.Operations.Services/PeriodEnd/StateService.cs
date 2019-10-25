using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class StateService : IStateService
    {
        private readonly IJsonSerializationService _serializationService;

        public StateService(
            IJsonSerializationService serializationService)
        {
            _serializationService = serializationService;
        }

        public bool PauseReferenceDataIsEnabled(string referenceDataJson)
        {
            var referenceJobs = _serializationService.Deserialize<IEnumerable<JobSchedule>>(referenceDataJson);

            return referenceJobs
                .Any(rj => !rj.Status.Equals(Constants.ReferenceDataJobPausedState, StringComparison.OrdinalIgnoreCase));
        }

        public bool CollectionClosedEmailSent(string pathItemStates)
        {
            var states = _serializationService.Deserialize<IEnumerable<PathPathItemsModel>>(pathItemStates);

            return states.First().CollectionClosedEmailSent;
        }
    }
}