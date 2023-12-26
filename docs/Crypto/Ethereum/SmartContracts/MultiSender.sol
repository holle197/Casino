pragma solidity >=0.6.12 <0.9.0;

contract MultiSender {

  address private owner;
     constructor (){
    owner = msg.sender;
  }

  //only publisher of the contract are allowed to use methods that require this modifier
  modifier OnlyOwner{
    require(msg.sender ==owner);
    _;
  }

  //length of addresses must be equal of length of funds
  modifier CheckEqualityOfAddressesAndFunds(address[] memory addresses, uint[] memory funds){
  require(addresses.length == funds.length);
  _;
  }
  //sum of all funds that are ready to be sent must be equal to the total amount 
  modifier CheckTotalSumEquality(uint[] memory values,uint256 total){
    uint256 sum = 0;
    for(uint i=0;i < values.length;i++){
        sum += values[i];
    }
    require(total == sum);
    _;
  }

  function GetOwner() public view returns(address){
        return owner;
  }

  function ChangeOwner(address newOwner) external OnlyOwner returns(bool) {
    owner = newOwner;
    return owner == newOwner;
  } 

  function Send( address[] memory addresses,uint[] memory funds ) external payable OnlyOwner CheckTotalSumEquality(funds,msg.value) CheckEqualityOfAddressesAndFunds(addresses,funds) returns (bool){
        //modifier checks all requirements 
        uint8 checker= 0;
        for(uint i=0;i<addresses.length;i++){
              address payable toAddress = payable(addresses[i]);
              toAddress.transfer(funds[i]);
              checker++;
        }
        if(checker == addresses.length) return true;
        //rejected tx
        revert();
  }

}