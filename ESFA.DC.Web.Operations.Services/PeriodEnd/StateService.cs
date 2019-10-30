using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class StateService : IStateService
    {
        private readonly IJsonSerializationService _serializationService;
        private readonly IPeriodEndStateFactory _periodEndStateFactory;

        public StateService(
            IJsonSerializationService serializationService,
            IPeriodEndStateFactory periodEndStateFactory)
        {
            _serializationService = serializationService;
            _periodEndStateFactory = periodEndStateFactory;
        }

        public bool PauseReferenceDataIsEnabled(string referenceDataJson)
        {
            var referenceJobs = _serializationService.Deserialize<IEnumerable<JobSchedule>>(referenceDataJson);

            return referenceJobs
                .Any(rj => !rj.Status.Equals(Constants.ReferenceDataJobPausedState, StringComparison.OrdinalIgnoreCase));
        }

        public bool CollectionClosedEmailSent(string pathItemStates)
        {
            var states = _serializationService.Deserialize<IEnumerable<PathPathItemsModel>>(pathItemStates).ToList();
            return states.FirstOrDefault() != null && states.First().CollectionClosedEmailSent;
        }

        public PeriodEndState GetPeriodEndState(string pathItemStates)
        {
            var state = _serializationService.Deserialize<IEnumerable<PathPathItemsModel>>(pathItemStates).First();
            var periodEndState = _periodEndStateFactory.GetPeriodEndState(state);

            return periodEndState;
        }
    }
}