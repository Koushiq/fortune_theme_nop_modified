using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/07/08 08:45:55:1687547", "NopStation.ProductTabs base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;
        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<ProductTab>(Create);
            _migrationManager.BuildTable<ProductTabItem>(Create);
            _migrationManager.BuildTable<ProductTabItemProduct>(Create);
        }
    }
}
