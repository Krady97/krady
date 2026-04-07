# Soulwake Vertical Slice - World Setup Checklist

This checklist wires the current gameplay scripts into a playable `World_VSlice` scene.

## 1) Required tags and layers

Create these tags:
- `Player`

Create these layers:
- `Enemy`
- `SpecialSoul`
- (optional) `Pickup`

## 2) Player GameObject

Create `Player` and assign tag `Player`.

Add components:
- `Rigidbody2D`
  - Body Type: Dynamic
  - Gravity Scale: 0
  - Interpolate: Interpolate
  - Freeze Rotation Z: enabled
- `Collider2D` (Capsule or Box)
- `PlayerInputReader`
- `PlayerMovementController`
- `PlayerStats`
- `Damageable2D` (`destroyOnDeath = false`)
- `PlayerMeleeAttack2D`
- `PlayerSoulInteractor`
- `PlayerSkillBook`
- `PlayerGoldWallet`
- `PlayerInventory`
- `PlayerProgressSaveController`

Important inspector setup:
- `PlayerMeleeAttack2D.targetLayers` includes `Enemy`
- `PlayerSoulInteractor.specialSoulLayer` includes `SpecialSoul`
- `PlayerProgressSaveController`
  - save key: `F5`
  - load key: `F9`
  - optional `autoLoadOnStart = true`

## 3) Normal enemy prefab

Create prefab `Enemy_Normal`.

Add components:
- `Rigidbody2D` (Gravity Scale 0)
- `Collider2D`
- `PlayerStats`
- `Damageable2D` (`destroyOnDeath = true`)
- `EnemyMovement2D`
- `EnemyMeleeAttack2D`
- `EnemyController2D`
- `EnemyDeathNotifier`
- `EnemyTagAuthoring` (optional helper)

Setup:
- Layer = `Enemy`
- `EnemyMeleeAttack2D.targetLayers` includes player layer/tag target collider
- `EnemyDeathNotifier`
  - `enemyId = enemy_normal_01`
  - `isUniqueEnemy = false`

## 4) Unique enemy prefab

Duplicate normal enemy to `Enemy_Unique`.

Differences:
- higher HP / attack values
- `EnemyDeathNotifier`
  - `enemyId = enemy_unique_01`
  - `isUniqueEnemy = true`

## 5) Wraith enemy prefab

Duplicate unique enemy to `Enemy_Wraith`.

Recommended:
- increase HP and attack further for trial feel
- keep enemy combat scripts
- add `WraithIdentity` (optional; manager can add runtime)

## 6) Soul prefabs

### Normal soul prefab (`Soul_Normal`)
- `SpriteRenderer`
- `Collider2D` (trigger)
- `NormalSoulPickup`
  - set stat gain values

### Special soul prefab (`Soul_Special`)
- Layer = `SpecialSoul`
- `SpriteRenderer` (distinct color)
- `Collider2D` (trigger)
- `SpecialSoulInteractable`
  - `sourceEnemyId = enemy_unique_01`
  - prompt text as desired

## 7) Loot pickup prefabs

### Gold pickup (`Pickup_Gold`)
- `SpriteRenderer`
- `Collider2D` (trigger)
- `GoldPickup2D`

### Item pickup (`Pickup_Item`)
- `SpriteRenderer`
- `Collider2D` (trigger)
- `ItemPickup2D`

## 8) Service objects in scene

Create empty objects:

### `SoulDropService`
- add `SoulDropService`
- assign:
  - normal soul prefab = `Soul_Normal`
  - special soul prefab = `Soul_Special`

### `LootDropService`
- add `LootDropService`
- assign:
  - gold pickup prefab = `Pickup_Gold`
  - item pickup prefab = `Pickup_Item`
- configure drop entries:
  - normal enemy entry for `enemy_normal_01`
  - unique enemy entry for `enemy_unique_01`

### `SoulRealmManager`
- add `SoulRealmManager`
- assign:
  - player transform (or keep tag auto-find)
  - player skill book (or auto-find)
  - realm player spawn point transform
  - realm wraith spawn point transform
  - wraith prefab = `Enemy_Wraith`
- add reward mapping:
  - `sourceEnemyId = enemy_unique_01`
  - set one `SkillRewardData`

### `VerticalSliceValidator`
- add `VerticalSliceSceneValidator`
- assign references (player/services/UI)

## 9) UI setup

Create Canvas with TMP texts:
- message text (for feed)
- HP text
- ATK text
- Gold text

Add components:
- `SimpleGameplayTextFeed` (assign message text)
- `PlayerHudTextPanel` (assign player references + HP/ATK/Gold texts)

## 10) Smoke test flow

1. Run scene.
2. Kill normal enemy -> normal soul + loot spawns.
3. Touch normal soul -> stat increases.
4. Kill unique enemy -> special soul spawns.
5. Press `F` near special soul -> enters realm.
6. Kill wraith -> skill learned + return to world.
7. Collect loot pickups -> gold/inventory updates.
8. Press `F5` to save, `F9` to load and verify persistence.
