using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 5;
    public float invincibleTime = 0.8f;

    public int CurrentHp { get; private set; }

    float invUntil;

    public System.Action<int, int> OnHpChanged; // (cur, max)
    public System.Action OnDied;

    void Start()
    {
        CurrentHp = maxHp;
        OnHpChanged?.Invoke(CurrentHp, maxHp);
    }

    public bool CanTakeDamage() => Time.time >= invUntil;

    public void TakeDamage(int amount)
    {

        if (!CanTakeDamage()) return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        Debug.Log($"HP: {CurrentHp}/{maxHp}");
        invUntil = Time.time + invincibleTime;

        OnHpChanged?.Invoke(CurrentHp, maxHp);
        if (CurrentHp <= 0) OnDied?.Invoke();
    }
}
