using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

using Byn.Awrtc.Native;
using Byn.Awrtc.Unity;

public class StreamerCam : MonoBehaviour {
    RenderTexture rtBuffer = null;
    Texture2D texture = null;
    bool rtReady = false;
    public string _DeviceName = "StreamerCam";
    public int _Fps = 60;
    public int _Width = 640;
    public int _Height = 480;

    string usedDeviceName;
    byte[] byteBuffer = null;

    NativeVideoInput videoInput;

    public static StreamerCam instance ;
    bool ready = false;

    private void Awake() {
        usedDeviceName = _DeviceName;
        //SetUpRT();
    }
    // Start is called before the first frame update
    void Start() {
        instance = this;
        Register();
    }

    private void OnDestroy() {
        if (videoInput != null) {
            videoInput.RemoveDevice(usedDeviceName);
        }
    }

    // Update is called once per frame
    void Update() {
        //if (!rtReady) {
        //    SetUpRT();
        //} else {

        //}

        Texture frame = Frame.CameraImage.Texture;
        if (frame != null && ready) {
            Texture2D f = frame as Texture2D;
            byteBuffer = f.GetRawTextureData();
            videoInput.UpdateFrame(usedDeviceName, byteBuffer, f.width, f.height, WebRtcCSharp.VideoType.kBGRA, 0, true);

            if (frame.width > 0 && frame.height > 0) {
                VideoTest.instance.DebugCall(4);
            }
        }
    }

    public void Register() {
        if (ready) {
            return;
        }

        Texture frame = Frame.CameraImage.Texture;
        videoInput = UnityCallFactory.Instance.VideoInput;
        if (videoInput != null && frame != null) {
            videoInput.AddDevice(usedDeviceName, frame.width, frame.height, _Fps);
        }
    }


    void SetUpRT() {
        Texture frame = Frame.CameraImage.Texture;
        if (frame == null) {
            return;
        }

        rtBuffer = new RenderTexture(frame.width, frame.height, 0, RenderTextureFormat.ARGB32);
        rtBuffer.wrapMode = TextureWrapMode.Repeat;

        rtReady = true;
    }
}
