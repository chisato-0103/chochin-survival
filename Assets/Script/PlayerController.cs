using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;
    public float acceleration = 20f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // カメラ(=プレイヤーの向き)基準で移動
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        moveInput = (right * h + forward * v).normalized;
    }

    void FixedUpdate()
    {
        Vector3 targetVel = moveInput * moveSpeed;
        Vector3 vel = rb.linearVelocity;

        // y速度は維持して、xzだけを狙う
        Vector3 velXZ = new Vector3(vel.x, 0, vel.z);
        Vector3 targetVelXZ = new Vector3(targetVel.x, 0, targetVel.z);

        Vector3 force = (targetVelXZ - velXZ) * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);
    }
}
