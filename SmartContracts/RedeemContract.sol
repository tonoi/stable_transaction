// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "./OfflineEscrowContract.sol";

contract RedeemContract {
    OfflineEscrowContract public immutable escrow;

    constructor(address _escrow) {
        escrow = OfflineEscrowContract(_escrow);
    }

    function redeemBundle(bytes[] calldata serials) external {
        for (uint i=0; i<serials.length; i++) {
            escrow.redeem(serials[i], msg.sender);
        }
    }
}