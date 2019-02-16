/* 
 * Copyright (C) 2015 Christoph Kutza
 * 
 * Please refer to the LICENSE file for license information
 */
var UnityWebRtcNetwork =
{
	UnityWebRtcNetworkIsAvailable:function()
    {
		if("awrtc" in window && typeof awrtc.CAPIWebRtcNetworkIsAvailable === 'function')
		{
			return awrtc.CAPIWebRtcNetworkIsAvailable();
		}
		return false;
    },
	UnityWebRtcNetworkCreate:function(lConfiguration)
	{
		return awrtc.CAPIWebRtcNetworkCreate(Pointer_stringify(lConfiguration));
	},
	UnityWebRtcNetworkRelease:function(lIndex)
	{
		awrtc.CAPIWebRtcNetworkRelease(lIndex);
	},
	UnityWebRtcNetworkConnect:function(lIndex, lRoom)
	{
		return awrtc.CAPIWebRtcNetworkConnect(lIndex, Pointer_stringify(lRoom));
	},
	UnityWebRtcNetworkStartServer:function(lIndex, lRoom)
	{
		awrtc.CAPIWebRtcNetworkStartServer(lIndex, Pointer_stringify(lRoom));
	},
	UnityWebRtcNetworkStopServer:function(lIndex)
	{
		awrtc.CAPIWebRtcNetworkStopServer(lIndex);
	},
	UnityWebRtcNetworkDisconnect:function(lIndex, lConnectionId)
	{
		awrtc.CAPIWebRtcNetworkDisconnect(lIndex, lConnectionId);
	},
	UnityWebRtcNetworkShutdown:function(lIndex)
	{
		awrtc.CAPIWebRtcNetworkShutdown(lIndex);
	},
	UnityWebRtcNetworkUpdate:function(lIndex)
	{
		awrtc.CAPIWebRtcNetworkUpdate(lIndex);
	},
	UnityWebRtcNetworkFlush:function(lIndex)
	{
		awrtc.CAPIWebRtcNetworkFlush(lIndex);
	},
	UnityWebRtcNetworkSendData:function(lIndex, lConnectionId, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lReliable)
	{
		var sndReliable = true;
		if(lReliable == false || lReliable == 0 || lReliable == "false" || lReliable == "False")
			sndReliable = false;
		return awrtc.CAPIWebRtcNetworkSendDataEm(lIndex, lConnectionId, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, sndReliable);
	},
	UnityWebRtcNetworkPeekEventDataLength:function(lIndex)
	{
		return awrtc.CAPIWebRtcNetworkPeekEventDataLength(lIndex);
	},
	UnityWebRtcNetworkDequeue:function(lIndex, lTypeIntArrayPtr, lConidIntArrayPtr, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lDataLenIntArrayPtr )
	{
		var val = awrtc.CAPIWebRtcNetworkDequeueEm(lIndex, HEAP32, lTypeIntArrayPtr >> 2, HEAP32, lConidIntArrayPtr >> 2, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, HEAP32, lDataLenIntArrayPtr >> 2);
		return val;
	},
	UnityWebRtcNetworkPeek:function(lIndex, lTypeIntArrayPtr, lConidIntArrayPtr, lUint8ArrayDataPtr, lUint8ArrayDataOffset, lUint8ArrayDataLength, lDataLenIntArrayPtr )
	{
		var val = awrtc.CAPIWebRtcNetworkPeekEm(lIndex, HEAP32, lTypeIntArrayPtr >> 2, HEAP32, lConidIntArrayPtr >> 2, HEAPU8, lUint8ArrayDataPtr + lUint8ArrayDataOffset, lUint8ArrayDataLength, HEAP32, lDataLenIntArrayPtr >> 2);
		return val;
	}
};

