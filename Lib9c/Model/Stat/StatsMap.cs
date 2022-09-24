using System;
using System.Collections.Generic;
using System.Linq;
using Bencodex.Types;
using Nekoyume.Model.State;

#nullable disable
namespace Nekoyume.Model.Stat
{
    // todo: `Stats`나 `StatModifier`로 대체되어야 함.
    [Serializable]
    public class StatsMap : IStats, IBaseAndAdditionalStats, IState
    {
        public int HP => HasHP ? _statMaps[StatType.HP].TotalValueAsInt : 0;
        public int ATK => HasATK ? _statMaps[StatType.ATK].TotalValueAsInt : 0;
        public int DEF => HasDEF ? _statMaps[StatType.DEF].TotalValueAsInt : 0;
        public int CRI => HasCRI ? _statMaps[StatType.CRI].TotalValueAsInt : 0;
        public int HIT => HasHIT ? _statMaps[StatType.HIT].TotalValueAsInt : 0;
        public int SPD => HasSPD ? _statMaps[StatType.SPD].TotalValueAsInt : 0;

        public bool HasHP => _statMaps.ContainsKey(StatType.HP) &&
                             (_statMaps[StatType.HP].HasValue || _statMaps[StatType.HP].HasAdditionalValue);

        public bool HasATK => _statMaps.ContainsKey(StatType.ATK) &&
                              (_statMaps[StatType.ATK].HasValue || _statMaps[StatType.ATK].HasAdditionalValue);

        public bool HasDEF => _statMaps.ContainsKey(StatType.DEF) &&
                              (_statMaps[StatType.DEF].HasValue || _statMaps[StatType.DEF].HasAdditionalValue);

        public bool HasCRI => _statMaps.ContainsKey(StatType.CRI) &&
                              (_statMaps[StatType.CRI].HasValue || _statMaps[StatType.CRI].HasAdditionalValue);

        public bool HasHIT => _statMaps.ContainsKey(StatType.HIT) &&
                              (_statMaps[StatType.HIT].HasValue || _statMaps[StatType.HIT].HasAdditionalValue);

        public bool HasSPD => _statMaps.ContainsKey(StatType.SPD) &&
                              (_statMaps[StatType.SPD].HasValue || _statMaps[StatType.SPD].HasAdditionalValue);

        public int BaseHP => HasBaseHP ? _statMaps[StatType.HP].ValueAsInt : 0;
        public int BaseATK => HasBaseATK ? _statMaps[StatType.ATK].ValueAsInt : 0;
        public int BaseDEF => HasBaseDEF ? _statMaps[StatType.DEF].ValueAsInt : 0;
        public int BaseCRI => HasBaseCRI ? _statMaps[StatType.CRI].ValueAsInt : 0;
        public int BaseHIT => HasBaseHIT ? _statMaps[StatType.HIT].ValueAsInt : 0;
        public int BaseSPD => HasBaseSPD ? _statMaps[StatType.SPD].ValueAsInt : 0;

        public bool HasBaseHP => _statMaps.ContainsKey(StatType.HP) && _statMaps[StatType.HP].HasValue;
        public bool HasBaseATK => _statMaps.ContainsKey(StatType.ATK) && _statMaps[StatType.ATK].HasValue;
        public bool HasBaseDEF => _statMaps.ContainsKey(StatType.DEF) && _statMaps[StatType.DEF].HasValue;
        public bool HasBaseCRI => _statMaps.ContainsKey(StatType.CRI) && _statMaps[StatType.CRI].HasValue;
        public bool HasBaseHIT => _statMaps.ContainsKey(StatType.HIT) && _statMaps[StatType.HIT].HasValue;
        public bool HasBaseSPD => _statMaps.ContainsKey(StatType.SPD) && _statMaps[StatType.SPD].HasValue;

        public int AdditionalHP => HasAdditionalHP ? _statMaps[StatType.HP].AdditionalValueAsInt : 0;
        public int AdditionalATK => HasAdditionalATK ? _statMaps[StatType.ATK].AdditionalValueAsInt : 0;
        public int AdditionalDEF => HasAdditionalDEF ? _statMaps[StatType.DEF].AdditionalValueAsInt : 0;
        public int AdditionalCRI => HasAdditionalCRI ? _statMaps[StatType.CRI].AdditionalValueAsInt : 0;
        public int AdditionalHIT => HasAdditionalHIT ? _statMaps[StatType.HIT].AdditionalValueAsInt : 0;
        public int AdditionalSPD => HasAdditionalSPD ? _statMaps[StatType.SPD].AdditionalValueAsInt : 0;

        public bool HasAdditionalHP => _statMaps.ContainsKey(StatType.HP) && _statMaps[StatType.HP].HasAdditionalValue;

        public bool HasAdditionalATK =>
            _statMaps.ContainsKey(StatType.ATK) && _statMaps[StatType.ATK].HasAdditionalValue;

