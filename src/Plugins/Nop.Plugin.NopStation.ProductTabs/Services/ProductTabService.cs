using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Data.Migrations.Indexes;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.ProductTabs.Services
{
    public class ProductTabService : IProductTabService
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;
        private readonly IRepository<ProductTab> _productTabRepository;
        private readonly IRepository<ProductTabItem> _productTabItemRepository;
        private readonly IRepository<ProductTabItemProduct> _productTabItemProductRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public ProductTabService(
            IStaticCacheManager cacheManager,
            IRepository<ProductTab> productTabRepository,
            IRepository<ProductTabItem> productTabItemRepository,
            IRepository<ProductTabItemProduct> productTabItemProductRepository,
            IRepository<StoreMapping> storeMappingRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _productTabRepository = productTabRepository;
            _productTabItemRepository = productTabItemRepository;
            _productTabItemProductRepository = productTabItemProductRepository;
            _storeMappingRepository = storeMappingRepository;
            _catalogSettings = catalogSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion
        
        #region Methods

        #region Product tabs

        public void DeleteProductTab(ProductTab productTab)
        {
            if (productTab == null)
                throw new ArgumentNullException(nameof(productTab));

            _productTabRepository.Delete(productTab);

            _eventPublisher.EntityDeleted(productTab);
        }

        public void InsertProductTab(ProductTab productTab)
        {
            if (productTab == null)
                throw new ArgumentNullException(nameof(productTab));

            _productTabRepository.Insert(productTab);

            _eventPublisher.EntityInserted(productTab);
        }

        public void UpdateProductTab(ProductTab productTab) 
        {
            if (productTab == null)
                throw new ArgumentNullException(nameof(productTab));

            _productTabRepository.Update(productTab);

            _eventPublisher.EntityUpdated(productTab);
        }

        public ProductTab GetProductTabById(int productTabId)
        {
            if (productTabId == 0)
                return null;

            return _productTabRepository.GetById(productTabId);
        }

        public IPagedList<ProductTab> GetAllProductTabs(List<int> widgetZoneIds = null, bool hasItemsOnly = false,
            int storeId = 0, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _productTabRepository.Table;

            if (widgetZoneIds != null && widgetZoneIds.Any())
                query = query.Where(productTab => widgetZoneIds.Contains(productTab.WidgetZoneId));

            //if (hasItemsOnly)
            //    query = query.Where(productTab => GetProductTabItemsByProductTabId(productTab.Id).Any());

            if (active.HasValue)
                query = query.Where(productTab => productTab.Active == active.Value);

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //var sm = _storeMappingRepository.Table
                //    .Where(x => x.EntityName == nameof(ProductTab) && x.StoreId == storeId)
                //    .ToList();

                //query = query.Where(x => !x.LimitedToStores || sm.Any(y => y.EntityId == x.Id));
                query = from o in query
                        join sm in _storeMappingRepository.Table
                          on new { c1 = o.Id, c2 = nameof(ProductTab) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into carousel_sm
                        from sm in carousel_sm.DefaultIfEmpty()
                        where !o.LimitedToStores || storeId == sm.StoreId
                        select o;
            }

            query = query.OrderBy(e => e.DisplayOrder);

            return new PagedList<ProductTab>(query, pageIndex, pageSize);
        }

        #endregion

        #region Product tab items

        public void DeleteProductTabItem(ProductTabItem productTabItem)
        {
            if (productTabItem == null)
                throw new ArgumentNullException(nameof(productTabItem));

            _productTabItemRepository.Delete(productTabItem);

            _eventPublisher.EntityDeleted(productTabItem);
        }

        public void UpdateProductTabItem(ProductTabItem productTabItem)
        {
            if (productTabItem == null)
                throw new ArgumentNullException(nameof(productTabItem));

            _productTabItemRepository.Update(productTabItem);

            _eventPublisher.EntityUpdated(productTabItem);
        }

        #endregion


        #region Product tab item products

        public ProductTabItemProduct GetProductTabItemProductById(int productTabItemProductId)
        {
            if (productTabItemProductId == 0)
                return null;

            return _productTabItemProductRepository.GetById(productTabItemProductId);
        }

        public void DeleteProductTabItemProduct(ProductTabItemProduct productTabItemProduct)
        {
            if (productTabItemProduct == null)
                throw new ArgumentNullException(nameof(productTabItemProduct));

            _productTabItemProductRepository.Delete(productTabItemProduct);

            _eventPublisher.EntityDeleted(productTabItemProduct);
        }

        public void UpdateProductTabItemProduct(ProductTabItemProduct productTabItemProduct)
        {
            if (productTabItemProduct == null)
                throw new ArgumentNullException(nameof(productTabItemProduct));

            _productTabItemProductRepository.Update(productTabItemProduct);

            _eventPublisher.EntityUpdated(productTabItemProduct);
        }

        public ProductTabItem GetProductTabItemById(int productTabItemId)
        {
            if (productTabItemId == 0)
                return null;

            return _productTabItemRepository.GetById(productTabItemId);
        }
        
        public List<ProductTabItem> GetProductTabItemsByProductTabId(int productTabId)
        {
            if (productTabId == 0)
                return null;
            var query = _productTabItemRepository.Table;
            query = query.Where(x => x.ProductTabId == productTabId);
            return query.ToList();
        }

        public List<ProductTabItemProduct> GetProductTabItemProductsByProductTabItemId(int productTabItemId)
        {
            if (productTabItemId == 0)
                return null;
            var query = _productTabItemProductRepository.Table;
            query = query.Where(x => x.ProductTabItemId == productTabItemId);
            return query.ToList();
        }

        public void InsertProductTabItem(ProductTabItem productTabItem)
        {
            if (productTabItem == null)
                return;
            _productTabItemRepository.Insert(productTabItem);
        }

        public void InsertProductTabItemProduct(ProductTabItemProduct productTabItemProduct)
        {
            if (productTabItemProduct == null)
                return;
            _productTabItemProductRepository.Insert(productTabItemProduct);
        }

        #endregion

        #endregion
    }
}
