using System;
using UnityEngine;

namespace Soulwake.Game.Skills
{
    [Serializable]
    public class SkillRewardData
    {
        [SerializeField] private string skillId = "skill_default";
        [SerializeField] private string displayName = "Unnamed Skill";
        [SerializeField] private SkillType skillType = SkillType.Passive;
        [TextArea]
        [SerializeField] private string description = "No description.";

        public string SkillId => skillId;
        public string DisplayName => displayName;
        public SkillType Type => skillType;
        public string Description => description;

        public bool IsValid => !string.IsNullOrWhiteSpace(skillId);
    }
}
