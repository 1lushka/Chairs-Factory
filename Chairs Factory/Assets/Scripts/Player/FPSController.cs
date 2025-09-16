using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float sensitivity;
    [SerializeField] float jumpForce;
    [SerializeField] Camera playerCam;

    Rigidbody rb;
    float xRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        playerCam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