        public bool HasAdditionalDEF =>
            _statMaps.ContainsKey(StatType.DEF) && _statMaps[StatType.DEF].HasAdditionalValue;

        public bool HasAdditionalCRI =>
            _statMaps.ContainsKey(StatType.CRI) && _statMaps[StatType.CRI].HasAdditionalValue;

        public bool HasAdditionalHIT =>
            _statMaps.ContainsKey(StatType.HIT) && _statMaps[StatType.HIT].HasAdditionalValue;

        public bool HasAdditionalSPD =>
            _statMaps.ContainsKey(StatType.SPD) && _statMaps[StatType.SPD].HasAdditionalValue;

        public bool HasAdditionalStats => HasAdditionalHP || HasAdditionalATK || HasAdditionalDEF || HasAdditionalCRI ||
                                          HasAdditionalHIT || HasAdditionalSPD;

        private readonly Dictionary<StatType, StatMapEx> _statMaps =
            new Dictionary<StatType, StatMapEx>(StatTypeComparer.Instance);

        protected bool Equals(StatsMap other)
        {
            return Equals(_statMaps, other._statMaps);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StatsMap)obj);
        }

        public override int GetHashCode()
        {
            return _statMaps != null ? _statMaps.GetHashCode() : 0;
        }

        public void AddStatValue(StatType key, decimal value)
        {
            if (!_statMaps.ContainsKey(key))
            {
                _statMaps.Add(key, new StatMapEx(key));
            }

            _statMaps[key].Value += value;
            PostStatValueChanged(key);
        }

        public void AddStatAdditionalValue(StatType key, decimal additionalValue)
        {
            if (!_statMaps.ContainsKey(key))
            {
                _statMaps.Add(key, new StatMapEx(key));
            }

            _statMaps[key].AdditionalValue += additionalValue;
            PostStatValueChanged(key);
        }

        public void AddStatAdditionalValue(StatModifier statModifier)
        {
            AddStatAdditionalValue(statModifier.StatType, statModifier.Value);
        }

        public void SetStatAdditionalValue(StatType key, decimal additionalValue)
        {
            if (!_statMaps.ContainsKey(key))
            {
                _statMaps.Add(key, new StatMapEx(key));
            }

            _statMaps[key].AdditionalValue = additionalValue;
            PostStatValueChanged(key);
        }

        private void PostStatValueChanged(StatType key)
        {
            if (!_statMaps.ContainsKey(key))
                return;

            var statMap = _statMaps[key];
            if (statMap.HasValue ||
                statMap.HasAdditionalValue)
                return;

            _statMaps.Remove(key);
        }

        public IValue Serialize() =>
#pragma warning disable LAA1002
            new Dictionary(
                _statMaps.Select(kv =>
                    new KeyValuePair<IKey, IValue>(
                        kv.Key.Serialize(),
                        kv.Value.Serialize()
                    )
                )
            );
