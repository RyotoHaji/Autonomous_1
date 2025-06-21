using UnityEngine;

public class JumpOnSpace : MonoBehaviour
{
    public float jumpForce = 5f; // ジャンプ力
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        // 既に空中にいる場合はジャンプしない（地面に触れてるかの判定を追加するとより良い）
        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
    }
}
