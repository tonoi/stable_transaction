// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "forge-std/Test.sol";
import "../contracts/OfflineEscrowContract.sol";
import "../contracts/MockERC20.sol";

contract DepositTest is Test {
    OfflineEscrowContract escrow;
    MockERC20 token;

    function setUp() public {
        token = new MockERC20();
        escrow = new OfflineEscrowContract(address(token));
    }

    function testRemainderToFirstSerial() public {
        bytes[] memory serials = new bytes[](3);
        serials[0] = "\x01";
        serials[1] = "\x02";
        serials[2] = "\x03";
        token.approve(address(escrow), 1000);
        escrow.deposit(1000, serials);
        (bytes memory s0,uint256 a0,,) = escrow.opts(serials[0]);
        (,uint256 a1,,) = escrow.opts(serials[1]);
        (,uint256 a2,,) = escrow.opts(serials[2]);
        assertEq(a0, 334);
        assertEq(a1, 333);
        assertEq(a2, 333);
    }
}
