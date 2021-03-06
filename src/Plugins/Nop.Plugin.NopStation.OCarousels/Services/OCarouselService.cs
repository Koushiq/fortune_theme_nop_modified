using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Plugin.NopStation.OCarousels.Services
{
    public class OCarouselService : IOCarouselService
    {
        #region Props

        private const string OCAROUSEL_ALL_KEY = "NS.OCarouselList.all";
        private const string OCAROUSEL_REGION_KEY = "NS.OCarouselList.region.{0}-{1}-{2}-{3}-{4}-{5}";
        private const string OCAROUSEL_PATTERN_KEY = "NS.OCarouselList.";

        #endregion

        #region Fields

        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<OCarousel> _carouselRepository;
        private readonly IRepository<OCarouselItem> _carouselItemRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public OCarouselService(IStaticCacheManager cacheManager,
            IStoreMappingService storeMappingService,
            IRepository<OCarousel> carouselRepository,
            IRepository<OCarouselItem> carouselItemRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings)
        {
            _cacheManager = cacheManager;
            _storeMappingService = storeMappingService;
            _carouselRepository = carouselRepository;
            _carouselItemRepository = carouselItemRepository;
            _storeMappingRepository = storeMappingRepository;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        #region OCarousel

        public virtual IPagedList<OCarousel> GetAllCarousels(List<int> widgetZoneIds = null, List<int> dataSources = null,
            int storeId = 0, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue) 
        {
            var query = _carouselRepository.Table.Where(x => !x.Deleted);
            if (widgetZoneIds != null && widgetZoneIds.Any())
                query = query.Where(carousel => widgetZoneIds.Contains(carousel.WidgetZoneId));

            if (dataSources != null && dataSources.Any())
                query = query.Where(carousel => dataSources.Contains(carousel.DataSourceTypeId));

            if (active.HasValue)
                query = query.Where(carousel => carousel.Active == active.Value);

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //var sm = _storeMappingRepository.Table
                //    .Where(x => x.EntityName == nameof(OCarousel) && x.StoreId == storeId)
                //    .ToList();

                //query = query.Where(x => !x.LimitedToStores || sm.Any(y => y.EntityId == x.Id));
                query = from o in query
                        join sm in _storeMappingRepository.Table
                          on new { c1 = o.Id, c2 = nameof(OCarousel) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into carousel_sm
                          from sm in carousel_sm.DefaultIfEmpty()
                          where !o.LimitedToStores || storeId == sm.StoreId
                          select o;
            }

            query = query.OrderBy(carousel => carousel.DisplayOrder);
            return new PagedList<OCarousel>(query, pageIndex, pageSize);
        }

        public virtual OCarousel GetCarouselById(int carouselId)
        {
            if (carouselId == 0)
                return null;

            return _carouselRepository.GetById(carouselId);
        }

        public virtual void InsertCarousel(OCarousel oCarousel)
        {
            if (oCarousel == null)
                throw new ArgumentNullException(nameof(oCarousel));

            _carouselRepository.Insert(oCarousel);

            _eventPublisher.EntityInserted(oCarousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        public virtual void UpdateCarousel(OCarousel oCarousel)
        {
            if (oCarousel == null)
                throw new ArgumentNullException(nameof(oCarousel));

            _carouselRepository.Update(oCarousel);

            _eventPublisher.EntityUpdated(oCarousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        public virtual void DeleteCarousel(OCarousel oCarousel)
        {
            if (oCarousel == null)
                throw new ArgumentNullException(nameof(oCarousel));

            oCarousel.Deleted = true;
            _carouselRepository.Update(oCarousel);

            _eventPublisher.EntityDeleted(oCarousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        #endregion

        #region OCarousel items

        public virtual IPagedList<OCarouselItem> GetOCarouselItemsByOCarouselId(int carouselId, int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = _carouselItemRepository.Table;

            query = query.Where(carouselItem => carouselItem.OCarouselId == carouselId)
                .OrderBy(carouselItem => carouselItem.DisplayOrder);

            return new PagedList<OCarouselItem>(query, pageIndex, pageSize);
        }

        public virtual OCarouselItem GetOCarouselItemById(int carouselItemId)
        {
            if (carouselItemId == 0)
                return null;

            return _carouselItemRepository.GetById(carouselItemId);
        }

        public void InsertOCarouselItem(OCarouselItem carouselItem)
        {
            if (carouselItem == null)
                throw new ArgumentNullException(nameof(carouselItem));

            _carouselItemRepository.Insert(carouselItem);

            _eventPublisher.EntityUpdated(carouselItem.OCarousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        public virtual void UpdateOCarouselItem(OCarouselItem carouselItem)
        {
            if (carouselItem == null)
                throw new ArgumentNullException(nameof(carouselItem));

            _carouselItemRepository.Update(carouselItem);

            _eventPublisher.EntityUpdated(carouselItem.OCarousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        public virtual void DeleteOCarouselItem(OCarouselItem carouselItem)
        {
            if (carouselItem == null)
                throw new ArgumentNullException(nameof(carouselItem));

            var carousel = carouselItem.OCarousel;
            _carouselItemRepository.Delete(carouselItem);

            _eventPublisher.EntityUpdated(carousel);

            _cacheManager.RemoveByPrefix(OCAROUSEL_PATTERN_KEY);
        }

        #endregion

        #endregion
    }
}
