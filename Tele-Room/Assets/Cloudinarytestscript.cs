using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudinaryDotNet;
using System.IO;
using Dummiesman;

public class Cloudinarytestscript : MonoBehaviour
{
    string m_Path;
    string tempPath;
    string tempPathNormal;
    string tempPathObject;
   public GameObject loadedObject;
    List<string> modelIDs;

    Shader shader;

    // Code to load PNG file
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


    // Start is called before the first frame update
    IEnumerator Start()
    {
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


    }

    // Update is called once per frame
    void Update()
    {

    }
}
