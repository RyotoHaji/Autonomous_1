using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequestを使うなら

public class CaptureAndSend : MonoBehaviour
{
    public Camera cam; // 追従カメラ
    public string serverURL = "http://127.0.0.1:5000/upload"; // PythonサーバのURL

    float timer = 0f;
    public float sendInterval = 10f; // 3秒ごとに送る

    void Update()
    {

            CaptureAndSendImage();
        
    }

    public void CaptureAndSendImage()
    {
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
                Debug.Log("受信成功：" + www.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning("ポスト送信失敗: " + www.error);
            }
        }
    }
}
