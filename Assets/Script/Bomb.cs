using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Damage/Knockback on hit")]
    public int damage = 1;
    public float knockback = 10f;
    public float upward = 2f;
    public float hitCooldown = 0.2f;

    [Header("Auto explode")]
    public bool autoExplode = true;
    public float fuseSeconds = 3f;          // スポーン後3秒で爆発
    public bool explodeOnHit = false;       // 接触した瞬間に爆発させたいなら true
    public bool destroyOnExplode = true;    // 爆発後に消す（falseなら残る）

    [Header("Explosion (optional AoE)")]
    public float explosionRadius = 0f;      // 0なら「爆発=消える+SEだけ」。範囲ダメージしたいなら >0
    public LayerMask explosionMask = ~0;    // 影響対象レイヤー

    [Header("SE")]
    public AudioClip sfx;                 // 爆発もヒットもこれ1つ
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    public bool playOnHit = true;
    public bool playOnExplode = true;

    float nextHitTime;
    bool exploded;

    void OnEnable()
    {
        exploded = false;
        nextHitTime = 0f;

        if (autoExplode)
        {
            CancelInvoke(nameof(Explode));
            Invoke(nameof(Explode), fuseSeconds);
        }
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Explode));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;
        if (Time.time < nextHitTime) return;

        var health = collision.collider.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        nextHitTime = Time.time + hitCooldown;

        // ヒット時SE（ただし「当たった瞬間爆発」の時は二重になりやすいので抑制）
        if (playOnHit && !explodeOnHit)
            PlaySfx();


        // ダメージ
        health.TakeDamage(damage);

        // ノックバック
        var prb = collision.collider.GetComponentInParent<Rigidbody>();
        if (prb != null)
        {
            Vector3 dir = (prb.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;
            dir.Normalize();

#if UNITY_6000_0_OR_NEWER
            Vector3 v = prb.linearVelocity;
            prb.linearVelocity = new Vector3(v.x, Mathf.Min(v.y, 0f), v.z);
#else
            Vector3 v = prb.velocity;
            prb.velocity = new Vector3(v.x, Mathf.Min(v.y, 0f), v.z);
#endif

            prb.AddForce(dir * knockback + Vector3.up * upward, ForceMode.VelocityChange);
        }

        if (explodeOnHit)
            Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        CancelInvoke(nameof(Explode));

        if (playOnExplode)
            PlaySfx();


        // 範囲効果を入れたい場合（任意）
        if (explosionRadius > 0f)
        {
            var cols = Physics.OverlapSphere(transform.position, explosionRadius, explosionMask,
                                             QueryTriggerInteraction.Ignore);

            foreach (var col in cols)
            {
                var health = col.GetComponentInParent<PlayerHealth>();
                if (health != null)
                    health.TakeDamage(damage);

                var rb = col.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (rb.position - transform.position);
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.0001f)
                    {
                        dir.Normalize();
                        rb.AddForce(dir * knockback + Vector3.up * upward, ForceMode.VelocityChange);
                    }
                }
            }
        }

        if (destroyOnExplode)
            Destroy(gameObject);
    }

    void PlaySfx()
    {
        if (sfx == null) return;
        AudioSource.PlayClipAtPoint(sfx, transform.position, sfxVolume);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (explosionRadius > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
#endif
}
