using UnityEngine;
using UnityEngine.Networking;
using System.Collections; 

public class CarControl : MonoBehaviour
{
    Rigidbody rb;
    public float accelerationForce = 10f;
    private float currentSpeed;
    public float maxSpeed = 20f; // �ő呬�x
    public float forceAmount = 10f;
    private float timer = 0f;
    public float interval = 1f;
    public float speed = 5f;
    public float turnSpeed = 50f; // �p���x
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
        currentSpeed = rb.velocity.magnitude; // ���݂̑��x
        float maxSpeed = 20f; // �ő呬�x
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
            Debug.Log("���ʁF" + result);

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
            Debug.LogError("�ʐM���s�A�G���[���e: " + www.error);
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
        Debug.Log("���i");
    }

    public void TurnLeft()
    {
        Quaternion deltaRotation = Quaternion.Euler(0, -turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
        Debug.Log("���ɉ�]");
    }

    public void TurnRight()
    {
        Quaternion deltaRotation = Quaternion.Euler(0, turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
        Debug.Log("�E�ɉ�]");
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