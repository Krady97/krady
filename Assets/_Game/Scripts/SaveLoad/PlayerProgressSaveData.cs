using System;
using System.Collections.Generic;

namespace Soulwake.Game.SaveLoad
{
    [Serializable]
    public class PlayerProgressSaveData
    {
        public int maxHp = 20;
        public int attackDamage = 5;
        public int currentHp = 20;
        public int gold = 0;
        public List<string> learnedSkillIds = new List<string>();
        public List<string> inventoryItemIds = new List<string>();

        public PlayerProgressSaveData()
        {
        }

        public PlayerProgressSaveData(
            int maxHpValue,
            int attackDamageValue,
            int currentHpValue,
            int goldValue,
            IEnumerable<string> inventoryIds,
            IEnumerable<string> skillIds)
        {
            maxHp = maxHpValue;
            attackDamage = attackDamageValue;
            currentHp = currentHpValue;
            gold = goldValue;
            inventoryItemIds = inventoryIds != null ? new List<string>(inventoryIds) : new List<string>();
            learnedSkillIds = skillIds != null ? new List<string>(skillIds) : new List<string>();
        }
    }
}
