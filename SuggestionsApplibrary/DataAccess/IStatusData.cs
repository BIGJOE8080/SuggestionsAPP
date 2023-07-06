using SuggestionsApplibrary.Models;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public interface IStatusData
    {
        Task CreateStatus(StatusModel status);
        Task<List<StatusModel>> GetAllStatuses();
    }
}