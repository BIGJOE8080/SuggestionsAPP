
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver.Linq;
using SuggestionsApplibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public class MongoSuggestionData : ISuggestionData
    {
        private readonly IDbConnection db;
        private readonly IUserData userData;
        private readonly IMemoryCache cache;
        private readonly IMongoCollection<SuggestionModel> _suggestions;
        private const string CacheName = "SuggestionData";

        public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
        {
            this.db = db;
            this.userData = userData;
            this.cache = cache;
            this._suggestions = db.SuggestioinCollection;
        }
        public async Task<List<SuggestionModel>> GetAllSuggestions()
        {
            var output = this.cache.Get<List<SuggestionModel>>(CacheName);
            if (output is null)
            {
                var results = await _suggestions.FindAsync(s => s.Archived == false);
                output = results.ToList();

                this.cache.Set(CacheName, output, TimeSpan.FromMinutes(value: 1));
            }
            return output;

        }
        public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
        {
            var output = await GetAllSuggestions();
            return output.Where(x => x.ApprovedForRelease).ToList();
        }
        public async Task<SuggestionModel> GetSuggestion(string id)
        {
            var results = await _suggestions.FindAsync(s => s.Id == id);
            return results.FirstOrDefault();
        }
        public async Task<List<SuggestionModel>> GetAllSuggestionsawaitingForApproval()
        {
            var output = await GetAllSuggestions();
            return output.Where(x =>
            x.ApprovedForRelease == false
            && x.Rejected == false).ToList();
        }
        public async Task UpdateSuggestion(SuggestionModel suggestion)
        {
            await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
            this.cache.Remove(CacheName);
        }

        public async Task UpvoteSuggestion(string suggestionId, string userId)
        {
            using var session = await client.StartSessionAsync();

            session.StartTransaction();


            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                var suggestion = (await suggestionInTransaction.FindAsync(suggestion => suggestion.Id == suggestionId)).FirstOrDefault();

                bool isUpvote = suggestion.Uservotes.Ass(userId);
                if (isUpvote == false)
                {
                    suggestion.Uservotes.Remove(userId);
                }
                await suggestionsInTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(_db.UsersCollectionName);
                var user = await _userData.GetUser(suggestion.Author.Id);

                if (isUpvote)
                {
                    user.VotedOnSuggestions.Add(item: new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionToRemove = user.VotedOnSuggestions.Where(suggestion => suggestion.Id == suggestionId).FirstOrDefault();
                    user.VotedOnSuggestions.Remove(suggestionToRemove);
                }
                await usersInTransaction.ReplaceOneAsync(user => user.Id == userId, user);

                await session.CommitTransactionAsync();

                this.cache.Remove(CacheName);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();

                throw;
            }
        }
        public async Task CreatSuggestion(SuggestionModel suggestion)
        {
            var client = _db.client;

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                await suggestionsInTransaction.InsertOneAsync(suggestion);

                var userInTransaction db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUser(suggestion.Author.Id);
                user.AuthoredSuggestions.Add(item: new BasicSuggestionModel(suggestion));
                await userInTransaction.ReplaceOneAsync(user => user.Id == user.Id, user);

                await session.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();

                throw;
            }
        }
    }
}
