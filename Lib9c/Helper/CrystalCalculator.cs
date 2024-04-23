using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lib9c;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using Nekoyume.Extensions;
using Nekoyume.Model.Item;
using Nekoyume.Model.State;
using Nekoyume.TableData;
using Nekoyume.TableData.Crystal;

namespace Nekoyume.Helper
{
    public static class CrystalCalculator
    {
#pragma warning disable CS0618
        // Use of obsolete method Currency.Legacy(): https://github.com/planetarium/lib9c/discussions/1319
        public static readonly Currency CRYSTAL = Currencies.Crystal;
#pragma warning restore CS0618
        public const int MaxLevelExponent = 5;
        public const long CrystalLimit = 100_000_000L;

        public static FungibleAssetValue CalculateRecipeUnlockCost(IEnumerable<int> recipeIds, EquipmentItemRecipeSheet equipmentItemRecipeSheet)
        {
            var cost = 0 * CRYSTAL;

            return recipeIds
                .Select(id => equipmentItemRecipeSheet[id])
                .Aggregate(cost, (current, row) => current + row.CRYSTAL * CRYSTAL);
        }

        public static FungibleAssetValue CalculateWorldUnlockCost(IEnumerable<int> worldIds, WorldUnlockSheet worldUnlockSheet)
        {
            var cost = 0 * CRYSTAL;

            return worldIds
                .Select(id => worldUnlockSheet.OrderedList.First(r => r.WorldIdToUnlock == id))
                .Aggregate(cost, (current, row) => current + row.CRYSTAL * CRYSTAL);
        }

        public static FungibleAssetValue CalculateBuffGachaCost(int stageId,
            bool advancedGacha,
            CrystalStageBuffGachaSheet stageBuffGachaSheet)
        {
            return CRYSTAL * (advancedGacha
                ? stageBuffGachaSheet[stageId].AdvancedCost
                : stageBuffGachaSheet[stageId].NormalCost);
        }

        public static FungibleAssetValue CalculateCrystal(
            Address agentAddress,
            IEnumerable<Equipment> equipmentList,
            FungibleAssetValue stakedAmount,
            bool enhancementFailed,
            CrystalEquipmentGrindingSheet crystalEquipmentGrindingSheet,
            CrystalMonsterCollectionMultiplierSheet crystalMonsterCollectionMultiplierSheet,
            StakeRegularRewardSheet stakeRegularRewardSheet
        )
        {
            int monsterCollectionLevel = 0;
            try
            {
                monsterCollectionLevel = stakeRegularRewardSheet.FindLevelByStakedAmount(agentAddress, stakedAmount);
            }
            catch (InsufficientBalanceException)
            {
                // Ignore exception from update table data.
                // https://github.com/planetarium/lib9c/commit/c2f65b9f603ba2e0433df7eaf00224865063f666
            }

            return CalculateCrystal(
                equipmentList,
                enhancementFailed,
                crystalEquipmentGrindingSheet,
                crystalMonsterCollectionMultiplierSheet,
                monsterCollectionLevel
            );
        }

        public static FungibleAssetValue CalculateCrystal(
            IEnumerable<Equipment> equipmentList,
            bool enhancementFailed,
            CrystalEquipmentGrindingSheet crystalEquipmentGrindingSheet,
            CrystalMonsterCollectionMultiplierSheet crystalMonsterCollectionMultiplierSheet,
            int stakingLevel
        )
        {
            var crystal = 0 * CRYSTAL;
            foreach (var equipment in equipmentList)
            {
                BigInteger crystalAmount = 0;
                var grindingRow = crystalEquipmentGrindingSheet[equipment.Id];
                crystalAmount += grindingRow.CRYSTAL;
                crystalAmount +=
                    (BigInteger.Pow(2, Math.Min(equipment.level, MaxLevelExponent)) - 1) *
                    crystalEquipmentGrindingSheet[grindingRow.EnchantBaseId].CRYSTAL;
                crystalAmount = BigInteger.Min(crystalAmount, CrystalLimit);
                crystal += crystalAmount * CRYSTAL;
            }

            // Divide Reward when itemEnhancement failed.
            if (enhancementFailed)
            {
                crystal = crystal.DivRem(2, out _);
            }

            CrystalMonsterCollectionMultiplierSheet.Row multiplierRow =
                crystalMonsterCollectionMultiplierSheet[stakingLevel];
            var extra = crystal.DivRem(100, out _) * multiplierRow.Multiplier;
            return crystal + extra;
        }

        public static FungibleAssetValue CalculateMaterialCost(
            int materialId,
            int materialCount,
            CrystalMaterialCostSheet crystalMaterialCostSheet)
        {
            if (!crystalMaterialCostSheet.TryGetValue(materialId, out var costRow))
            {
                throw new ArgumentException($"This material is not replaceable with crystal. id : {materialId}");
            }

            return costRow.CRYSTAL * materialCount * CRYSTAL;
        }

        public static FungibleAssetValue CalculateCombinationCost(FungibleAssetValue crystal,
            CrystalFluctuationSheet.Row row,
            CrystalCostState prevWeeklyCostState = null,
            CrystalCostState beforePrevWeeklyCostState = null)
        {
            if (prevWeeklyCostState?.CRYSTAL > 0 * CRYSTAL && beforePrevWeeklyCostState?.CRYSTAL > 0 * CRYSTAL)
            {
                int multiplier = (int) (prevWeeklyCostState.CRYSTAL.RawValue * 100 /
                                        beforePrevWeeklyCostState.CRYSTAL.RawValue);
                multiplier = Math.Min(row.MaximumRate, Math.Max(row.MinimumRate, multiplier));
                crystal = crystal.DivRem(100, out _) * multiplier;
            }

            return crystal;
        }

        public static FungibleAssetValue CalculateEntranceFee(int level, BigInteger entranceFee)
        {
            return entranceFee * level * level * CRYSTAL;
        }
    }
}
