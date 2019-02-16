using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Photon.Pun;
using GoogleARCore;

public class CameraMovement : MonoBehaviourPunCallbacks, IPunObservable {
    Camera cam;

    bool LockedToMaster = true;
    bool tracking = true;

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
            Debug.Log("frick");
            DisableTracking();
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
