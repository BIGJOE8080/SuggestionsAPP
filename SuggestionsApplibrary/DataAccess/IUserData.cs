using SuggestionsApplibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public interface IUserData
    {
        Task CreatUser(UserModel user);
        Task<UserModel> GetUser(string id);
        Task<List<UserModel>> GetUserAsync();
        Task<UserModel> GetUserFromAuthentication(string objectId);
        Task UpdateUser(UserModel user);
    }
}