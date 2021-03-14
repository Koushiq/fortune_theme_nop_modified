using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.CategoryBanners.Domains;

namespace Nop.Plugin.NopStation.CategoryBanners.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/07/07 09:19:55:1687541", "NopStation.CategoryBanner base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;
        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<CategoryBanner>(Create);
        }
    }
}

