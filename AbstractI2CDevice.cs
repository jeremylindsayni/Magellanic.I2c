using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Magellanic.I2C
{
    public abstract class AbstractI2CDevice : I2cSlave
    {
        public I2cDevice Slave { get; set; }

        public async void Initialize(byte initialisationAddress)
        {
            string aqs = I2cDevice.GetDeviceSelector();

            var dis = await DeviceInformation.FindAllAsync(aqs);

            if (dis.Count == 0)
            {
                throw new Exception("No I2C controllers were found on the system");
            }

            var settings = new I2cConnectionSettings(initialisationAddress);

            settings.BusSpeed = I2cBusSpeed.FastMode;

            var i2cDevice = await I2cDevice.FromIdAsync(dis[0].Id, settings);

            if (i2cDevice == null)
            {
                throw new Exception(string.Format(
                    "Slave address {0} on I2C Controller {1} is currently in use by " +
                    "another application. Please ensure that no other applications are using I2C.",
                    settings.SlaveAddress,
                    dis[0].Id));
            }

            this.Slave = i2cDevice;
        }
    }
}
