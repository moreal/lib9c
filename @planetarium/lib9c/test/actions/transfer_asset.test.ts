import { Address } from "@planetarium/account";
import { Decimal } from "decimal.js";
import { describe } from "vitest";
import { MINTERLESS_NCG, NCG, TransferAsset } from "../../src/index.js";
import { runTests } from "./common.js";
import { agentAddress } from "./fixtures.js";

describe("TransferAsset", () => {
  describe("odin", () => {
    runTests("valid case", [
      new TransferAsset({
        sender: agentAddress,
        recipient: agentAddress,
        amount: {
          currency: NCG,
          rawValue: BigInt(Decimal.pow(10, 2).mul(2).toString()),
        },
      }),
    ]);
  });
  describe("heimdall", () => {
    runTests("valid case", [
      new TransferAsset({
        sender: agentAddress,
        recipient: agentAddress,
        amount: {
          currency: MINTERLESS_NCG,
          rawValue: BigInt(Decimal.pow(10, 2).mul(2).toString()),
        },
      }),
    ]);
  });
});