var UnityMediaNetwork =
{
    //function awrtc.CAPIMediaNetwork_IsAvailable(): boolean
    UnityMediaNetwork_IsAvailable: function () {
		//hacky way to make sure the device info is available as early as possible
		awrtc.CAPI_DeviceApi_Update();
        if ("awrtc" in window && typeof awrtc.CAPIMediaNetwork_IsAvailable === 'function') {
            return awrtc.CAPIMediaNetwork_IsAvailable();
        }
        return false;
    },
    //function CAPIMediaNetwork_Create(lJsonConfiguration):number
    UnityMediaNetwork_Create: function (lJsonConfiguration) {
        return awrtc.CAPIMediaNetwork_Create(Pointer_stringify(lJsonConfiguration));
    },
    //function CAPIMediaNetwork_Configure(lIndex:number, audio: boolean, video: boolean,
    //minWidth: number, minHeight: number,
    //maxWidth: number, maxHeight: number,
    //idealWidth: number, idealHeight: number,
    //minFps: number, maxFps: number, idealFps: number, deviceName: string = "")
	UnityMediaNetwork_Configure: function (lIndex, audio, video,
        minWidth, minHeight,
        maxWidth, maxHeight,
        idealWidth, idealHeight,
		minFps, maxFps, idealFps, deviceName) {
        awrtc.CAPIMediaNetwork_Configure(lIndex, audio, video, minWidth, minHeight, maxWidth, maxHeight, idealWidth, idealHeight, minFps, maxFps, idealFps, Pointer_stringify(deviceName));
    },
    //function CAPIMediaNetwork_GetConfigurationState(lIndex: number): number
    UnityMediaNetwork_GetConfigurationState: function (lIndex) {
        return awrtc.CAPIMediaNetwork_GetConfigurationState(lIndex);
    },
    //function CAPIMediaNetwork_GetConfigurationError(lIndex: number): string
    UnityMediaNetwork_GetConfigurationError: function (lIndex) {
        //TODO:
        return awrtc.CAPIMediaNetwork_GetConfigurationError(lIndex);
    },
    //function awrtc.CAPIMediaNetwork_ResetConfiguration(lIndex: number) : void 
    UnityMediaNetwork_ResetConfiguration: function (lIndex) {
        awrtc.CAPIMediaNetwork_ResetConfiguration(lIndex);
    },
    //function awrtc.CAPIMediaNetwork_TryGetFrame(lIndex: number, lConnectionId: number, lWidthInt32Array: Int32Array, lWidthIntArrayIndex: number, lHeightInt32Array: Int32Array, lHeightIntArrayIndex: number, lBufferUint8Array: Uint8Array, lBufferUint8ArrayOffset: number, lBufferUint8ArrayLength: number): boolean
    UnityMediaNetwork_TryGetFrame: function (lIndex, lConnectionId, lWidthInt32ArrayPtr, lHeightInt32ArrayPtr, lBufferUint8ArrayPtr, lBufferUint8ArrayOffset, lBufferUint8ArrayLength) {
        return awrtc.CAPIMediaNetwork_TryGetFrame(lIndex, lConnectionId,
                                        HEAP32, lWidthInt32ArrayPtr >> 2,
                                        HEAP32, lHeightInt32ArrayPtr >> 2,
                                        HEAPU8, lBufferUint8ArrayPtr + lBufferUint8ArrayOffset, lBufferUint8ArrayLength);
    },
    //function awrtc.CAPIMediaNetwork_TryGetFrameDataLength(lIndex: number, connectionId: number) : number
    UnityMediaNetwork_TryGetFrameDataLength: function (lIndex, connectionId) {
        return awrtc.CAPIMediaNetwork_TryGetFrameDataLength(lIndex, connectionId);
    },
    UnityMediaNetwork_TryGetFrameDataLength: function (lIndex, connectionId) {
        return awrtc.CAPIMediaNetwork_TryGetFrameDataLength(lIndex, connectionId);
    },
    UnityMediaNetwork_SetVolume: function(lIndex, volume, connectionId) {
        awrtc.CAPIMediaNetwork_SetVolume(lIndex, volume, connectionId);
    },
    UnityMediaNetwork_HasAudioTrack: function(lIndex, connectionId) {
        return awrtc.CAPIMediaNetwork_HasAudioTrack(lIndex, connectionId);
    },
    UnityMediaNetwork_HasVideoTrack: function(lIndex, connectionId) {
        return awrtc.CAPIMediaNetwork_HasVideoTrack(lIndex, connectionId);
    },
    UnityMediaNetwork_SetMute: function(lIndex, value) {
        awrtc.CAPIMediaNetwork_SetMute(lIndex, value);
    },
    UnityMediaNetwork_IsMute: function(lIndex) {
        return awrtc.CAPIMediaNetwork_IsMute(lIndex);
    },
	DeviceApi_Update: function()
	{
		awrtc.CAPI_DeviceApi_Update();
	},
	DeviceApi_RequestUpdate: function()
	{
		awrtc.CAPI_DeviceApi_RequestUpdate();
	},
	DeviceApi_LastUpdate: function()
	{
		return awrtc.CAPI_DeviceApi_LastUpdate();
	},
	DeviceApi_Devices_Length: function()
	{
		return awrtc.CAPI_DeviceApi_Devices_Length();
	},
	DeviceApi_Devices_Get: function(lIndex, lBufferPtr, lBufferLen)
	{
		var jsres = awrtc.CAPI_DeviceApi_Devices_Get(lIndex);
		
		//Unity 2017 uses Module.stringToUTF8
		//Some later Unity 2018 version updated emscripten
		//that uses stringToUTF8 as global
		var strToUTF8 = stringToUTF8;
		if(typeof strToUTF8 !== "function") 
		    strToUTF8 = window.Module.stringToUTF8;

		//will copy to HEAPU8 at lBufferPtr 
		strToUTF8(jsres, lBufferPtr, lBufferLen);
	},
	SLog_SetLogLevel: function(loglevel)
	{
		awrtc.CAPI_SLog_SetLogLevel(loglevel);
	}
}

mergeInto(LibraryManager.library, UnityWebRtcNetwork);
mergeInto(LibraryManager.library, UnityMediaNetwork);
