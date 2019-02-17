using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Photon.Pun;
using GoogleARCore;
using CloudinaryDotNet;
using System.IO;
using Dummiesman;

public class CameraMovement : MonoBehaviourPunCallbacks, IPunObservable {
    Camera cam;

    public GameObject model;
    public float dist = 1.5f;

    bool LockedToMaster = true;
    bool tracking = true;

    public AudioClip MusicClip;
    public AudioSource MusicSource;

    //Cloudinary setup
    string m_Path;
    string tempPath;
    string tempPathNormal;
    string tempPathObject;
    public GameObject loadedObject;
    List<string> modelIDs;
    Shader shader;

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    //Code to convert String into Stream
    //https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string
    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

//End of Cloudinary setup



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
    IEnumerator Start() {
        MusicSource.clip = MusicClip;

        //Load Cloudinary object

        CloudinaryDotNet.Account account = new CloudinaryDotNet.Account("dti0lstz7", "536872319738796", "0duscwiC_5ncAS86R0vr6dEOGXo");
        CloudinaryDotNet.Cloudinary cloudinary = new CloudinaryDotNet.Cloudinary(account);


        //Get the path of the Game data folder
        //m_Path = Application.dataPath + "/testUpload.jpg";
        //Output the Game data path to the console

        CloudinaryDotNet.Actions.SearchResult result = cloudinary.Search()
          .Expression("tags=object")
          .SortBy("public_id", "desc")
          .MaxResults(30)
          .Execute();


        // ############################################################################################### 
        // ############### Auto Loader of Models from Cloudinary, Supports Multiple Models ###############
        // ############### NOT COMPLETED YET #############################################################
        // ###############################################################################################

        // All item IDs on Cloudinary with tag "object" are put into a List
        modelIDs = new List<string>();

        foreach (CloudinaryDotNet.Actions.SearchResource item in result.Resources)
        {
            modelIDs.Add(item.PublicId);
        }

        // Files on Cloudinary should follow the naming convention "modelName-FileType"
        // (eg. "Orange-Normal.jpg)
        foreach (string i in modelIDs)
        {
            string[] split = i.Split('-');
            Debug.Log(split[1]);
        }

        // ############################################################################################### 
        // ############################################################################################### 
        // ############################################################################################### 
        // ############################################################################################### 


        // TargetID of the color file online
        string targetIDNormal = "http://res.cloudinary.com/dti0lstz7/image/upload/v1550299645/" + result.Resources[0].PublicId;
        string targetID = "http://res.cloudinary.com/dti0lstz7/image/upload/v1550299645/" + result.Resources[2].PublicId;
        string targetIDObj = "http://res.cloudinary.com/dti0lstz7/raw/upload/v1550299645/" + result.Resources[1].PublicId;

        string objectString;
        using (WWW www = new WWW(targetIDObj))
        {
            yield return www;
            //www.text(objectString);
            objectString = www.text;
            //Debug.Log(objectString);

            // Load function takes a stream, so we need to convert string to stream

            using (var stream = GenerateStreamFromString(objectString))
            {
                loadedObject = new OBJLoader().Load(stream);
            }
        }

        Material myNewMaterial = new Material(Shader.Find("Standard"));

        //Download texture from Cloudinary and applies it to Model
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(targetID))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);
            myNewMaterial.SetTexture("_MainTex", tex);
        }

        //Download Normal mapping from Cloudinary and applies it to Model
        Texture2D texNormal;
        texNormal = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(targetIDNormal))
        {
            yield return www;
            www.LoadImageIntoTexture(texNormal);
            myNewMaterial.SetTexture("_Texture", texNormal);
        }


        loadedObject.GetComponentInChildren<Renderer>().material = myNewMaterial;





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

                model = loadedObject;
                model.tag = "model";
                
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


    //Mouse controls for testing
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 startPos = touch.position;

            Vector3 tapPosFar = new Vector3(startPos.x, startPos.y, (cam.nearClipPlane + dist));
            Vector3 tapPosNear = new Vector3(startPos.x, startPos.y, (cam.nearClipPlane));

            Vector3 tapPosF = cam.ScreenToWorldPoint(tapPosFar);
            Vector3 tapPosN = cam.ScreenToWorldPoint(tapPosNear);

            model = loadedObject;
            model.tag = "model";

            //Touch Raycast + Audio 

            if (Physics.Raycast(tapPosN, tapPosF - tapPosN, out hit) && hit.transform.tag == "model")
            {
                MusicSource.Play();
            }
            else      //Tap to instantiate objects
            {

                Instantiate(model, tapPosF, transform.rotation);
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
