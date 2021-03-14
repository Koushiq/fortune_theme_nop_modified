using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.AnywhereSlider
{
    public class AnywhereSliderPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageSliders = new PermissionRecord { Name = "NopStation anywhere slider. Manage slider", SystemName = "ManageNopStationSliders", Category = "NopStation" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageSliders
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
                       ManageSliders
                    }
                )
            };
        }
    }
}
