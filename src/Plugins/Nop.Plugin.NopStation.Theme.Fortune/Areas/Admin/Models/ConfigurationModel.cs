using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Theme.Fortune.Areas.Admin.Models
{
    public class ConfigurationModel : BaseNopModel, ISettingsModel, ILocalizedModel<ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }
        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomThemeColor")]
        public string CustomThemeColor { get; set; }
        public bool CustomThemeColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableImageLazyLoad")]
        public bool EnableImageLazyLoad { get; set; }
        public bool EnableImageLazyLoad_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.LazyLoadPictureId")]
        [UIHint("Picture")]
        public int LazyLoadPictureId { get; set; }
        public bool LazyLoadPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter")]
        public bool ShowSupportedCardsPictureAtPageFooter { get; set; }
        public bool ShowSupportedCardsPictureAtPageFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.SupportedCardsPictureId")]
        [UIHint("Picture")]
        public int SupportedCardsPictureId { get; set; }
        public bool SupportedCardsPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowLogoAtPageFooter")]
        public bool ShowLogoAtPageFooter { get; set; }
        public bool ShowLogoAtPageFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterLogoPictureId")]
        [UIHint("Picture")]
        public int FooterLogoPictureId { get; set; }
        public bool FooterLogoPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterEmail")]
        public string FooterEmail { get; set; }
        public bool FooterEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxOne")]
        public bool EnableDescriptionBoxOne { get; set; }

        public bool EnableDescriptionBoxOne_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitle")]
        public string DescriptionBoxOneTitle { get; set; }

        public bool DescriptionBoxOneTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOnePictureId")]
        [UIHint("Picture")]
        public int DescriptionBoxOnePictureId { get; set; }

        public bool DescriptionBoxOnePictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneText")]
        public string DescriptionBoxOneText { get; set; }

        public bool DescriptionBoxOneText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneUrl")]
        public string DescriptionBoxOneUrl { get; set; }

        public bool DescriptionBoxOneUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxTwo")]
        public bool EnableDescriptionBoxTwo { get; set; }

        public bool EnableDescriptionBoxTwo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitle")]
        public string DescriptionBoxTwoTitle { get; set; }

        public bool DescriptionBoxTwoTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoPictureId")]
        [UIHint("Picture")]
        public int DescriptionBoxTwoPictureId { get; set; }

        public bool DescriptionBoxTwoPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoText")]
        public string DescriptionBoxTwoText { get; set; }

        public bool DescriptionBoxTwoText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoUrl")]
        public string DescriptionBoxTwoUrl { get; set; }

        public bool DescriptionBoxTwoUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxThree")]
        public bool EnableDescriptionBoxThree { get; set; }

        public bool EnableDescriptionBoxThree_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitle")]
        public string DescriptionBoxThreeTitle { get; set; }

        public bool DescriptionBoxThreeTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreePictureId")]
        [UIHint("Picture")]
        public int DescriptionBoxThreePictureId { get; set; }

        public bool DescriptionBoxThreePictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeText")]
        public string DescriptionBoxThreeText { get; set; }

        public bool DescriptionBoxThreeText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeUrl")]
        public string DescriptionBoxThreeUrl { get; set; }

        public bool DescriptionBoxThreeUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxFour")]
        public bool EnableDescriptionBoxFour { get; set; }

        public bool EnableDescriptionBoxFour_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitle")]
        public string DescriptionBoxFourTitle { get; set; }

        public bool DescriptionBoxFourTitle_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourPictureId")]
        [UIHint("Picture")]
        public int DescriptionBoxFourPictureId { get; set; }

        public bool DescriptionBoxFourPictureId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourText")]
        public string DescriptionBoxFourText { get; set; }

        public bool DescriptionBoxFourText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourUrl")]
        public string DescriptionBoxFourUrl { get; set; }

        public bool DescriptionBoxFourUrl_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomCss")]
        public string CustomCss { get; set; }
        public bool CustomCss_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }

    public class ConfigurationLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitle")]
        public string DescriptionBoxOneTitle { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneText")]
        public string DescriptionBoxOneText { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitle")]
        public string DescriptionBoxTwoTitle { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoText")]
        public string DescriptionBoxTwoText { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitle")]
        public string DescriptionBoxThreeTitle { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeText")]
        public string DescriptionBoxThreeText { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitle")]
        public string DescriptionBoxFourTitle { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourText")]
        public string DescriptionBoxFourText { get; set; }
    }
}
