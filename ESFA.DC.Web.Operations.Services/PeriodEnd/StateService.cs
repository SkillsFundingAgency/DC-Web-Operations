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

        private readonly HashSet<string> _pausedStatusLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Constants.ReferenceDataJobPausedState, Constants.ReferenceDataJobDisabledState };

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
            return referenceJobs.Any(rj => !_pausedStatusLookup.Contains(rj.Status));
        }

        public PeriodEndStateModel GetState(string pathItemStates)
        {
            return _serializationService.Deserialize<PeriodEndStateModel>(pathItemStates);
        }

        public bool CollectionClosedEmailSent(PeriodEndStateModel periodEndStateModel)
        {
            return periodEndStateModel.CollectionClosedEmailSent;
        }

        public PeriodEndState GetPeriodEndState(PeriodEndStateModel periodEndStateModel)
        {
            var periodEndState = _periodEndStateFactory.GetPeriodEndState(periodEndStateModel);
            return periodEndState;
        }
    }
}