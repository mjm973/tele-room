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

    public void SetFrame(IFrame frame, FramePixelFormat format) {
        if (frame != null) {
            Debug.Log("frameee");
            UnityMediaHelper.UpdateTexture(frame, ref VideoTexture);
            mr.material.mainTexture = VideoTexture;
        }
    }
}
