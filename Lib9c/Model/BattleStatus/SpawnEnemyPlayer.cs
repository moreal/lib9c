using System;
using System.Collections;

#nullable disable
namespace Nekoyume.Model.BattleStatus
{
    [Serializable]
    public class SpawnEnemyPlayer : EventBase
    {
        public SpawnEnemyPlayer(CharacterBase character) : base(character)
        {
        }

        public override IEnumerator CoExecute(IStage stage)
        {
            yield return stage.CoSpawnEnemyPlayer((EnemyPlayer)Character);
        }
    }
}
