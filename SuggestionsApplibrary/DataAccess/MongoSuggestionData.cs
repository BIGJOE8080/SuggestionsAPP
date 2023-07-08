
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver.Linq;
using SuggestionsApplibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public class MongoSuggestionData
    {
        private readonly IDbConnection _db;
        private readonly IUserData _userData;
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<SuggestionModel> _suggestions;
       // private object client;
        private const string CacheName = "SuggestionData";

        public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
        {
            _db = db;
            _userData = userData;
            _cache = cache;
            _suggestions = db.SuggestioinCollection;
        }
        public async Task<List<SuggestionModel>> GetAllSuggestions()
        {
            var output = _cache .Get<List<SuggestionModel>>(CacheName);
            if (output is null)
            {
                var results = await _suggestions.FindAsync(s => s.Archived == false);
                output= results.ToList();

                _cache.Set(CacheName, output, TimeSpan.FromMinutes(value: 1));
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
            x.ApprovedForRelease== false 
            && x.Rejected == false).ToList();
        }
        public async Task UpdateSuggestion(SuggestionModel suggestion)
        {
            await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
            _cache.Remove(CacheName);
        }

        public async Task UpvoteSuggestion(string suggestionId, string userId)
        {
            var client = _db.Client;
            using var session = await client.StartSessionAsync();

            session.StartTransaction();


            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                var suggestion = (await suggestionsInTransaction.FindAsync(s => s.Id  == suggestionId)).First();

                bool isUpvote = suggestion.UserVotes.Add(userId);
                if (isUpvote == false) 
                {
                    suggestion.UserVotes.Remove(userId);
                }
                await suggestionsInTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUser(suggestion.Author.Id);

                if (isUpvote)
                {
                    user.VoteOnSuggetions.Add(item:new BasicSuggestionModel(suggestion));
                }
                else
                
                {
                    var suggestionToRemove =  user.VoteOnSuggetions.Where(suggestion => suggestion.Id == suggestionId).FirstOrDefault();
                    user.VoteOnSuggetions.Remove(suggestionToRemove);
                }
                await usersInTransaction.ReplaceOneAsync(user => user.Id == userId, user);

                await session.CommitTransactionAsync();

                _cache.Remove(CacheName);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();

                throw;
            }
        }
        public async Task CreatSuggestion(SuggestionModel suggestion)
        {
            var client = _db.Client; 

            using var session = await client.StartSessionAsync();

            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                await suggestionsInTransaction.InsertOneAsync(suggestion);

                var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUser(suggestion.Author.Id);
                user.AuthoredSuggestions.Add(item: new BasicSuggestionModel(suggestion));
                await usersInTransaction.ReplaceOneAsync(user => user.Id == user.Id, user);

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
