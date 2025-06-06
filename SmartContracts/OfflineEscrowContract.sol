// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@openzeppelin/contracts/token/ERC20/IERC20.sol";

contract OfflineEscrowContract {
    IERC20 public immutable jpyc;
    address public owner;

    struct OPT {
        bytes serial;
        uint256 amount;
        uint256 expiry;
        bool redeemed;
    }

    mapping(bytes => OPT) public opts;

    constructor(address _jpyc) {
        jpyc = IERC20(_jpyc);
        owner = msg.sender;
    }

    event Issued(bytes serial, uint256 amount, uint256 expiry);
    event Redeemed(bytes serial, address merchant);

    function deposit(uint256 amount, bytes[] calldata serials) external {
        require(jpyc.transferFrom(msg.sender, address(this), amount), "transfer failed");
        uint256 per = amount / serials.length;
        uint256 remainder = amount % serials.length;
        uint256 expiry = block.timestamp + 7 days;
        for (uint i=0; i<serials.length; i++) {
            uint256 amt = per;
            if (i == 0) {
                amt += remainder;
            }
            opts[serials[i]] = OPT(serials[i], amt, expiry, false);
            emit Issued(serials[i], amt, expiry);
        }
    }

    function redeem(bytes calldata serial, address merchant) external {
        OPT storage o = opts[serial];
        require(o.amount > 0, "invalid");
        require(!o.redeemed, "double");
        require(o.expiry > block.timestamp, "expired");
        o.redeemed = true;
        require(jpyc.transfer(merchant, o.amount), "payout fail");
        emit Redeemed(serial, merchant);
    }
}