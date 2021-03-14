using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.BlogNews.Domains;

namespace Nop.Plugin.NopStation.BlogNews.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/07/08 09:55:55:1687542", "NopStation.BlogNews base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;
        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<BlogNewsPicture>(Create);
        }
    }
}
