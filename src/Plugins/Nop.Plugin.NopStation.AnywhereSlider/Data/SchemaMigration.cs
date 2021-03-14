using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;

namespace Nop.Plugin.NopStation.AnywhereSlider.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/06/06 09:40:55:1687541", "NopStation.AnywhereSlider base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;
        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<Slider>(Create);
            _migrationManager.BuildTable<SliderItem>(Create);
        }
    }
}

