namespace TIKSN.Numbering;

public interface ISerialNumber<TSelf> : ISerial<TSelf>, ISequentialNavigator<TSelf>
    where TSelf : ISerialNumber<TSelf>;
