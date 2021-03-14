using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.CategoryBanners
{
    public class CategoryBannerPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageCategoryBanner = new PermissionRecord { Name = "NopStation category banner. Manage category banner", SystemName = "ManageNopStationCategoryBanner", Category = "NopStation" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageCategoryBanner
            };
        }

        HashSet<(string systemRoleName, PermissionRecord[] permissions)> IPermissionProvider.GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        ManageCategoryBanner
                    }
                )
            };
        }


    }
}
