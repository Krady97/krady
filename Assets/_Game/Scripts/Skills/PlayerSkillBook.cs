using System;
using System.Collections.Generic;
using UnityEngine;
using Soulwake.Game.Core;

namespace Soulwake.Game.Skills
{
    /// <summary>
    /// Runtime container for learned skills in the vertical slice.
    /// </summary>
    public class PlayerSkillBook : MonoBehaviour
    {
        [SerializeField] private List<string> learnedSkillIds = new List<string>();

        public event Action<SkillRewardData> OnSkillLearned;

        public IReadOnlyList<string> LearnedSkillIds => learnedSkillIds;

        public bool HasSkill(string skillId)
        {
            return !string.IsNullOrWhiteSpace(skillId) && learnedSkillIds.Contains(skillId);
        }

        public bool TryLearnSkill(SkillRewardData reward)
        {
            if (reward == null || !reward.IsValid)
            {
                return false;
            }

            if (HasSkill(reward.SkillId))
            {
                return false;
            }

            learnedSkillIds.Add(reward.SkillId);
            OnSkillLearned?.Invoke(reward);
            GameplayTextEvents.Raise($"Learned {reward.Type} skill: {reward.DisplayName}");
            return true;
        }
    }
}
