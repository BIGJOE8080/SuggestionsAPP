﻿
using Microsoft.Extensions.Caching.Memory;
using SuggestionsApplibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public class MongoCategoryData : ICategoryData
    {
        private readonly IMongoCollection<CategoryModel> _categories;
        private readonly IMemoryCache _cache;
        private const string CacheName = "CategoryData";

        public MongoCategoryData(IDbConnection db, IMemoryCache cache)
        {
            _cache = cache;
            _categories = db.CategoryCollection;
        }

        public async Task<List<CategoryModel>> GetAllCategories()
        {
            var output = _cache.Get<List<CategoryModel>>(CacheName);
            if (output == null)
            {
                var results = await _categories.FindAsync(_ => true);
                output = results.ToList();

                _cache.Set(CacheName, output, TimeSpan.FromDays(value: 1));
            }
            return output;

        }
        public Task CreateCategory(CategoryModel category)
        {
            return _categories.InsertOneAsync(category);
        }
    }


}