#pragma warning restore LAA1002

        public void Deserialize(Dictionary serialized)
        {
#pragma warning disable LAA1002
            foreach (KeyValuePair<IKey, IValue> kv in serialized)
#pragma warning restore LAA1002
            {
                _statMaps[StatTypeExtension.Deserialize((Binary)kv.Key)] =
                    new StatMapEx((Dictionary)kv.Value);
            }
        }

        public int GetStat(StatType statType, bool ignoreAdditional = false)
        {
            switch (statType)
            {
                case StatType.HP:
                    return ignoreAdditional ? BaseHP : HP;
                case StatType.ATK:
                    return ignoreAdditional ? BaseATK : ATK;
                case StatType.DEF:
                    return ignoreAdditional ? BaseDEF : DEF;
                case StatType.CRI:
                    return ignoreAdditional ? BaseCRI : CRI;
                case StatType.HIT:
                    return ignoreAdditional ? BaseHIT : HIT;
                case StatType.SPD:
                    return ignoreAdditional ? BaseSPD : SPD;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
            }
        }

        public IEnumerable<(StatType statType, int value)> GetStats(bool ignoreZero = false)
        {
            if (ignoreZero)
            {
                if (HasHP)
                    yield return (StatType.HP, HP);
                if (HasATK)
                    yield return (StatType.ATK, ATK);
                if (HasDEF)
                    yield return (StatType.DEF, DEF);
                if (HasCRI)
                    yield return (StatType.CRI, CRI);
                if (HasHIT)
                    yield return (StatType.HIT, HIT);
                if (HasSPD)
                    yield return (StatType.SPD, SPD);
            }
            else
            {
                yield return (StatType.HP, HP);
                yield return (StatType.ATK, ATK);
                yield return (StatType.DEF, DEF);
                yield return (StatType.CRI, CRI);
                yield return (StatType.HIT, HIT);
                yield return (StatType.SPD, SPD);
            }
        }

        public IEnumerable<(StatType statType, int baseValue)> GetBaseStats(bool ignoreZero = false)
        {
            if (ignoreZero)
            {
                if (HasBaseHP)
                    yield return (StatType.HP, BaseHP);
                if (HasBaseATK)
                    yield return (StatType.ATK, BaseATK);
                if (HasBaseDEF)
                    yield return (StatType.DEF, BaseDEF);
                if (HasBaseCRI)
                    yield return (StatType.CRI, BaseCRI);
                if (HasBaseHIT)
                    yield return (StatType.HIT, BaseHIT);
                if (HasBaseSPD)
                    yield return (StatType.SPD, BaseSPD);
            }
            else
            {
                yield return (StatType.HP, BaseHP);
                yield return (StatType.ATK, BaseATK);
                yield return (StatType.DEF, BaseDEF);
                yield return (StatType.CRI, BaseCRI);
                yield return (StatType.HIT, BaseHIT);
                yield return (StatType.SPD, BaseSPD);
            }
        }

        public IEnumerable<(StatType statType, int additionalValue)> GetAdditionalStats(bool ignoreZero = false)
        {
            if (ignoreZero)
            {
                if (HasAdditionalHP)
                    yield return (StatType.HP, AdditionalHP);
                if (HasAdditionalATK)
                    yield return (StatType.ATK, AdditionalATK);
                if (HasAdditionalDEF)
                    yield return (StatType.DEF, AdditionalDEF);
                if (HasAdditionalCRI)
                    yield return (StatType.CRI, AdditionalCRI);
                if (HasAdditionalHIT)
                    yield return (StatType.HIT, AdditionalHIT);
                if (HasAdditionalSPD)
                    yield return (StatType.SPD, AdditionalSPD);
            }
            else
            {
                yield return (StatType.HP, AdditionalHP);
                yield return (StatType.ATK, AdditionalATK);
                yield return (StatType.DEF, AdditionalDEF);
                yield return (StatType.CRI, AdditionalCRI);
                yield return (StatType.HIT, AdditionalHIT);
                yield return (StatType.SPD, AdditionalSPD);
            }
        }

        public IEnumerable<(StatType statType, int baseValue, int additionalValue)> GetBaseAndAdditionalStats(
            bool ignoreZero = false)
        {
            if (ignoreZero)
            {
                if (HasBaseHP || HasAdditionalHP)
                    yield return (StatType.HP, BaseHP, AdditionalHP);
                if (HasBaseATK || HasAdditionalATK)
                    yield return (StatType.ATK, BaseATK, AdditionalATK);
                if (HasBaseDEF || HasAdditionalDEF)
                    yield return (StatType.DEF, BaseDEF, AdditionalDEF);
                if (HasBaseCRI || HasAdditionalCRI)
                    yield return (StatType.CRI, BaseCRI, AdditionalCRI);
                if (HasBaseHIT || HasAdditionalHIT)
                    yield return (StatType.HIT, BaseHIT, AdditionalHIT);
                if (HasBaseSPD || HasAdditionalSPD)
                    yield return (StatType.SPD, BaseSPD, AdditionalSPD);
            }
            else
            {
                yield return (StatType.HP, BaseHP, AdditionalHP);
                yield return (StatType.ATK, BaseATK, AdditionalATK);
                yield return (StatType.DEF, BaseDEF, AdditionalDEF);
                yield return (StatType.CRI, BaseCRI, AdditionalCRI);
                yield return (StatType.HIT, BaseHIT, AdditionalHIT);
                yield return (StatType.SPD, BaseSPD, AdditionalSPD);
            }
        }

        public IEnumerable<StatMapEx> GetStats()
        {
            if (HasHP)
                yield return _statMaps[StatType.HP];
            if (HasATK)
                yield return _statMaps[StatType.ATK];
            if (HasDEF)
                yield return _statMaps[StatType.DEF];
            if (HasCRI)
                yield return _statMaps[StatType.CRI];
            if (HasHIT)
                yield return _statMaps[StatType.HIT];
            if (HasSPD)
                yield return _statMaps[StatType.SPD];
        }

        /// <summery>
        /// 추가 스탯이 붙어 있는 스탯맵을 열거형으로 반환합니다.
        /// 이 스탯맵에는 기본 스탯이 포함되어 있기 때문에 구분해서 사용해야 합니다.
        /// </summery>
        public IEnumerable<StatMapEx> GetAdditionalStats()
        {
            if (HasAdditionalHP)
                yield return _statMaps[StatType.HP];
            if (HasAdditionalATK)
                yield return _statMaps[StatType.ATK];
            if (HasAdditionalDEF)
                yield return _statMaps[StatType.DEF];
            if (HasAdditionalCRI)
                yield return _statMaps[StatType.CRI];
            if (HasAdditionalHIT)
                yield return _statMaps[StatType.HIT];
            if (HasAdditionalSPD)
                yield return _statMaps[StatType.SPD];
        }
    }
}
