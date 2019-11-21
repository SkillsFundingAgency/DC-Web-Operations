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

        private readonly HashSet<string> _pausedStatusLookup = new HashSet<string> { Constants.ReferenceDataJobPausedState, Constants.ReferenceDataJobDisabledState };

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
                .Any(rj => !_pausedStatusLookup.Contains(rj.Status, StringComparer.OrdinalIgnoreCase));
        }

        public bool CollectionClosedEmailSent(string pathItemStates)
        {
            var states = _serializationService.Deserialize<IEnumerable<PathPathItemsModel>>(pathItemStates).ToList();
            return states.FirstOrDefault() != null && states.First().CollectionClosedEmailSent;
        }

        public PeriodEndState GetPeriodEndState(string pathItemStates)
        {
            var state = _serializationService
                .Deserialize<IEnumerable<PathPathItemsModel>>(pathItemStates)
                .Single(path => path.PathId == Constants.CriticalPathHubId);
            var periodEndState = _periodEndStateFactory.GetPeriodEndState(state);

            return periodEndState;
        }
    }
}