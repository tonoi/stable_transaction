// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "forge-std/Test.sol";
import "../SmartContracts/OfflineEscrowContract.sol";
import "../SmartContracts/RedeemContract.sol";
import "../SmartContracts/MockERC20.sol";

contract RedeemTest is Test {
    OfflineEscrowContract escrow;
    RedeemContract redeem;
    MockERC20 token;
    address merchant = address(0xBEEF);

    bytes constant S1 = hex"01";
    bytes constant S2 = hex"02";

    function setUp() public {
        token = new MockERC20();
        escrow = new OfflineEscrowContract(address(token));
        redeem = new RedeemContract(address(escrow));

        bytes[] memory serials = new bytes[](2);
        serials[0] = S1;
        serials[1] = S2;
        token.approve(address(escrow), 1000);
        escrow.deposit(1000, serials);
    }

    function testRedeemBundleTransfers() public {
        bytes[] memory serials = new bytes[](2);
        serials[0] = S1;
        serials[1] = S2;

        vm.prank(merchant);
        redeem.redeemBundle(serials);

        assertEq(token.balanceOf(merchant), 1000);
        (, , , bool r1) = escrow.opts(S1);
        (, , , bool r2) = escrow.opts(S2);
        assertTrue(r1 && r2);
    }

    function testDoubleRedeemFails() public {
        bytes[] memory serials = new bytes[](1);
        serials[0] = S1;
        vm.prank(merchant);
        redeem.redeemBundle(serials);
        vm.prank(merchant);
        vm.expectRevert("double");
        redeem.redeemBundle(serials);
    }

    function testRedeemExpiredFails() public {
        vm.warp(block.timestamp + 8 days);
        bytes[] memory serials = new bytes[](1);
        serials[0] = S1;
        vm.prank(merchant);
        vm.expectRevert("expired");
        redeem.redeemBundle(serials);
    }
}
