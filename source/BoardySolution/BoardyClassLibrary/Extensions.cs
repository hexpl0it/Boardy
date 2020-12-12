using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoardyClassLibrary
{
    public static class Extensions
    {
        public static MMDevice GetDeviceFromPath(this MMDeviceEnumerator devEnum, string devPath)
        {
            using (var mmdeviceCollection = devEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                foreach (var device in mmdeviceCollection)
                {
                    if (device.DevicePath == devPath)
                        return device;
                }
            }

            return null;
        }

    }
}
