using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;
using Nop.Services.Caching;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.AnywhereSlider.Services
{
    public class SliderService : ISliderService
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;
        private readonly IRepository<SliderItem> _sliderItemRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<Slider> _sliderRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheKeyService _cacheKeyService;

        #endregion

        #region Constants

        private const string NS_SLIDER_ALL_KEY = "NS.HomePageSlider.all";
        private const string NS_SLIDER_REGION_KEY = "NS.HomePageSlider.region.{0}-{1}-{2}-{3}";
        private const string NS_SLIDER_PATTERN_KEY = "NS.HomePageSlider.";

        #endregion

        #region ctor

        public SliderService(IRepository<Slider> sliderRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<SliderItem> sliderItemRepository,
            IStaticCacheManager cacheManager,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            ICacheKeyService cacheKeyService
            )
        {
            _sliderRepository = sliderRepository;
            _storeMappingRepository = storeMappingRepository;
            _sliderItemRepository = sliderItemRepository;
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
            _cacheKeyService = cacheKeyService;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        protected virtual IDictionary<string, IList<Slider>> GetAllSlidersCached()
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(new CacheKey(NS_SLIDER_ALL_KEY));

            return _cacheManager.Get(key, () =>
            {
                var query = from s in _sliderRepository.Table 
                            where !s.Deleted
                            orderby s.DisplayOrder
                            select s;
                var sliders = query.ToList();
                var dictionary = new Dictionary<string, IList<Slider>>();
                foreach (var slider in sliders)
                {
                    var resourceName = slider.Id.ToString();
                    
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<Slider>
                        {
                            new Slider()
                            {
                                Active = slider.Active,
                                BackgroundPictureId = slider.BackgroundPictureId,
                                DisplayOrder = slider.DisplayOrder,
                                Id = slider.Id,
                                Name = slider.Name,
                                SliderItems = slider.SliderItems,
                                WidgetZoneId = slider.WidgetZoneId,
                                AnimateIn = slider.AnimateIn,
                                AutoPlayTimeout = slider.AutoPlayTimeout,
                                AutoPlay = slider.AutoPlay,
                                AnimateOut = slider.AnimateOut,
                                AutoPlayHoverPause = slider.AutoPlayHoverPause,
                                Deleted = slider.Deleted,
                                LazyLoad = slider.LazyLoad,
                                LazyLoadEager = slider.LazyLoadEager,
                                Loop = slider.Loop,
                                Nav = slider.Nav,
                                StartPosition = slider.StartPosition,
                                Video = slider.Video
                            }
                        });
                    }
                    else
                    {
                        dictionary[resourceName].Add(slider);
                    }
                }

                return dictionary;
            });
        }

        #endregion

        #region Slider

        public virtual IPagedList<Slider> GetAllSliders(List<int> widgetZoneIds = null, int storeId = 0, 
            bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var sliders = _sliderRepository.Table.Where(x => !x.Deleted);
            if (widgetZoneIds != null && widgetZoneIds.Any())
                sliders = sliders.Where(carousel => widgetZoneIds.Contains(carousel.WidgetZoneId));

            if (active.HasValue)
                sliders = sliders.Where(carousel => carousel.Active == active.Value);

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //var sm = _storeMappingRepository.Table
                //    .Where(x => x.EntityName == nameof(Slider) && x.StoreId == storeId)
                //    .ToList().DefaultIfEmpty();

                //sliders = sliders.Where(x => !x.LimitedToStores || sm.Any(y => y.EntityId == x.Id));
                sliders = from o in sliders
                          join sm in _storeMappingRepository.Table
                          on new { c1 = o.Id, c2 = nameof(Slider) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into carousel_sm
                        from sm in carousel_sm.DefaultIfEmpty()
                        where !o.LimitedToStores || storeId == sm.StoreId
                        select o;
            }

            var query = sliders.OrderBy(carousel => carousel.DisplayOrder);
            return new PagedList<Slider>(query, pageIndex, pageSize);
        }

        public virtual Slider GetSliderById(int sliderId)
        {
            if (sliderId == 0)
                return null;

            return _sliderRepository.GetById(sliderId);
        }

        public virtual void InsertSlider(Slider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderRepository.Insert(slider);

            _eventPublisher.EntityInserted(slider);

            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(NS_SLIDER_ALL_KEY);
        }

        public virtual void UpdateSlider(Slider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderRepository.Update(slider);

            _eventPublisher.EntityUpdated(slider);

            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(NS_SLIDER_ALL_KEY);
        }

        public virtual void DeleteSlider(Slider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderRepository.Delete(slider);

            _eventPublisher.EntityDeleted(slider);

            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(NS_SLIDER_ALL_KEY);
        }

        #endregion

        #region Slider items

        public virtual IPagedList<SliderItem> GetSliderItemsBySliderId(int sliderId, int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = _sliderItemRepository.Table;

            query = query.Where(sliderItem => sliderItem.SliderId == sliderId)
                .OrderBy(sliderItem => sliderItem.DisplayOrder);

            return new PagedList<SliderItem>(query, pageIndex, pageSize);
        }

        public virtual SliderItem GetSliderItemById(int sliderItemId)
        {
            if (sliderItemId == 0)
                return null;

            return _sliderItemRepository.GetById(sliderItemId);
        }

        public void InsertSliderItem(SliderItem sliderItem)
        {
            if (sliderItem == null)
                throw new ArgumentNullException(nameof(sliderItem));

            _sliderItemRepository.Insert(sliderItem);

            _eventPublisher.EntityInserted(sliderItem);
            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
        }

        public virtual void UpdateSliderItem(SliderItem sliderItem)
        {
            if (sliderItem == null)
                throw new ArgumentNullException(nameof(sliderItem));

            _sliderItemRepository.Update(sliderItem);

            _eventPublisher.EntityUpdated(sliderItem);
            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
        }

        public virtual void DeleteSliderItem(SliderItem sliderItem)
        {
            if (sliderItem == null)
                throw new ArgumentNullException(nameof(sliderItem));

            _sliderItemRepository.Delete(sliderItem);

            _eventPublisher.EntityUpdated(sliderItem);
            _cacheManager.RemoveByPrefix(NS_SLIDER_PATTERN_KEY);
        }

        #endregion
    }
}
