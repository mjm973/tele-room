using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Photon.Pun;
using GoogleARCore;

public class CameraMovement : MonoBehaviourPunCallbacks, IPunObservable {
    Camera cam;

    public GameObject model;
    public float dist;

    bool LockedToMaster = true;
    bool tracking = true;

    public AudioClip MusicClip;
    public AudioSource MusicSource;

    #region Properties



    #endregion
        

    #region Base callbacks

    void Awake() {
        cam = GetComponent<Camera>();
        if (cam == null) {
            cam = Camera.main;
        }

        if (CheckDisable()) {
            DisableTracking();
        }

    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.clip = MusicClip;
        //cam = GetComponent<Camera>();
        //if (cam == null) {
        //    cam = Camera.main;
        //}

        //if (photonView.IsMine) {
        //    TrackedPoseDriver tpd = GetComponent<TrackedPoseDriver>();
        //    GoogleARCore.ARCoreSession session = GetComponent<GoogleARCore.ARCoreSession>();
        //    if (tpd) {
        //        tpd.enabled = false;
        //        session.enabled = false;
        //        Debug.Log("Tracking Disabled");
        //    }
        //}
    }

    // Update is called once per frame
    void Update() {
        if (CheckDisable()) {
            Debug.Log("oh forking shirtballs");
            DisableTracking();
        }

  //Detect Screen tap
        Touch touch = Input.GetTouch(0);
        RaycastHit hit;

        switch (touch.phase)
        {

            case TouchPhase.Began:


                Vector2 startPos = touch.position;

                Vector3 tapPosFar = new Vector3(startPos.x, startPos.y, (cam.nearClipPlane + dist));
                Vector3 tapPosNear = new Vector3(startPos.x, startPos.y, (cam.nearClipPlane));

                Vector3 tapPosF = cam.ScreenToWorldPoint(tapPosFar);
                Vector3 tapPosN = cam.ScreenToWorldPoint(tapPosNear);
                
                //Touch Raycast + Audio 

                if (Physics.Raycast(tapPosN, tapPosF - tapPosN, out hit) && hit.transform.tag == "model")
                {
                    MusicSource.Play();
                }
                else      //Tap to instantiate objects
                {

                    Instantiate(model, tapPosF, transform.rotation);
                }

                break;
        }


 

        //Line of sight raycast + Haptic
        Ray lray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(lray, out hit))
        {
            if (hit.transform.tag == "model")
            {
                Handheld.Vibrate();
            }
        }



    }

    #endregion

    #region PUN Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        Debug.Log("merp");
        if (stream.IsWriting) {
            Vector3 pos = GetPosition();
            Quaternion rot = GetRotation();
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else {
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            if (!photonView.IsMine) {
                Debug.Log(string.Format("Received Position ({0})", pos.ToString()));
                Sync(pos, rot);
            }
            else {
                Debug.Log("View is mine :(");
            }
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        Debug.Log("Hi");
        //Debug.Log(string.Format("Instantiated by master? {0}", info.Sender.IsMasterClient));
    }

    #endregion

    #region Methods

    Vector3 GetPosition() {
        return transform.position;
    }

    Quaternion GetRotation() {
        return transform.rotation;
    }

    void Sync(Vector3 pos, Quaternion rot) {
        if (LockedToMaster) {
            transform.position = pos;
            transform.rotation = rot;
        }
    }

    bool CheckDisable() {
        return tracking && !photonView.IsMine && PhotonNetwork.InRoom;
    }

    void DisableTracking() {
        TrackedPoseDriver tpd = GetComponent<TrackedPoseDriver>();
        ARCoreSession session = GetComponent<ARCoreSession>();
        InstantPreviewTrackedPoseDriver iptpd = GetComponent<InstantPreviewTrackedPoseDriver>();
        if (tpd) {
            tpd.enabled = false;
        }
        if (session) {
            session.enabled = false;
        }
        if (iptpd) {
            iptpd.enabled = false;
        }
        tracking = false;
        Debug.Log("Tracking Disabled");

    }


    #endregion
}
