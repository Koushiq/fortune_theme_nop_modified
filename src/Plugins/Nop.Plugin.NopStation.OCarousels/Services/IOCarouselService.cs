using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.OCarousels.Domains;

namespace Nop.Plugin.NopStation.OCarousels.Services
{
    public interface IOCarouselService
    {
        IPagedList<OCarousel> GetAllCarousels(List<int> widgetZoneIds = null, List<int> dataSources = null,
            int storeId = 0, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);

        OCarousel GetCarouselById(int carouselId);

        void InsertCarousel(OCarousel oCarousel);

        void UpdateCarousel(OCarousel oCarousel);

        void DeleteCarousel(OCarousel oCarousel);

        IPagedList<OCarouselItem> GetOCarouselItemsByOCarouselId(int carouselId, int pageIndex = 0,
            int pageSize = int.MaxValue);

        OCarouselItem GetOCarouselItemById(int carouselItemId);

        void InsertOCarouselItem(OCarouselItem carouselItem);

        void UpdateOCarouselItem(OCarouselItem carouselItem);

        void DeleteOCarouselItem(OCarouselItem carouselItem);
    }
}