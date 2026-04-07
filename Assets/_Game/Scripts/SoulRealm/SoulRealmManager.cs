using System;
using System.Collections.Generic;
using UnityEngine;
using Soulwake.Game.Combat;
using Soulwake.Game.Core;
using Soulwake.Game.Enemy;
using Soulwake.Game.Skills;
using Soulwake.Game.Souls;

namespace Soulwake.Game.SoulRealm
{
    /// <summary>
    /// Handles entering a soul realm, spawning a wraith trial,
    /// awarding a skill on victory, then returning the player.
    /// Uses in-scene teleport points for a simple vertical slice flow.
    /// </summary>
    public class SoulRealmManager : MonoBehaviour
    {
        [Serializable]
        private class RewardByEnemyId
        {
            [SerializeField] private string sourceEnemyId = "unique_enemy_01";
            [SerializeField] private SkillRewardData skillReward = new SkillRewardData();

            public string SourceEnemyId => sourceEnemyId;
            public SkillRewardData SkillReward => skillReward;
        }

        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerSkillBook playerSkillBook;
        [SerializeField] private string playerTag = "Player";

        [Header("Realm Points")]
        [SerializeField] private Transform soulRealmPlayerSpawnPoint;
        [SerializeField] private Transform soulRealmWraithSpawnPoint;

        [Header("Wraith")]
        [SerializeField] private GameObject wraithPrefab;
        [SerializeField] private bool disableEnemyDeathNotifierOnWraith = true;

        [Header("Skill Rewards")]
        [SerializeField] private SkillRewardData fallbackReward = new SkillRewardData();
        [SerializeField] private List<RewardByEnemyId> rewardByEnemyId = new List<RewardByEnemyId>();

        private readonly HashSet<string> completedTrials = new HashSet<string>();
        private GameObject activeWraithInstance;
        private Damageable2D activeWraithDamageable;
        private Vector2 savedWorldPlayerPosition;
        private string activeEnemyId;
        private bool inSoulRealm;

        private void OnEnable()
        {
            SoulWorldEnterRequest.Requested += HandleEnterRequested;
        }

        private void OnDisable()
        {
            SoulWorldEnterRequest.Requested -= HandleEnterRequested;
            UnsubscribeFromActiveWraithDeath();
        }

        private void Start()
        {
            ResolvePlayerReferences();
        }

        private void HandleEnterRequested(string sourceEnemyId, Vector2 _)
        {
            ResolvePlayerReferences();
            if (playerTransform == null || playerSkillBook == null)
            {
                GameplayTextEvents.Raise("Soul realm cannot open: missing player references.");
                return;
            }

            if (inSoulRealm)
            {
                GameplayTextEvents.Raise("Already in a soul realm trial.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sourceEnemyId))
            {
                GameplayTextEvents.Raise("This soul has no trial identity.");
                return;
            }

            if (completedTrials.Contains(sourceEnemyId))
            {
                GameplayTextEvents.Raise("That soul's trial is already complete.");
                return;
            }

            if (soulRealmPlayerSpawnPoint == null || soulRealmWraithSpawnPoint == null || wraithPrefab == null)
            {
                GameplayTextEvents.Raise("Soul realm setup incomplete in inspector.");
                return;
            }

            savedWorldPlayerPosition = playerTransform.position;
            activeEnemyId = sourceEnemyId;
            inSoulRealm = true;

            TeleportPlayerToRealm();
            SpawnWraithForActiveEnemy();
            GameplayTextEvents.Raise($"Soul realm entered: defeat the {activeEnemyId} wraith.");
        }

        private void ResolvePlayerReferences()
        {
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
            }

            if (playerSkillBook == null && playerTransform != null)
            {
                playerSkillBook = playerTransform.GetComponent<PlayerSkillBook>();
            }
        }

        private void TeleportPlayerToRealm()
        {
            playerTransform.position = soulRealmPlayerSpawnPoint.position;
        }

        private void TeleportPlayerBackToWorld()
        {
            playerTransform.position = savedWorldPlayerPosition;
        }

        private void SpawnWraithForActiveEnemy()
        {
            activeWraithInstance = Instantiate(
                wraithPrefab,
                soulRealmWraithSpawnPoint.position,
                Quaternion.identity);

            WraithIdentity identity = activeWraithInstance.GetComponent<WraithIdentity>();
            if (identity == null)
            {
                identity = activeWraithInstance.AddComponent<WraithIdentity>();
            }
            identity.SetSourceEnemyId(activeEnemyId);

            if (disableEnemyDeathNotifierOnWraith)
            {
                EnemyDeathNotifier deathNotifier = activeWraithInstance.GetComponent<EnemyDeathNotifier>();
                if (deathNotifier != null)
                {
                    deathNotifier.enabled = false;
                }
            }

            activeWraithDamageable = activeWraithInstance.GetComponent<Damageable2D>();
            if (activeWraithDamageable == null)
            {
                GameplayTextEvents.Raise("Wraith prefab is missing Damageable2D.");
                if (activeWraithInstance != null)
                {
                    Destroy(activeWraithInstance);
                    activeWraithInstance = null;
                }
                TeleportPlayerBackToWorld();
                activeEnemyId = string.Empty;
                inSoulRealm = false;
                return;
            }

            activeWraithDamageable.OnDied += HandleWraithDied;
        }

        private void HandleWraithDied(Damageable2D _)
        {
            UnsubscribeFromActiveWraithDeath();
            RewardAndExitRealm();
        }

        private void RewardAndExitRealm()
        {
            SkillRewardData reward = ResolveRewardForEnemy(activeEnemyId);
            bool learned = playerSkillBook.TryLearnSkill(reward);
            if (!learned)
            {
                if (reward != null && reward.IsValid)
                {
                    GameplayTextEvents.Raise($"Skill already learned: {reward.DisplayName}");
                }
                else
                {
                    GameplayTextEvents.Raise("Trial complete, but no valid skill reward configured.");
                }
            }

            completedTrials.Add(activeEnemyId);
            TeleportPlayerBackToWorld();

            activeEnemyId = string.Empty;
            inSoulRealm = false;
            activeWraithInstance = null;
            activeWraithDamageable = null;

            GameplayTextEvents.Raise("Soul realm trial complete. Returned to world.");
        }

        private SkillRewardData ResolveRewardForEnemy(string sourceEnemyId)
        {
            for (int i = 0; i < rewardByEnemyId.Count; i++)
            {
                RewardByEnemyId entry = rewardByEnemyId[i];
                if (entry == null)
                {
                    continue;
                }

                if (string.Equals(entry.SourceEnemyId, sourceEnemyId, StringComparison.Ordinal))
                {
                    return entry.SkillReward;
                }
            }

            return fallbackReward;
        }

        private void UnsubscribeFromActiveWraithDeath()
        {
            if (activeWraithDamageable != null)
            {
                activeWraithDamageable.OnDied -= HandleWraithDied;
            }
        }
    }
}
