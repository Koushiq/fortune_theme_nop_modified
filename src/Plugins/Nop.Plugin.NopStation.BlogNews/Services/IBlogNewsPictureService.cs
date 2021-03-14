using Nop.Core;
using Nop.Plugin.NopStation.BlogNews.Domains;

namespace Nop.Plugin.NopStation.BlogNews.Services
{
    public interface IBlogNewsPictureService
    {
        void InsertBlogNewsPicture(BlogNewsPicture blogNewsPicture);

        void UpdateBlogNewsPicture(BlogNewsPicture blogNewsPicture);

        void DeleteBlogNewsPicture(BlogNewsPicture blogNewsPicture);

        BlogNewsPicture GetBlogNewsPictureByEntytiId(int entityId, EntityType entityType);

        IPagedList<BlogNewsPicture> GetAllPictures(EntityType entityType, 
            bool? showOnHomePage = null, bool? published = null, int storeId = 0, 
            int languageId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}