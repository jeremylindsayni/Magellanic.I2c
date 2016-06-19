using Magellanic.I2C.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Magellanic.I2C
{
    public abstract class AbstractI2CDevice : I2cSlave
    {
        public byte[] DeviceIdentifier { get; set; }

        public I2cDevice Slave { get; set; }

        public abstract byte[] GetDeviceId();

        public abstract byte GetI2cAddress();

        public async Task Initialize()
        {
            string advancedQueryString = I2cDevice.GetDeviceSelector();

            var deviceInformations = await DeviceInformation.FindAllAsync(advancedQueryString);

            if (!deviceInformations.Any())
            {
                throw new I2cDeviceNotFoundException("No I2C controllers are connected.");
            }

            var i2cSettings = new I2cConnectionSettings(this.GetI2cAddress());

            i2cSettings.BusSpeed = I2cBusSpeed.FastMode;

            var i2cDevice = await I2cDevice.FromIdAsync(deviceInformations[0].Id, i2cSettings);

            if (i2cDevice == null)
            {
                throw new I2cSlaveAddressInUseException(
                    $"Slave address {i2cSettings.SlaveAddress} on I2C Controller {deviceInformations[0].Id} is currently in use by another device or application");
            }

            this.Slave = i2cDevice;
        }

        public bool IsConnected()
        {
            if (this.DeviceIdentifier?.Length == 0)
            {
                throw new I2cDeviceConnectionException("Specify DeviceIdentifier byte(s) before checking if the device is connected.");
            }

            var identifierReadFromDevice = this.GetDeviceId();

            if (identifierReadFromDevice?.Length == 0)
            {
                throw new I2cDeviceConnectionException("No bytes were read back from the device for identification");
            }

            if (identifierReadFromDevice == this.DeviceIdentifier)
            {
                return true;
            }

            for (int i = 0; i < this.DeviceIdentifier.Length; i++)
            {
                if (identifierReadFromDevice[i] != this.DeviceIdentifier[i])
                {
                    throw new I2cDeviceConnectionException("Device has an unexpected device identifier.");
                }
            }

            return true;
        }
    }
}