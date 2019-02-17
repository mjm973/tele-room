using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Byn.Awrtc;
using Byn.Awrtc.Unity;

public class VideoTest : MonoBehaviour
{
    public Texture2D VideoTexture = null;
    public static VideoTest instance = null;


    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugState(IOType role) {
        switch (role) {
            case IOType.Streamer:
                transform.localScale = Vector3.one * 0.5f;
                break;
            case IOType.Receiver:
                transform.localScale = Vector3.one * 2f;
                break;
        }
    }

    public void DebugCall(int idx) {
        switch (idx) {
            case 0:
                mr.material.color = Color.green;
                break;
            case 1:
                mr.material.color = Color.yellow;
                break;
            case 2:
                mr.material.color = Color.white;
                break;
            case 3:
                mr.material.color = Color.gray; // updateframe callback
                break;
            case 4:
                mr.material.color = Color.cyan; // v cam receiving data
                break;
            case 5:
                mr.material.color = Color.red; // receive frame, before callback
                break;
        }
    }

    public void SetFrame(IFrame frame, FramePixelFormat format) {
        if (frame != null) {
            Debug.Log("frameee");
            UnityMediaHelper.UpdateTexture(frame, ref VideoTexture);
            mr.material.mainTexture = VideoTexture;
        }
    }

    public void SetFrameAR(Texture frame, FramePixelFormat format) {

    }
}
