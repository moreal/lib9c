namespace Lib9c.Tests.Action
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Libplanet;
    using Libplanet.Action;
    using Libplanet.Assets;
    using Libplanet.Crypto;
    using Nekoyume;
    using Nekoyume.Action;
    using Nekoyume.Model;
    using Nekoyume.Model.BattleStatus;
    using Nekoyume.Model.State;
    using Nekoyume.TableData;
    using Xunit;

    public class RankingBattleTest
    {
        private readonly IAccountStateDelta _initialState;

        private readonly Dictionary<string, string> _sheets;
        private readonly TableSheets _tableSheets;

        private readonly Address _agent1Address;
        private readonly Address _avatar1Address;

        private readonly Address _agent2Address;
        private readonly Address _avatar2Address;

        private readonly Address _rankingMapAddress;
        private readonly Address _weeklyArenaAddress;

        public RankingBattleTest()
        {
            _initialState = new State();

            _sheets = TableSheetsImporter.ImportSheets();
            foreach (var (key, value) in _sheets)
            {
                _initialState = _initialState.SetState(
                    Addresses.TableSheet.Derive(key),
                    value.Serialize());
            }

            _tableSheets = new TableSheets(_sheets);

            _rankingMapAddress = new PrivateKey().ToAddress();

            var (agent1State, avatar1State) = GetAgentStateWithAvatarState(
                _sheets,
                _tableSheets,
                _rankingMapAddress);
            _agent1Address = agent1State.address;
            _avatar1Address = avatar1State.address;

            var (agent2State, avatar2State) = GetAgentStateWithAvatarState(
                _sheets,
                _tableSheets,
                _rankingMapAddress);
            _agent2Address = agent2State.address;
            _avatar2Address = avatar2State.address;

            var weeklyArenaState = new WeeklyArenaState(0);
            weeklyArenaState.Set(avatar1State, _tableSheets.CharacterSheet);
            weeklyArenaState[_avatar1Address].Activate();
            weeklyArenaState.Set(avatar2State, _tableSheets.CharacterSheet);
            weeklyArenaState[_avatar2Address].Activate();
            _weeklyArenaAddress = weeklyArenaState.address;

            _initialState = _initialState
                .SetState(_agent1Address, agent1State.Serialize())
                .SetState(_avatar1Address, avatar1State.Serialize())
                .SetState(_agent2Address, agent2State.Serialize())
                .SetState(_avatar2Address, avatar2State.Serialize())
                .SetState(_weeklyArenaAddress, weeklyArenaState.Serialize());
        }

        [Fact]
        public void Execute()
        {
            var previousWeeklyState = _initialState.GetWeeklyArenaState(0);
            var previousAvatar1State = _initialState.GetAvatarState(_avatar1Address);
            previousAvatar1State.level = 10;

            var previousState = _initialState.SetState(
                _avatar1Address,
                previousAvatar1State.Serialize());

            var itemIds = _tableSheets.WeeklyArenaRewardSheet.Values
                .Select(r => r.Reward.ItemId)
                .ToList();

            Assert.All(itemIds, id => Assert.False(previousAvatar1State.inventory.HasItem(id)));

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Null(action.Result);

            var nextState = action.Execute(new ActionContext()
            {
                PreviousStates = previousState,
                Signer = _agent1Address,
                Random = new ItemEnhancementTest.TestRandom(),
                Rehearsal = false,
            });

            var nextAvatar1State = nextState.GetAvatarState(_avatar1Address);
            var nextWeeklyState = nextState.GetWeeklyArenaState(0);

            Assert.Contains(nextAvatar1State.inventory.Materials, i => itemIds.Contains(i.Id));
            Assert.NotNull(action.Result);
            Assert.Contains(typeof(GetReward), action.Result.Select(e => e.GetType()));
            Assert.Equal(BattleLog.Result.Win, action.Result.result);
            Assert.True(nextWeeklyState[_avatar1Address].Score >
                        previousWeeklyState[_avatar1Address].Score);
        }

        [Fact]
        public void ExecuteThrowInvalidAddressException()
        {
            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar1Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<InvalidAddressException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = _initialState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ExecuteThrowFailedLoadStateException(int caseIndex)
        {
            Address signer;
            Address avatarAddress;
            Address enemyAddress;

            switch (caseIndex)
            {
                case 0:
                    signer = new PrivateKey().ToAddress();
                    avatarAddress = _avatar1Address;
                    enemyAddress = _avatar2Address;
                    break;
                case 1:
                    signer = _agent1Address;
                    avatarAddress = _avatar1Address;
                    enemyAddress = new PrivateKey().ToAddress();
                    break;
            }

            var action = new RankingBattle
            {
                AvatarAddress = avatarAddress,
                EnemyAddress = enemyAddress,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<FailedLoadStateException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = _initialState,
                    Signer = signer,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Fact]
        public void ExecuteThrowNotEnoughClearedStageLevelException()
        {
            var previousAvatar1State = _initialState.GetAvatarState(_avatar1Address);
            previousAvatar1State.worldInformation = new WorldInformation(
                0,
                _tableSheets.WorldSheet,
                false
            );
            var previousState = _initialState.SetState(
                _avatar1Address,
                previousAvatar1State.Serialize());

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<NotEnoughClearedStageLevelException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = previousState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Fact]
        public void ExecuteThrowWeeklyArenaStateAlreadyEndedException()
        {
            var previousWeeklyArenaState = _initialState.GetWeeklyArenaState(_weeklyArenaAddress);
            previousWeeklyArenaState.Ended = true;

            var previousState = _initialState.SetState(
                _weeklyArenaAddress,
                previousWeeklyArenaState.Serialize());

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<WeeklyArenaStateAlreadyEndedException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = previousState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ExecuteThrowWeeklyArenaStateNotContainsAvatarAddressException(
            int caseIndex)
        {
            Address targetAddress;
            switch (caseIndex)
            {
                case 0:
                    targetAddress = _avatar1Address;
                    break;
                case 1:
                    targetAddress = _avatar2Address;
                    break;
            }

            var previousWeeklyArenaState = _initialState.GetWeeklyArenaState(_weeklyArenaAddress);
            previousWeeklyArenaState.Remove(targetAddress);

            var previousState = _initialState.SetState(
                _weeklyArenaAddress,
                previousWeeklyArenaState.Serialize());

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<WeeklyArenaStateNotContainsAvatarAddressException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = previousState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Fact]
        public void ExecuteThrowNotEnoughWeeklyArenaChallengeCountException()
        {
            var previousAvatarState = _initialState.GetAvatarState(_avatar1Address);
            var previousWeeklyArenaState = _initialState.GetWeeklyArenaState(_weeklyArenaAddress);
            while (true)
            {
                var arenaInfo = previousWeeklyArenaState.GetArenaInfo(_avatar1Address);
                arenaInfo.Update(previousAvatarState, arenaInfo, BattleLog.Result.Lose);
                if (arenaInfo.DailyChallengeCount == 0)
                {
                    break;
                }
            }

            var previousState = _initialState.SetState(
                _weeklyArenaAddress,
                previousWeeklyArenaState.Serialize());

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<NotEnoughWeeklyArenaChallengeCountException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = previousState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Fact]
        public void ExecuteThrowNotEnoughFungibleAssetValueException()
        {
            var previousWeeklyArenaState = _initialState.GetWeeklyArenaState(_weeklyArenaAddress);
            var arenaInfo = previousWeeklyArenaState.GetArenaInfo(_avatar1Address);
            previousWeeklyArenaState.Update(new ArenaInfo(arenaInfo));

            var previousState = _initialState.SetState(
                _weeklyArenaAddress,
                previousWeeklyArenaState.Serialize());

            var goldCurrency = new Currency("NCG", 2, Addresses.GoldCurrency);
            var previousAgentGoldState = _initialState.GetBalance(
                _agent1Address,
                goldCurrency);

            if (previousAgentGoldState.Sign > 0)
            {
                previousState = _initialState.TransferAsset(
                    _agent1Address,
                    Addresses.GoldCurrency,
                    previousAgentGoldState);
            }

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            Assert.Throws<NotEnoughFungibleAssetValueException>(() =>
            {
                action.Execute(new ActionContext()
                {
                    PreviousStates = previousState,
                    Signer = _agent1Address,
                    Random = new ItemEnhancementTest.TestRandom(),
                    Rehearsal = false,
                });
            });
        }

        [Fact]
        public void Determinism()
        {
            var previousWeeklyState = _initialState.GetWeeklyArenaState(0);
            var previousAvatar1State = _initialState.GetAvatarState(_avatar1Address);
            previousAvatar1State.level = 10;

            var previousState = _initialState.SetState(
                _avatar1Address,
                previousAvatar1State.Serialize());

            var itemIds = _tableSheets.WeeklyArenaRewardSheet.Values
                .Select(r => r.Reward.ItemId)
                .ToList();

            var action = new RankingBattle
            {
                AvatarAddress = _avatar1Address,
                EnemyAddress = _avatar2Address,
                WeeklyArenaAddress = _weeklyArenaAddress,
                costumeIds = new List<int>(),
                equipmentIds = new List<Guid>(),
                consumableIds = new List<Guid>(),
            };

            HashDigest<SHA256> stateRootHashA = ActionExecutionUtils.CalculateStateRootHash(action, _initialState, signer: _agent1Address);
            HashDigest<SHA256> stateRootHashB = ActionExecutionUtils.CalculateStateRootHash(action, _initialState, signer: _agent1Address);

            Assert.Equal(stateRootHashA, stateRootHashB);
        }

        private static (AgentState, AvatarState) GetAgentStateWithAvatarState(
            IReadOnlyDictionary<string, string> sheets,
            TableSheets tableSheets,
            Address rankingMapAddress)
        {
            var agentAddress = new PrivateKey().ToAddress();
            var agentState = new AgentState(agentAddress);

            var avatarAddress = agentAddress.Derive("avatar");
            var avatarState = new AvatarState(
                avatarAddress,
                agentAddress,
                0,
                tableSheets.GetAvatarSheets(),
                new GameConfigState(sheets[nameof(GameConfigSheet)]),
                rankingMapAddress)
            {
                worldInformation = new WorldInformation(
                    0,
                    tableSheets.WorldSheet,
                    Math.Max(
                        tableSheets.StageSheet.First?.Id ?? 1,
                        GameConfig.RequireClearedStageLevel.ActionsInRankingBoard)),
            };
            agentState.avatarAddresses.Add(0, avatarAddress);

            return (agentState, avatarState);
        }
    }
}
