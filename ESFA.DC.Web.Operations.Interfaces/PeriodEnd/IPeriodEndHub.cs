using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndHub
    {
        Task SendMessage(string paths);
    }
}