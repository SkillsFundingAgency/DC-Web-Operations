using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Reports;

namespace ESFA.DC.Web.Operations.Interfaces.Authorisation
{
    public interface IAuthorisationService
    {
        Task<bool> IsAuthorisedForReport(IReport report);
    }
}
