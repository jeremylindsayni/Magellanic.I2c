using System;
using static Magellanic.I2C.I2cDiagnostics;

namespace Magellanic.I2C.Exceptions
{
    public class I2cDeviceNotFoundException : Exception
    {
        public I2cDeviceNotFoundException(string message) : base($"{message} Device: {GetDeviceHardwareInformation()}, {GetDeviceOperatingSystem()} {GetDeviceOperatingSystemVersion()}")
        {

        }
    }
}
