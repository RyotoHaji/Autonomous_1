using UnityEngine;
using UnityEngine.Networking;
using System.Collections; 

public class CarControl : MonoBehaviour
{
    Rigidbody rb;
    public float accelerationForce = 10f;
    private float currentSpeed;
    public float maxSpeed = 20f; // 最大速度
    public float forceAmount = 10f;
    private float timer = 0f;
    public float interval = 1f;
    public float speed = 5f;
    public float turnSpeed = 50f; // 角速度
    public float turnTorque = 30f;

    private string currentCommand = "";
    private bool hasCommand = false;
    private int centerX;
    private int redlineX;
    private int whitelineX;
    bool isProcessing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = rb.velocity.magnitude; // 現在の速度
        float maxSpeed = 20f; // 最大速度
    }

    void Update()
    {
        if(!isProcessing)
            StartCoroutine(GetResultAndControl());

    }

    IEnumerator GetResultAndControl()
    {
        isProcessing = true;

        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:5000/getresult");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string result = www.downloadHandler.text;
            Debug.Log("結果：" + result);

            ResponseData responseData = JsonUtility.FromJson<ResponseData>(www.downloadHandler.text);

            centerX = responseData.center;
            redlineX = responseData.right;
            whitelineX = responseData.left ;

            Debug.Log(centerX);
            Debug.Log("responseData: " + responseData.ToString());

            if (centerX >= 128 && centerX <= 134)
            {
                currentCommand = "Straight";
                
            }
            else if (centerX < 128 || redlineX < 50)
            {
                currentCommand = "Left";
            }
            else if (centerX > 134)
            {
                currentCommand = "Right";
            }
            Debug.Log(currentCommand);
            hasCommand = true;
            
        }
        else
        {
            Debug.LogError("通信失敗、エラー内容: " + www.error);
        }

        isProcessing = false;
    }

    void FixedUpdate()
    {
        if (hasCommand)
        {
            if (currentCommand == "Straight" && rb.velocity.magnitude < maxSpeed)
            {
                DriveStraight();
            }
            else if (currentCommand == "Left")
            {
                TurnLeft();
            }
            else if (currentCommand == "Right")
            {
                TurnRight();
            }
        }
    }

    public void DriveStraight()
    {
        rb.AddForce(transform.forward * accelerationForce, ForceMode.Force);
        Debug.Log("直進");
    }

    public void TurnLeft()
    {
        Quaternion deltaRotation = Quaternion.Euler(0, -turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
        Debug.Log("左に回転");
    }

    public void TurnRight()
    {
        Quaternion deltaRotation = Quaternion.Euler(0, turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
        Debug.Log("右に回転");
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