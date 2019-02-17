using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byn.Awrtc;
using Byn.Awrtc.Unity;
using Photon.Pun;
using Photon.Realtime;

public enum IOType {
    Undefined,
    Streamer,
    Receiver
}

[RequireComponent(typeof(PhotonView))]
public class StreamerChan : CallApp
{
    PhotonView view;

    IOType role = IOType.Undefined;

    protected bool localVideo = true; // bc base version is private???????
    ConnectionId remoteId = ConnectionId.INVALID;
    string useAddress = null;

    protected override void Awake() {
        base.Awake();

        view = GetComponent<PhotonView>();

        if (view == null) {
            throw new MissingComponentException("Missing Photon View!");
        }
    }

    [PunRPC]
    public void GoGo(bool receiver = false) {
        if (receiver) {
            role = IOType.Receiver;
            Debug.Log("I'm Watching!");
            view.RPC("GoGo", RpcTarget.Others, false);
        } else {
            role = IOType.Streamer;
            Debug.Log("I'm Streaming!");
        }


        ConfigureStream();

        SetupCall();

        VideoTest.instance.DebugState(role);
    }

    public void FetchFrame() {
        
    }

    protected override void UpdateFrame(FrameUpdateEventArgs frameUpdateEventArgs) {
        //base.UpdateFrame(frameUpdateEventArgs);

        Debug.Log("hi hi");

        VideoTest.instance.DebugCall(3);

        if (VideoTest.instance) { // && role == IOType.Receiver) {
            VideoTest.instance.SetFrame(frameUpdateEventArgs.Frame, frameUpdateEventArgs.Format);
        }

        
    }

    public override void SetupCall() {
        Debug.Log("Setting up...");

        StreamerCam.instance.Register();

        NetworkConfig netConfig = CreateNetworkConfig();

        Debug.Log(string.Format("Setting up server with NetworkConfig {0}", netConfig));
        mCall = UnityCallFactory.Instance.Create(netConfig);
        if (mCall == null) {
            Debug.LogWarning("Call Failed!");
            return;
        }

        mCall.LocalFrameEvents = localVideo;
        string[] devices = UnityCallFactory.Instance.GetVideoDevices();
        foreach (string d in devices) {
            Debug.Log(d);
        }
        if (devices == null || devices.Length == 0) {
            Debug.LogWarning("No device info???");
        } else {
            Debug.Log("Found devices!");
        }

        Debug.Log("Call created!");
        mCall.CallEvent += Call_CallEvent;

        mMediaConfigInUse = mMediaConfig.DeepClone();

        // Set up default video device
        if (mMediaConfigInUse.Video && string.IsNullOrEmpty(mMediaConfigInUse.VideoDeviceName)) {
            mMediaConfigInUse.VideoDeviceName = UnityCallFactory.Instance.GetDefaultVideoDevice();
        }

        Debug.Log(string.Format("Call configured with MediaConfig {0}", mMediaConfigInUse));
        mCall.Configure(mMediaConfigInUse);

        Join("StreamerChan");
    }

    public override void Join(string address) {
        base.Join(address);
        useAddress = address;
    }

    protected override void Call_CallEvent(object sender, CallEventArgs e) {
        switch (e.Type) {
            case CallEventType.CallAccepted:
                //Outgoing call was successful or an incoming call arrived
                Debug.Log("Connection established");
                remoteId = ((CallAcceptedEventArgs)e).ConnectionId;
                Debug.Log("New connection with id: " + remoteId
                    + " audio:" + mCall.HasAudioTrack(remoteId)
                    + " video:" + mCall.HasVideoTrack(remoteId));

                // DEBUG
                VideoTest.instance.DebugCall(2);

                break;
            case CallEventType.CallEnded:
                //Call was ended / one of the users hung up -> reset the app
                Append("Call ended");
                ResetCall();
                break;
            case CallEventType.ListeningFailed:
                //listening for incoming connections failed
                //this usually means a user is using the string / room name already to wait for incoming calls
                //try to connect to this user
                //(note might also mean the server is down or the name is invalid in which case call will fail as well)
                mCall.Call(useAddress);
                break;

            case CallEventType.ConnectionFailed: {
                    ErrorEventArgs args = e as ErrorEventArgs;
                    Append("Connection failed error: " + args.ErrorMessage);
                    ResetCall();
                }
                break;
            case CallEventType.ConfigurationFailed: {
                    ErrorEventArgs args = e as ErrorEventArgs;
                    Append("Configuration failed error: " + args.ErrorMessage);
                    ResetCall();
                }
                break;

            case CallEventType.FrameUpdate: {
                    VideoTest.instance.DebugCall(5);
                    //new frame received from webrtc (either from local camera or network)
                    if (e is FrameUpdateEventArgs) {
                        UpdateFrame((FrameUpdateEventArgs)e);
                    }
                    break;
                }

            case CallEventType.Message: {
                    //text message received
                    MessageEventArgs args = e as MessageEventArgs;
                    Append(args.Content);
                    break;
                }
            case CallEventType.WaitForIncomingCall: {
                    //the chat app will wait for another app to connect via the same string
                    WaitForIncomingCallEventArgs args = e as WaitForIncomingCallEventArgs;
                    Append("Waiting for incoming call address: " + args.Address);

                    // DEBUG
                    VideoTest.instance.DebugCall(1);

                    break;
                }
        }
    }

    void Append(string txt) {
        Debug.Log(txt);
    }

    protected override NetworkConfig CreateNetworkConfig() {
        NetworkConfig config = base.CreateNetworkConfig();

        // Not bothering with secure for now
        config.SignalingUrl = uSignalingUrl;

        return config;
    }

    public override MediaConfig CreateMediaConfig() {
        MediaConfig config = base.CreateMediaConfig();
        switch (role) {
            case IOType.Streamer:
                config.Video = true; // Send, not receive
                config.VideoDeviceName = "StreamerCam"; // TODO: get camera
                break;
            case IOType.Receiver:
                config.Video = true; // Receive, not send
                config.VideoDeviceName = "StreamerCam";
                break;
            case IOType.Undefined:
                Debug.LogWarning("Waiting for other side, returning null (MediaConfig)!");
                return null;
        }

        config.Audio = false;

        return config;
    }

    void ConfigureStream() {
        mMediaConfig = CreateMediaConfig();

        switch (role) {
            case IOType.Streamer:
                localVideo = true;
                break;
            case IOType.Receiver:
                localVideo = false;
                break;
            case IOType.Undefined:
                Debug.LogWarning("Waiting for other side, returning!");
                return;
        }
    }
}
