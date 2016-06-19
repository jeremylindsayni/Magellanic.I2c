using System;
using static Magellanic.I2C.I2cDiagnostics;

namespace Magellanic.I2C.Exceptions
{
    public class I2cDeviceConnectionException : Exception
    {
        public I2cDeviceConnectionException(string message) : base($"{message} Device: {GetDeviceHardwareInformation()}, {GetDeviceOperatingSystem()} {GetDeviceOperatingSystemVersion()}")
        {

        }
    }
}
