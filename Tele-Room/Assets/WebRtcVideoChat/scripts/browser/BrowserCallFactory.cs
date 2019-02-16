using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Byn.Awrtc.Browser
{
    public class BrowserCallFactory : IAwrtcFactory
    {
        public static bool IsAvailable()
        {
#if UNITY_WEBGL
            try
            {
                //js side will check if all needed functions are available and if the browser is supported
                return BrowserMediaNetwork.UnityMediaNetwork_IsAvailable();
            }
            catch (EntryPointNotFoundException)
            {
                //method is missing entirely
            }
#endif
            return false;
        }


        public IBasicNetwork CreateDefault(string websocketUrl, IceServer[] lIceServers)
        {
            return new BrowserWebRtcNetwork(websocketUrl, lIceServers);
        }
        public IBasicNetwork CreateBasicNetwork(string websocketUrl, IceServer[] lIceServers = null)
        {
            return new BrowserWebRtcNetwork(websocketUrl, lIceServers);
        }
        public ICall Create(NetworkConfig config = null)
        {
            return CreateCall(config);
        }

        public ICall CreateCall(NetworkConfig config)
        {
            return new BrowserWebRtcCall(config);
        }

        public IMediaNetwork CreateMediaNetwork(NetworkConfig config)
        {
            return new BrowserMediaNetwork(config);
        }

        public void Dispose()
        {

        }


        public bool CanSelectVideoDevice()
        {
            return CAPI.DeviceApi_LastUpdate() > 0;
        }

        public string[] GetVideoDevices()
        {
            int bufflen = 1024;
            byte[] buffer = new byte[bufflen];
            uint len = CAPI.DeviceApi_Devices_Length();
            string[] arr = new string[len];
            for (int i = 0; i < len; i++)
            {
                CAPI.DeviceApi_Devices_Get(i, buffer, bufflen);
                arr[i] = Encoding.UTF8.GetString(buffer);
                Debug.Log("device read: " + arr[i]);
            }
            return arr;
        }
    }
}
