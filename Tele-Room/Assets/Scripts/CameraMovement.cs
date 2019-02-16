using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    Camera cam;

    bool LockedToMaster = true;
    
    #region Properties



    #endregion


    #region Base callbacks

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) {
            cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        } else {
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            if (!photonView.IsMine) {
                Debug.Log(string.Format("Received Position ({0})", pos.ToString()));
                Sync(pos, rot);
            } else {
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

        //return transform.position;
        Vector3 p = Input.mousePosition;
        p.z = 5;

        p = cam.ScreenToWorldPoint(p);

        return p;
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

    #endregion
}
