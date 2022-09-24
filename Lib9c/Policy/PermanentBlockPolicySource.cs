﻿using System.Collections.Immutable;
using Libplanet;

#nullable disable
namespace Nekoyume.BlockChain.Policy
{
    public static class PermanentBlockPolicySource
    {
        public const long AuthorizedMinersPolicyInterval = 2;

        public static readonly ImmutableHashSet<Address> AuthorizedMiners = new Address[]
        {
            new Address("82b857D3fE3Bd09d778B40f0a8430B711b3525ED"),
        }.ToImmutableHashSet();

        public static readonly ImmutableHashSet<Address> PermissionedMiners = new Address[]
        {
            new Address("211afcd0E152A61C92600D6a5a63Ca088a85Fbb1"),
            new Address("8a393e376d6Fd3b837314c7d4e249cc90a6B7B17"),
            new Address("590c887BDac8d957Ca5d3c1770489Cf2aFBd868E"),
            new Address("2b8e3F6A88BeB833682b20666fD1814836C39293"),
            new Address("BdA56083bccdb09583c64Af10Ce7e78679abD6C6"),
            new Address("2FF6f9fd3c013F37bC34C3efCBdD997f00617595"),
            new Address("CEF99ff33C1F6214746B5CA450d9a9041161E648"),
            new Address("c33c64a22af3794e6e05621193424a59e033874e"),
            new Address("8129A10a23d468377c2e0CE553B4914C161c158B"),
            new Address("21ebd41b89ef931cf89d81da3e65f0007930d4fa"),
            new Address("4A4C97E3D01bC2337e53C8200df473d8a77cB6b2"),
            new Address("4FEC7CacCd0dCcdbBD150b78f3cAd8947a052E65"),
            new Address("19165456076F35B94363b15CB1C6255da7a30610"),
            new Address("def16a37D53455a9b3b0231Fc8f4966f6805f038"),
            new Address("057a4fD9d5d414580bD86803aE17f3807750ABfe"),
            new Address("e6f030E3E9Df6628A8FE3707AE4DFD41664999cf"),
            new Address("0d6312c647f9c64b9bd9cf9653d3288af02dc9c1"),
            new Address("Fb5600c0AA73a40F67c4dCc9b178654612DEfFe5"),
            new Address("3BdC87fAb06CF3358F87c0368b3086f288f7a95f"),
            new Address("EEE0F468D0214c389693b119Cec2094Af727574f"),
            new Address("1012041FF2254f43d0a938aDF89c3f11867A2A58"),
            new Address("0268eB60f7efe8f5e3c4c6972D4a2072D47cb45C"),
            new Address("f784268f65035b21dac64778087ea1fac8bfadbc"),
            new Address("057B7843a6FCd8C4d24744b1E9F32A4a3b974728"),
            new Address("e391aa290D47EB23b2124C61784b4E9d2652d0b0"),
            new Address("91392F800989E76AFe30dBF4422B6dcA9F6cfE25"),
            new Address("95E0DE5fc49276c6d3fcB41757374AAdB2b1B38B"),
            new Address("a49d64c31a2594e8fb452238c9a03befd1119963"),
            new Address("5E7158B44aD5FCC969dBed9905a8f5FF882Ea713"),
            new Address("bda9AF57a4cD5e6Ce737a493Ecb1C5BE358FbBA9"),
            new Address("D9fCe9E571ed4bdB34D3CF46A918Eb6938571a5B"),
            new Address("b2e8AbcB5bd4744321B0F655bf3D34CC2AD9f18F"),
            new Address("E47E152F38109EcFbc73ff894114188496A315dB"),
            new Address("f6844E6b07C4f64eA85456B06233fC10c1D350EC"),
            new Address("0e50540d1604AB21Fa167FA2e1d04C3635FcA4D9"),
            new Address("F76FBe6F125bCD91931D420726b43ec6331195E7"),
            new Address("493C16b25E6Be0735F85D78d3226bC3E1260eb59"),
            new Address("9F2C5e93E8586D08a1045Ec58096f6422627C970"),
            new Address("226e0ed8b2ea2fac5a9927babce45dfa969489d4"),
            new Address("194bf61B6743F0776461C4A5cE1E1eBB9F62BECa"),
            new Address("22A28AEB62A9ED8DF939c1D18dAf1d64fF300Cbb"),
            new Address("6374FE5F54CdeD72Ff334d09980270c61BC95186"),
            new Address("d285A1498eE3187fF8ea11088a375DBE166d58F6"),
            new Address("07e3ac1f2e67cced644683618ed71ce24f500037"),
            new Address("97998cd578dee2fac4d222c0ae1bb42cb3ab0279"),
            new Address("AD1b4A3d126e2dFfF62c22a2ed83c497Ed0F619B"),
            new Address("5EdFEE0f7E22Ba61Ed1021fA1c6c0423c27DcB21"),
            new Address("73E91e6d7647Baa21521cF362e5464A3C27bcC0c"),
            new Address("CDA77446aD6da472f5f413a5a4904D370bc83aCA"),
            new Address("0558A7d65159DdF3e17dD489107111061bcCab94"),
            new Address("199458f5CABD6F782F90f7c13a644FA0e7A182eD"),
            new Address("35dc9585796E7b5DaC310c92dC67B6B7142A6C40"),
            new Address("eF981A0Bbcc76C57f3E6D7c82504b5F08A8877C8"),
            new Address("497a9210BA0aba4574A17fFD5a9D0592F0cf343a"),
            new Address("92DB83D2240d108bbeb1DE8C37ea8532e807FF78"),
            new Address("d736718F24DDE5dfe5a407767644eB0599cCf533"),
            new Address("8B30A122D23D0c8AB5215880Dc0C2Fb0a4F4b8d3"),
            new Address("c046724401405AA8ef8321588b5818E4144A9638"),
            new Address("D0043Fb67c33fBe14787e96573C5C4273A0ea4D4"),
            new Address("4D14Be1F5bAA885b36151586308e897bCf4754d0"),
            new Address("7D6C386aAb01D57Fc85CA856c73c5A073954ab03"),
            new Address("552899453F1f7475D724dabbF1E351940138Ce07"),
            new Address("D478d3A6cFe30C32F501e80D1d228C5d2881c4Cf"),
            new Address("a44383c1D4f12920f0eb8c77cca24886eA666dd5"),
            new Address("625D3aF1b2D9f6D13fc91b7AF6dde41A3c72B2AC"),
            new Address("AA1AF83d5D20F1D82F1F755971577B75833c30ed"),
            new Address("00C32B4d7c690B5F900da469f4302f6496810B62"),
            new Address("d936f927942ae879527b596bf87688f85f3a1e04"),
            new Address("Ad2a3cF68Bd99bAEC1084C991B1cb01b470Fa2b6"),
            new Address("C0bA278CB8379683E66C28928fa0Aa8bfF3D95E6"),
            new Address("3f57C197e231DEBE35D2771F9daBCBfd87749b8d"),
            new Address("CbAf782c7E7736a998021A7A6DedC85410a7acb4"),
            new Address("38e782a80Bec4B7821d14CFc0b49489aEB0AfF42"),
            new Address("Dcdffe41264A7f9e5E35fD120233A6673069F544"),
            new Address("2f8a40FD4F133568dFC84bd1e850D243c9c04afd"),
            new Address("CfaA743C6d3e957Dc7902057cD4b8217aA1BEBfc"),
        }.ToImmutableHashSet();
    }
}
