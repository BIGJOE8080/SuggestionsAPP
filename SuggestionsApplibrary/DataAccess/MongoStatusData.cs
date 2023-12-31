﻿

using Microsoft.Extensions.Caching.Memory;
using SuggestionsApplibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public class MongoStatusData : IStatusData
    {
        private readonly IMongoCollection<StatusModel> _statuses;
        private readonly IMemoryCache _cache;
        private const string CacheName = "statusData";



        public MongoStatusData(IDbConnection db, IMemoryCache cache)
        {
            _cache = cache;
            _statuses = db.StatusCollection;
        }

        public async Task<List<StatusModel>> GetAllStatuses()
        {
            var output = _cache.Get<List<StatusModel>>(CacheName);
            if (output is null)
            {
                var results = await _statuses.FindAsync(_ => true);
                output = results.ToList();

                _cache.Set(CacheName, output, TimeSpan.FromDays(value: 1));
            }
            return output;
        }

        public Task CreateStatus(StatusModel status)
        {
            return _statuses.InsertOneAsync(status);
        }  
    }
}
