// SPDX-License-Identifier: MIT
pragma solidity ^0.8.24;

import "@account-abstraction/accounts/SimpleAccount.sol";

contract JPYCWallet is SimpleAccount {
    constructor(IEntryPoint ep) SimpleAccount(ep) {}
}