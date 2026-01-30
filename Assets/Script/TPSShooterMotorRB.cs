using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TPSShooterMotorRB : MonoBehaviour
{
    public Transform cam;
    public float moveSpeed = 6f;
    public float acceleration = 20f;
    public float turnSpeed = 18f;

    [Header("Aim settings")]
    public bool rotateAlways = true;
    public KeyCode aimKey = KeyCode.Mouse1;

    [Header("Jump")]
    public float jumpVelocity = 6.5f;              // 上向き速度（大きいほど高く跳ぶ）
    public float groundCheckExtra = 0.08f;         // 地面判定の余裕
    public LayerMask groundLayers = ~0;            // 地面レイヤー（できればGroundだけにする）
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Animation")]
    public Animator animator;
    public string runParam = "Run";
    public string jumpParam = "Jump";
    public float runThreshold = 0.1f;

    Rigidbody rb;
    CapsuleCollider capsule;
    Vector3 inputDir;
    Quaternion desiredRot;

    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();

        desiredRot = rb.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        // 走りアニメ
        if (animator != null)
        {
            bool isRun = inputDir.sqrMagnitude > runThreshold * runThreshold;
            animator.SetBool(runParam, isRun);
        }

        // ジャンプ入力（Updateで拾うのが安定）
        if (Input.GetKeyDown(jumpKey) && grounded)
        {
            // 落下中などでも安定して跳べるようにY速度をリセットしてから上向き速度を付与
            Vector3 v0 = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v0.x, 0f, v0.z);
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

            if (animator != null)
                animator.SetTrigger(jumpParam);
        }

        if (cam == null) return;

        bool aiming = rotateAlways || Input.GetKey(aimKey);

        if (aiming)
        {
            Vector3 f = cam.forward; f.y = 0f;
            if (f.sqrMagnitude < 0.0001f)
            {
                f = transform.forward; f.y = 0f;
            }
            if (f.sqrMagnitude > 0.0001f)
            {
                f.Normalize();
                desiredRot = Quaternion.LookRotation(f, Vector3.up);
            }
        }
        else if (inputDir.sqrMagnitude > 0.0001f)
        {
            Vector3 camF = cam.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cam.right;   camR.y = 0; camR.Normalize();
            Vector3 moveWorld = camR * inputDir.x + camF * inputDir.z;

            if (moveWorld.sqrMagnitude > 0.0001f)
                desiredRot = Quaternion.LookRotation(moveWorld.normalized, Vector3.up);
        }
    }

    void FixedUpdate()
    {
        grounded = CheckGrounded();

        if (cam == null) return;

        // 移動：カメラ基準（ストレイフ）
        Vector3 camF = cam.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cam.right;   camR.y = 0; camR.Normalize();
        Vector3 moveWorld = camR * inputDir.x + camF * inputDir.z;
        if (moveWorld.sqrMagnitude > 1f) moveWorld.Normalize();

        Vector3 targetVel = moveWorld * moveSpeed;
        Vector3 vel = rb.linearVelocity;
        Vector3 velXZ = new Vector3(vel.x, 0, vel.z);

        Vector3 force = (targetVel - velXZ) * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);

        Quaternion q = Quaternion.Slerp(rb.rotation, desiredRot, turnSpeed * Time.fixedDeltaTime);
        q = Quaternion.Normalize(q);
        rb.MoveRotation(q);
    }

    bool CheckGrounded()
    {
        if (capsule == null) return false;

        // カプセル下端付近からSphereCastで地面判定
        Vector3 center = transform.TransformPoint(capsule.center);

        // スケール込み半径（XZの大きい方）
        float scaleXZ = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        float radius = capsule.radius * scaleXZ * 0.95f;

        // カプセルの半分高さ（Yスケール込み）
        float halfHeight = (capsule.height * transform.lossyScale.y) * 0.5f;
        float castDist = (halfHeight - radius) + groundCheckExtra;

        return Physics.SphereCast(center, radius, Vector3.down, out _, castDist, groundLayers, QueryTriggerInteraction.Ignore);
    }
}
