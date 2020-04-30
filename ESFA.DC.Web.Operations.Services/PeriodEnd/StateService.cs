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

        private readonly HashSet<string> _pausedStatusLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Constants.ReferenceDataJobPausedState, Constants.ReferenceDataJobDisabledState };

        public StateService(
            IJsonSerializationService serializationService)
        {
            _serializationService = serializationService;
        }

        public bool PauseReferenceDataIsEnabled(IEnumerable<JobSchedule> referenceJobs)
        {
            return referenceJobs.Any(rj => !_pausedStatusLookup.Contains(rj.Status));
        }

        public PeriodEndStateModel GetMainState(string pathItemStates)
        {
            return _serializationService.Deserialize<PeriodEndStateModel>(pathItemStates);
        }

        public PeriodEndPrepModel GetPrepState(string state)
        {
            return _serializationService.Deserialize<PeriodEndPrepModel>(state);
        }

        public bool CollectionClosedEmailSent(PeriodEndStateModel periodEndStateModel)
        {
            return periodEndStateModel.CollectionClosedEmailSent;
        }

        public bool AllJobsHaveCompleted(PeriodEndStateModel state)
        {
            return (state?.Paths?.Any() ?? false) &&
                      state.Paths.All(path => (path.PathItems?.Any() ?? false) &&
                            path.PathItems.All(pi => (pi.PathItemJobs?.Any() ?? false) &&
                                pi.PathItemJobs.All(pij => pij.Status == Constants.JobStatus_Completed)));
        }
    }
}