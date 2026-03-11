using MGSC;
using System;
using System.Collections.Generic;

namespace QM_ImporterAPI.Templates
{
    [Serializable]
    public class LocalizationTemplate
    {
        public Dictionary<string, Dictionary<Localization.Lang, string>> Keys { get; set; } = new Dictionary<string, Dictionary<Localization.Lang, string>>();

        public static LocalizationTemplate GetExample(string itemId)
        {
            var localizationTemplate = new LocalizationTemplate();

            localizationTemplate.Keys.Add($"item.{itemId}.name", new Dictionary<Localization.Lang, string>()
            {
                { Localization.Lang.EnglishUS, "Example Name" },
                { Localization.Lang.Russian,"" },
                { Localization.Lang.German,"" },
                { Localization.Lang.French,"" },
                { Localization.Lang.Spanish,"" },
                { Localization.Lang.Polish,"" },
                { Localization.Lang.Turkish,"" },
                { Localization.Lang.BrazilianPortugal,"" },
                { Localization.Lang.Korean,"" },
                { Localization.Lang.Japanese,"" },
                { Localization.Lang.ChineseSimp,"" },
            });

            localizationTemplate.Keys.Add($"item.{itemId}.desc", new Dictionary<Localization.Lang, string>()
            {
                { Localization.Lang.EnglishUS,"Example Description" },
                { Localization.Lang.Russian,"" },
                { Localization.Lang.German,"" },
                { Localization.Lang.French,"" },
                { Localization.Lang.Spanish,"" },
                { Localization.Lang.Polish,"" },
                { Localization.Lang.Turkish,"" },
                { Localization.Lang.BrazilianPortugal,"" },
                { Localization.Lang.Korean,"" },
                { Localization.Lang.Japanese,"" },
                { Localization.Lang.ChineseSimp,"" },
            });

            localizationTemplate.Keys.Add($"item.{itemId}.shortdesc", new Dictionary<Localization.Lang, string>()
            {
                { Localization.Lang.EnglishUS,"Example Description" },
                { Localization.Lang.Russian,"" },
                { Localization.Lang.German,"" },
                { Localization.Lang.French,"" },
                { Localization.Lang.Spanish,"" },
                { Localization.Lang.Polish,"" },
                { Localization.Lang.Turkish,"" },
                { Localization.Lang.BrazilianPortugal,"" },
                { Localization.Lang.Korean,"" },
                { Localization.Lang.Japanese,"" },
                { Localization.Lang.ChineseSimp,"" },
            });

            return localizationTemplate;
        }
    }
}