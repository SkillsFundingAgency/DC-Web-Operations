using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndStateFactory
    {
        PeriodEndState GetPeriodEndState(PeriodEndStateModel pathModel);
    }
}