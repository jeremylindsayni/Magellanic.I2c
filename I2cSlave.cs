using Windows.Devices.I2c;

namespace Magellanic.I2C
{
    public interface I2cSlave
    {
        void Initialize(byte initialisationAddress);

        I2cDevice Slave { get; }
    }
}
