using Magellanic.I2C.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace Magellanic.I2C
{
    public static class I2cDiagnostics
    {
        public static string GetDeviceOperatingSystem()
        {
            return new EasClientDeviceInformation().OperatingSystem;
        }

        public static string GetDeviceHardwareInformation()
        {
            var device = new EasClientDeviceInformation();

            return $"{device.SystemManufacturer}, {device.SystemProductName} ({device.SystemSku})";
        }

        public static string GetDeviceOperatingSystemVersion()
        {
            ulong version = 0;
            if (!ulong.TryParse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion, out version))
            {
                return null;
            }
            else
            {
                var versionComponent1 = (version & 0xFFFF000000000000) >> 48;
                var versionComponent2 = (version & 0x0000FFFF00000000) >> 32;
                var versionComponent3 = (version & 0x00000000FFFF0000) >> 16;
                var versionComponent4 = version & 0x000000000000FFFF;

                return $"{versionComponent1}.{versionComponent2}.{versionComponent3}.{versionComponent4}";
            }
        }
        
        public async static Task<List<byte>> DetectI2cDevicesAsync()
        {
            string advancedQueryString = I2cDevice.GetDeviceSelector();

            var deviceInformations = await DeviceInformation.FindAllAsync(advancedQueryString);

            if (!deviceInformations.Any())
            {
                throw new I2cDeviceNotFoundException("No I2C controllers are connected.");
            }

            var matchingAddresses = new List<byte>();

            for (byte i = 0; i < 128; i++)
            {
                var i2cSettings = new I2cConnectionSettings(i);
                
                i2cSettings.BusSpeed = I2cBusSpeed.FastMode;
                
                var i2cDevice = await I2cDevice.FromIdAsync(deviceInformations[0].Id, i2cSettings);

                var addressToReadFrom = new byte[] { 0x00, 0x00 };

                var result = i2cDevice.ReadPartial(addressToReadFrom);

                if (result.Status == I2cTransferStatus.FullTransfer)
                {
                    matchingAddresses.Add(i);
                }
            }

            if (!matchingAddresses.Any())
            {
                throw new I2cDeviceNotFoundException("No I2C Devices found on the controller.");
            }

            return matchingAddresses;
        }
    }
}
