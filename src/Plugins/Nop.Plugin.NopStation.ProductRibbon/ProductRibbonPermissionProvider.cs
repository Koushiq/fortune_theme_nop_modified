using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.ProductRibbon
{
    public class ProductRibbonPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageProductRibbon = new PermissionRecord { Name = "NopStation product ribbon. Manage product ribbon", SystemName = "ManageNopStationProductRibbon", Category = "NopStation" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageProductRibbon
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
                        ManageProductRibbon
                    }
                )
            };
        }

    }
}
