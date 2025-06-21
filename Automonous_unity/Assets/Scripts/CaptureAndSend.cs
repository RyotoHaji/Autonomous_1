using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequest���g���Ȃ�

public class CaptureAndSend : MonoBehaviour
{
    public Camera cam; // �Ǐ]�J����
    public string serverURL = "http://127.0.0.1:5000/upload"; // Python�T�[�o��URL

    float timer = 0f;
    public float sendInterval = 10f; // 3�b���Ƃɑ���

    void Update()
    {

            CaptureAndSendImage();
        
    }

    public void CaptureAndSendImage()
    {
        //�摜���B�e
        RenderTexture rt = cam.targetTexture;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // �摜��byte�z��ɕϊ��iJPEG�Ɉ��k�j
        byte[] bytes = tex.EncodeToJPG();

        // UnityWebRequest�ő��M
        StartCoroutine(PostImage(bytes));
    }

    IEnumerator PostImage(byte[] imageBytes)
    {
        using (UnityWebRequest www = new UnityWebRequest(serverURL, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(imageBytes);�@//����f�[�^���Ǘ�
            www.SetRequestHeader("Content-Type", "image/jpeg"); //�ǉ��̏����T�[�o�[�ɓ`����
            www.downloadHandler = new DownloadHandlerBuffer(); //�T�[�o�[����̃��X�|���X�f�[�^���󂯎��

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("��M�����F" + www.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning("�|�X�g���M���s: " + www.error);
            }
        }
    }
}
