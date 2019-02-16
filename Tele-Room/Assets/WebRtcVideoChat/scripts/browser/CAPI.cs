using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Byn.Awrtc.Browser
{
    public static class CAPI
    {

        [DllImport("__Internal")]
        public static extern void DeviceApi_Update();
        [DllImport("__Internal")]
        public static extern void DeviceApi_RequestUpdate();
        [DllImport("__Internal")]
        public static extern ulong DeviceApi_LastUpdate();
        [DllImport("__Internal")]
        public static extern uint DeviceApi_Devices_Length();
        [DllImport("__Internal")]
        public static extern string DeviceApi_Devices_Get(int index, byte[] bufferPtr, int buffLen);

        /// <summary>
        ///  None = 0,
        /// Errors = 1,
        /// Warnings = 2,
        /// Verbose = 3
        /// </summary>
        /// <param name="logLevel">Value for the corresponding log level.</param>
        [DllImport("__Internal")]
        public static extern void SLog_SetLogLevel(int logLevel);
    }
}