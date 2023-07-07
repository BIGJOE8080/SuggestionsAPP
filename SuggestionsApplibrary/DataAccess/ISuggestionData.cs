using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public interface ISuggestionData
    {
        Task CreatSuggestion(SuggestionModel suggestion);
        Task<List<SuggestionModel>> GetAllApprovedSuggestions();
        Task<List<SuggestionModel>> GetAllSuggestions();
        Task<List<SuggestionModel>> GetAllSuggestionsawaitingForApproval();
        Task<SuggestionModel> GetSuggestion(string id);
        Task UpdateSuggestion(SuggestionModel suggestion);
        Task UpvoteSuggestion(string suggestionId, string userId);
    }
}