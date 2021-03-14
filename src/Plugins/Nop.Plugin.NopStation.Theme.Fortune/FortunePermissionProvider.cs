using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.Theme.Fortune
{
    public class FortunePermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageFortune = new PermissionRecord { Name = "Fortune theme. Manage NopStation Fortune theme", SystemName = "ManageNopStationFortune", Category = "NopStation" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageFortune
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
                        ManageFortune
                    }
                )
            };
        }
    }
}
