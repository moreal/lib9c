using System;
using System.Collections.Generic;
using Bencodex.Types;

namespace Nekoyume.Model.State
{
    [Serializable]
    public class RaiderState : IState
    {
        public int TotalScore;
        public int HighScore;
        public int TotalChallengeCount;
        public int RemainChallengeCount;
        public int LatestRewardRank;
        public long ClaimedBlockIndex;

        public RaiderState()
        {
            TotalScore = 0;
            HighScore = 0;
            TotalChallengeCount = 0;
            RemainChallengeCount = 3;
        }

        public RaiderState(List rawState)
        {
            TotalScore = rawState[0].ToInteger();
            HighScore = rawState[1].ToInteger();
            TotalChallengeCount = rawState[2].ToInteger();
            RemainChallengeCount = rawState[3].ToInteger();
            LatestRewardRank = rawState[4].ToInteger();
            ClaimedBlockIndex = rawState[5].ToLong();
        }

        public IValue Serialize()
        {
            return List.Empty
                .Add(TotalScore.Serialize())
                .Add(HighScore.Serialize())
                .Add(TotalChallengeCount.Serialize())
                .Add(RemainChallengeCount.Serialize())
                .Add(LatestRewardRank.Serialize())
                .Add(ClaimedBlockIndex.Serialize());
        }
    }
}
