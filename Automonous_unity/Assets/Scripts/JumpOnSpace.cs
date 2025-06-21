using UnityEngine;

public class JumpOnSpace : MonoBehaviour
{
    public float jumpForce = 5f; // �W�����v��
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody��������܂���");
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
        // ���ɋ󒆂ɂ���ꍇ�̓W�����v���Ȃ��i�n�ʂɐG��Ă邩�̔����ǉ�����Ƃ��ǂ��j
        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
    }
}
