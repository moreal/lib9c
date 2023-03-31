using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Bencodex;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blocks;
using Libplanet.Tx;
using MessagePack;
using MessagePack.Formatters;
using Boolean = Bencodex.Types.Boolean;

namespace Lib9c.Formatters
{
    public class ActionContextFormatter : IMessagePackFormatter<IActionContext>
    {
        public void Serialize(ref MessagePackWriter writer, IActionContext value,
            MessagePackSerializerOptions options)
        {
            byte[] Serialize(object obj) =>
                MessagePackSerializer.Serialize(
                    obj,
                    MessagePackSerializerOptions.Standard.WithResolver(NineChroniclesResolver.Instance));

            var serializedPreviousStates = Serialize(value.PreviousStates);
            var bdict = Dictionary.Empty
                .Add("signer", value.Signer.ToHex())
                .Add("randomSeed", value.Random.Seed)
                .Add("blockAction", value.BlockAction)
                .Add("blockIndex", value.BlockIndex)
                .Add("txId", value.TxId is { } txId ? (Binary)txId.ByteArray : Null.Value)
                .Add("rehearsal", value.Rehearsal)
                .Add("miner", value.Miner.ToHex())
                .Add("genesisHash",
                    value.GenesisHash is { } genesisHash
                        ? (Binary)genesisHash.ByteArray
                        : Null.Value)
                .Add("previousStates", serializedPreviousStates)
                .Add("previousStateRootHash",
                    value.PreviousStateRootHash is { } previousStateRootHash
                        ? (Binary)previousStateRootHash.ByteArray
                        : Null.Value);
            writer.Write(new Codec().Encode(bdict));
        }

        public IActionContext Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            options.Security.DepthStep(ref reader);

            var bytes = reader.ReadBytes();
            if (bytes is null)
            {
                throw new NullReferenceException($"ReadBytes from serialized {nameof(IActionContext)} is null.");
            }

            var bdict = (Dictionary)new Codec().Decode(bytes.Value.ToArray());
            // BlockHash? genesisHash, Address signer, TxId? txId, Address miner, long blockIndex,
            // bool rehearsal, IAccountStateDelta previousStates, IRandom random,
            // HashDigest<SHA256>? previousStateRootHash, bool blockAction

            return new ActionContext(
                BlockHash.FromString((Text)bdict["genesisHash"]),
                new Address(bdict["signer"]),
                new TxId(((Binary)bdict["txId"]).ByteArray),
                new Address(bdict["miner"]),
                (Integer)bdict["blockIndex"],
                (Boolean)bdict["rehearsal"],
                MessagePackSerializer.Deserialize<IAccountStateDelta>(
                    ((Binary)bdict["previousStates"]).ToByteArray(),
                    MessagePackSerializerOptions.Standard.WithResolver(NineChroniclesResolver.Instance)),
                new Random((Integer)bdict["randomSeed"]),
                new HashDigest<SHA256>((Binary)bdict["previousStateRootHash"]),
                (Boolean)bdict["blockAction"]
            );
        }

        private class ActionContext : IActionContext
        {
            public ActionContext(BlockHash? genesisHash, Address signer, TxId? txId, Address miner, long blockIndex, bool rehearsal, IAccountStateDelta previousStates, IRandom random, HashDigest<SHA256>? previousStateRootHash, bool blockAction)
            {
                GenesisHash = genesisHash;
                Signer = signer;
                TxId = txId;
                Miner = miner;
                BlockIndex = blockIndex;
                Rehearsal = rehearsal;
                PreviousStates = previousStates;
                Random = random;
                PreviousStateRootHash = previousStateRootHash;
                BlockAction = blockAction;
            }

            public BlockHash? GenesisHash { get; }
            public Address Signer { get; init; }
            public TxId? TxId { get; }
            public Address Miner { get; init; }
            public long BlockIndex { get; init; }
            public bool Rehearsal { get; init; }
            public IAccountStateDelta PreviousStates { get; init; }
            public IRandom Random { get; init; }
            public HashDigest<SHA256>? PreviousStateRootHash { get; init; }
            public bool BlockAction { get; init; }

            public void PutLog(string log)
            {
                throw new NotImplementedException();
            }

            public bool IsNativeToken(Currency currency)
            {
                throw new NotImplementedException();
            }

            public IActionContext GetUnconsumedContext()
            {
                throw new NotImplementedException();
            }
        }

        private class Random : System.Random, IRandom
        {
            public Random(int seed)
                : base(seed)
            {
                Seed = seed;
            }

            public int Seed { get; private set; }
        }
    }
}
