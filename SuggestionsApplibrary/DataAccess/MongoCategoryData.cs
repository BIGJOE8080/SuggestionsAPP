
using Microsoft.Extensions.Caching.Memory;
using SuggestionsApplibrary.Models;
using System.Threading.Tasks;

namespace SuggestionsApplibrary.DataAccess
{
    public class MongoCategoryData
    {
        private readonly IMongoCollection<CategoryModel> _categories;
        private readonly IMemoryCache _cache;
        private const string cacheName = "CategoryData";

        public MongoCategoryData(IMemoryCache cache)
        {
            _cache = cache;
            _categories = db.CategoryCollection;
        }

        public async Task<List> CategoryModel>>GetAllCategories()
        {

        }
    }

  
}
