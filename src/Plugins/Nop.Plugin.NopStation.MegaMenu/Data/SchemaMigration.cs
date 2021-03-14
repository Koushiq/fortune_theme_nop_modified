using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.MegaMenu.Domains;

namespace Nop.Plugin.NopStation.MegaMenu.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/09/20 03:10:23:1264924", "NopStation.MegaMenu base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<CategoryIcon>(Create);
        }
    }
}
