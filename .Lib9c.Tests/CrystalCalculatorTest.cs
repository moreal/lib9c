namespace Lib9c.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Libplanet.Assets;
    using Nekoyume.Helper;
    using Nekoyume.Model.Item;
    using Nekoyume.Model.State;
    using Nekoyume.TableData;
    using Nekoyume.TableData.Crystal;
    using Xunit;

    public class CrystalCalculatorTest
    {
        private readonly TableSheets _tableSheets;
        private readonly EquipmentItemRecipeSheet _equipmentItemRecipeSheet;
        private readonly WorldUnlockSheet _worldUnlockSheet;
        private readonly CrystalMaterialCostSheet _crystalMaterialCostSheet;
        private readonly Currency _ncgCurrency;

        public CrystalCalculatorTest()
        {
            _tableSheets = new TableSheets(TableSheetsImporter.ImportSheets());
            _equipmentItemRecipeSheet = _tableSheets.EquipmentItemRecipeSheet;
            _worldUnlockSheet = _tableSheets.WorldUnlockSheet;
            _crystalMaterialCostSheet = _tableSheets.CrystalMaterialCostSheet;
            _ncgCurrency = new Currency("NCG", 2, minters: null);
        }

        [Theory]
        [InlineData(new[] { 2 }, 19)]
        [InlineData(new[] { 2, 3 }, 38)]
        public void CalculateRecipeUnlockCost(IEnumerable<int> recipeIds, int expected)
        {
            Assert.Equal(expected * CrystalCalculator.CRYSTAL, CrystalCalculator.CalculateRecipeUnlockCost(recipeIds, _equipmentItemRecipeSheet));
        }

        [Theory]
        [InlineData(new[] { 2 }, 250)]
        [InlineData(new[] { 2, 3 }, 20250)]
        public void CalculateWorldUnlockCost(IEnumerable<int> worldIds, int expected)
        {
            Assert.Equal(expected * CrystalCalculator.CRYSTAL, CrystalCalculator.CalculateWorldUnlockCost(worldIds, _worldUnlockSheet));
        }

        [Theory]
        [ClassData(typeof(CalculateCrystalData))]
        public void CalculateCrystal((int EquipmentId, int Level)[] equipmentInfos, int stakedAmount, bool enhancementFailed, int expected)
        {
            var equipmentList = new List<Equipment>();
            foreach (var (equipmentId, level) in equipmentInfos)
            {
                var row = _tableSheets.EquipmentItemSheet[equipmentId];
                var equipment =
                    ItemFactory.CreateItemUsable(row, default, 0, level);
                equipmentList.Add((Equipment)equipment);
            }

            var actual = CrystalCalculator.CalculateCrystal(
                default,
                equipmentList,
                stakedAmount * _ncgCurrency,
                enhancementFailed,
                _tableSheets.CrystalEquipmentGrindingSheet,
                _tableSheets.CrystalMonsterCollectionMultiplierSheet,
                _tableSheets.StakeRegularRewardSheet
            );

            Assert.Equal(
                expected * CrystalCalculator.CRYSTAL,
                actual);
        }

        [Theory]
        [InlineData(2, 1, 200)]
        [InlineData(9, 10, 90)]
        // Minimum
        [InlineData(1, 2, 50)]
        // Maximum
        [InlineData(3, 1, 300)]
        public void CalculateCombinationCost(int psCount, int bpsCount, int expected)
        {
            var crystal = 100 * CrystalCalculator.CRYSTAL;
            var ps = new CrystalCostState(default, crystal * psCount);
            var bps = new CrystalCostState(default, crystal * bpsCount);
            var row = _tableSheets.CrystalFluctuationSheet.Values.First(r =>
                r.Type == CrystalFluctuationSheet.ServiceType.Combination);
            Assert.Equal(expected * CrystalCalculator.CRYSTAL, CrystalCalculator.CalculateCombinationCost(crystal, row, prevWeeklyCostState: ps, beforePrevWeeklyCostState: bps));
        }

        [Fact]
        public void CalculateRandomBuffCost()
        {
            var stageBuffGachaSheet = _tableSheets.CrystalStageBuffGachaSheet;
            foreach (var row in stageBuffGachaSheet.Values)
            {
                var expectedCost = row.CRYSTAL * CrystalCalculator.CRYSTAL;
                Assert.Equal(
                    expectedCost,
                    CrystalCalculator.CalculateBuffGachaCost(row.StageId, 5, stageBuffGachaSheet));
                Assert.Equal(
                    expectedCost * 3,
                    CrystalCalculator.CalculateBuffGachaCost(row.StageId, 10, stageBuffGachaSheet));
            }
        }

        [Theory]
        [InlineData(302000, 1, 30, null)]
        [InlineData(302003, 2, 60, null)]
        [InlineData(306068, 1, 100, typeof(ArgumentException))]
        public void CalculateMaterialCost(int materialId, int materialCount, int expected, Type exc)
        {
            if (_crystalMaterialCostSheet.ContainsKey(materialId))
            {
                var cost = CrystalCalculator.CalculateMaterialCost(materialId, materialCount, _crystalMaterialCostSheet);
                Assert.Equal(expected * CrystalCalculator.CRYSTAL, cost);
            }
            else
            {
                Assert.Throws(exc, () => CrystalCalculator.CalculateMaterialCost(materialId, materialCount, _crystalMaterialCostSheet));
            }
        }

        private class CalculateCrystalData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                // 1000 + (2^0 - 1) * 100 = 1000
                // enchant level 2
                // 10 + (2^2 - 1) * 10 = 40
                // total 1040
                new object[]
                {
                    new[]
                    {
                        (10100000, 0),
                        (10110000, 2),
                    },
                    10,
                    false,
                    1040,
                },
                new object[]
                {
                    // enchant failed
                    // (1000 + (2^0 -1) * 1000) / 2 = 500
                    // total 500
                    new[]
                    {
                        (10100000, 0),
                    },
                    10,
                    true,
                    500,
                },
                // enchant level 3 & failed
                // (10 + (2^3 - 1) * 10) / 2 = 450
                // multiply by staking
                // 40 * 0.2 = 8
                // total 48
                new object[]
                {
                    new[]
                    {
                        (10110000, 3),
                    },
                    100,
                    true,
                    48,
                },
                // enchant level 1
                // 1000 + (2^1 - 1) * 1000 = 2000
                // enchant level 2
                // 10 + (2^2 - 1) * 10 = 40
                // multiply by staking
                // 2040 * 0.2 = 408
                // total 2448
                new object[]
                {
                    new[]
                    {
                        (10100000, 1),
                        (10110000, 2),
                    },
                    100,
                    false,
                    2448,
                },
                // enchant level 1
                // 10 + (2^1 - 1) * 10 = 20
                new object[]
                {
                    new[]
                    {
                        (10110000, 1),
                    },
                    0,
                    false,
                    20,
                },
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        }
    }
}