using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequestを使うなら

public class CaptureAndSend : MonoBehaviour
{
    public static CaptureAndSend Instance{get;  private set;}

    public Camera cam; // 追従カメラ
    public string serverURL = "http://127.0.0.1:5000/upload"; // PythonサーバのURL

    float timer = 0f;
    public float sendInterval = 1;
    public  int centerX;
    public  int redlineX;
    public  int whitelineX;
    bool isProcessing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 必要なら
        }
        else
        {
            Destroy(gameObject); // 既に存在していたら破棄
        }
    }
    void Update()
    {
        if (!isProcessing)
            CaptureAndSendImage();    
    }
    

    public void CaptureAndSendImage()
    {
        isProcessing = true;
        //画像を撮影
        RenderTexture rt = cam.targetTexture;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // 画像をbyte配列に変換（JPEGに圧縮）
        byte[] bytes = tex.EncodeToJPG();

        // UnityWebRequestで送信
        StartCoroutine(PostImage(bytes));
    }

    IEnumerator PostImage(byte[] imageBytes)
    {
        using (UnityWebRequest www = new UnityWebRequest(serverURL, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(imageBytes);　//送るデータを管理
            www.SetRequestHeader("Content-Type", "image/jpeg"); //追加の情報をサーバーに伝える
            www.downloadHandler = new DownloadHandlerBuffer(); //サーバーからのレスポンスデータを受け取る

            yield return www.SendWebRequest();



            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

                // 2. 変数に格納
                centerX = responseData.center;
                redlineX = responseData.right;
                whitelineX = responseData.left;
                //Debug.Log("受信成功：" + www.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning("ポスト送信失敗: " + www.error);
            }
        }

        isProcessing = false;
    }

    [System.Serializable]
    public class ResponseData
    {
        public int left;
        public int center;
        public int right;
        public int width;
    }
}
