using System;
using System.Collections.Generic;
using MGSC;

namespace QM_ImporterAPI.Templates
{
    [Serializable]
    public class FactionTemplate
    {
        public List<FactionReward> FactionRewardList = new List<FactionReward>();
        public FactionTemplate()
        {

        }

        public static FactionTemplate GetExample(string id)
        {
            return new FactionTemplate()
            {
                FactionRewardList = new List<FactionReward>()
                {
                    new FactionReward()
                    {
                        FactionName = "ChurchRevelation",
                        RewardType = ContentDropTableType.rewardEquipment,
                        contentRecords = new List<ContentDropRecord>()
                        {
                            new ContentDropRecord()
                            {
                                ContentIds = new List<string>() { id },
                                TechLevel = 1,
                                Weight = 75.0f,
                                Points = 350.0f
                            }
                        }
                    }
                }
            };
        }
    }

    [Serializable]
    public class FactionReward
    {
        public string GetTableName() => $"{FactionName}_{RewardType}";
        public string FactionName = string.Empty;
        public ContentDropTableType RewardType = ContentDropTableType.rewardEquipment;
        public List<ContentDropRecord> contentRecords = new List<ContentDropRecord>();
        public FactionReward()
        {

        }
    }

    public enum ContentDropTableType
    {
        rewardEquipment,
        rewardChips,
        rewardConsumables
    }
}