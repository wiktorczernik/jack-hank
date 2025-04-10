using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    /// <summary>
    /// Health of this entity
    /// </summary>
    public float health
    {
        get => _health;
        set
        {
            SetHealth(value);
        }
    }
    /// <summary>
    /// Maximum health for this entity
    /// </summary>
    public float maxHealth
    {
        get => _maxHealth;
        set
        {
            SetMaxHealth(value);
        }
    }

    /// <summary>
    /// Determines if this entity can ever receive any damage
    /// </summary>
    public bool canReceiveDamage = true;
    /// <summary>
    /// Determines if this entity can ever be healed
    /// </summary>
    public bool canBeHealed = true;

    /// <summary>
    /// Tells is this entity alive
    /// </summary>
    public bool isAlive => _health >= float.Epsilon;
    /// <summary>
    /// True if health equals maximum health, otherwise false
    /// </summary>
    public bool isFullHealth => _health >= _maxHealth;
    
    [Header("Health")]
    /// <summary>
    /// Health of this entity
    /// </summary>
    [SerializeField] float _health = 100f;
    /// <summary>
    /// Maximum health for this entity
    /// </summary>
    [SerializeField] float _maxHealth = 100f;

    [Header("Explosion State")]
    /// <summary>
    /// Tells if this entity was exploded, no matter if it was
    /// exploded by some other entity or on its own.
    /// </summary>
    public bool exploded = false;

    [Header("Explosion Settings")]
    /// <summary>
    /// Determines if this entity can be exploded by other entity
    /// </summary>
    public bool canBeExploded = false;
    /// <summary>
    /// Determines if this entity can produce explosion
    /// </summary>
    public bool canSelfExplode = false;
    /// <summary>
    /// Properties of produced explosion by this entity.
    /// Modify only if this entity can self explode
    /// </summary>
    public ExplosionProperties selfExplosionProps = new ExplosionProperties()
    {
        force = 500f,
        epicenterRadius = 10f,
        shakeMaxDistance = 100f,
        shakeIntensity = 0.5f
    };

    /// <summary>
    /// Called after entity's health was changed.
    /// First value is old health, second is a new one.
    /// </summary>
    public event Action<float, float> onHealthChange;
    /// <summary>
    /// Called after entity was healed.
    /// Amount of health points it received is a passed value
    /// </summary>
    public event Action<float> onHeal;
    /// <summary>
    /// Called after entity was hurt, but haven't died.
    /// Amount of health points was taken is a passed value
    /// </summary>
    public event Action<float> onHurt;
    /// <summary>
    /// Called when entity achieves maximum health.
    /// </summary>
    public event Action onFullHealth;
    /// <summary>
    /// Called when entity was revived.
    /// </summary>
    public event Action onRevive;
    /// <summary>
    /// Called after entity died.
    /// </summary>
    public event Action onDeath;

    public event Action<ExplosionProperties> onSelfExplode;
    public event Action<ExplosionProperties> onExplode;


    /// <summary>
    /// Set world position of this entity
    /// </summary>
    public virtual void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }
    /// <summary>
    /// Gets world position of this entity
    /// </summary>
    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Sets health to specific number.
    /// If it didn't change, no events or actions will be taken.
    /// If health was increased, it will call OnHeal and OnFullHealth 
    /// in case it was healed to the maximum.
    /// If health was decreased, it wall call OnHurt if it didn't die or
    /// OnDeath if it died.
    /// If it was dead and health was increased, it will be revived with
    /// given newHealth as starting health
    /// Will always call OnHealthChange if health changed
    /// </summary>
    /// <param name="newHealth"></param>
    public bool SetHealth(float newHealth)
    {
        if (_health != newHealth)
        {
            if (!isAlive)
            {
                return Revive(newHealth);
            }
            else
            {
                if (newHealth > _health)
                {
                    return ForceHeal(newHealth - _health);
                }
                else
                {
                    return ForceHurt(_health - newHealth);
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Sets maximum health for this entity
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        // TODO: Pomyslec czy zostawic jak jest czy rozwinac
        _maxHealth = newMaxHealth;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }
    /// Reduces entity's health if it can receive damage.
    /// Invokes OnHurt if entity didn't die after being hurt, and OnDeath if it died.
    /// Always invokes OnHealthChange if health changed
    /// </summary>
    /// <param name="amount">How many damage will entity take</param>
    /// <returns>True if health changed, otherwise false</returns>
    public bool Hurt(float amount)
    {
        if (canReceiveDamage)
        {
            return ForceHurt(amount);
        }
        return false;
    }
    /// <summary>
    /// Heals entity if it can be healed.
    /// Invokes OnHeal and OnHealthChange events if health changes.
    /// Additionally invokes OnFullHealth if it was healed to the maximum health.
    /// </summary>
    /// <param name="amount">How many HP will entity receive</param>
    /// <returns>True if health changed, otherwise false</returns>
    public bool Heal(float amount)
    {
        if (canBeHealed)
        {
            return ForceHeal(amount);
        }
        return false;
    }
    /// <summary>
    /// Forcefully reduces entity's health, no matter if it can receive damage or not.
    /// Invokes OnHurt if entity didn't die after being hurt, and OnDeath if it died.
    /// Always invokes OnHealthChange if health changed
    /// </summary>
    /// <param name="amount">How many damage will entity take</param>
    /// <returns>True if health changed, otherwise false</returns>
    public bool ForceHurt(float amount)
    {
        if (!isAlive)
            return false;

        float oldHealth = _health;
        float newHealth = _health - amount;
        bool metDeath = newHealth <= float.Epsilon;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);

        if (newHealth < oldHealth)
        {
            _health = newHealth;
            if (metDeath)
            {
                OnDeathInternal();
                onDeath?.Invoke();
            }
            else
            {
                OnHurtInternal(amount);
                onHurt?.Invoke(amount);
            }
            OnHealthChangeInternal(oldHealth, newHealth);
            onHealthChange?.Invoke(oldHealth, newHealth);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Forcefully heals entity, no matter if it can be healed or not.
    /// Invokes OnHeal and OnHealthChange events if health changes.
    /// Additionally invokes OnFullHealth if it was healed to the maximum health.
    /// </summary>
    /// <param name="amount">How many HP will entity receive</param>
    /// <returns>True if health changed, otherwise false</returns>
    public bool ForceHeal(float amount)
    {
        if (!isAlive)
            return false;

        float oldHealth = _health;
        float newHealth = _health + amount;

        bool achievedMax = newHealth >= _maxHealth;
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);

        if (newHealth > oldHealth)
        {
            _health = newHealth;
            OnHealInternal(amount);
            onHeal?.Invoke(amount);
            if (achievedMax)
            {
                OnFullHealthInternal();
                onFullHealth?.Invoke();
            }
            OnHealthChangeInternal(oldHealth, newHealth);
            onHealthChange?.Invoke(oldHealth, newHealth);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Heals entity to its maximum health. Invokes OnHeal, OnFullHealth if healed to max
    /// and OnHealthChange events
    /// </summary>
    public bool HealMax()
    {
        if (!isFullHealth && isAlive)
        {
            float oldHealth = _health;
            _health = _maxHealth;
            OnHealInternal(_maxHealth - oldHealth);
            onHeal?.Invoke(_maxHealth - oldHealth);
            OnFullHealthInternal();
            onFullHealth?.Invoke();
            OnHealthChangeInternal(oldHealth, _maxHealth);
            onHealthChange?.Invoke(oldHealth, _maxHealth);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Kills entity if it's alive. Invokes OnDeath and OnHealthChange events if killed
    /// </summary>
    public bool Kill()
    {
        if (isAlive)
        {
            float oldHealth = _health;
            _health = 0;
            OnDeathInternal();
            onDeath?.Invoke();
            OnHealthChangeInternal(oldHealth, 0);
            onHealthChange?.Invoke(oldHealth, 0);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Revives entity if it's dead. Invokes OnRevive and OnHealthChange events if revived
    /// </summary>
    /// <param name="startHealth">How many HP will entity have after revival</param>
    public bool Revive(float startHealth = -1)
    {
        if (!isAlive)
        {
            if (startHealth > _maxHealth || startHealth < float.Epsilon)
            {
                startHealth = _maxHealth;
            }
            _health = startHealth;
            OnReviveInternal();
            onRevive?.Invoke();
            OnHealthChangeInternal(0, _health);
            onHealthChange?.Invoke(0, _health);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Called after entity's health was changed.
    /// </summary>
    /// <param name="oldHealth">HP before change</param>
    /// <param name="newHealth">HP after change</param>
    protected virtual void OnHealthChangeInternal(float oldHealth, float newHealth) { }
    /// <summary>
    /// Called after entity was healed.
    /// </summary>
    /// <param name="amount">Amount of HP entity received</param>
    protected virtual void OnHealInternal(float amount) { }
    /// <summary>
    /// Called after entity was hurt, but haven't died.
    /// </summary>
    /// <param name="amount">Amount of HP that was taken</param>
    protected virtual void OnHurtInternal(float amount) { }
    /// <summary>
    /// Called when entity achieves maximum health.
    /// </summary>
    protected virtual void OnFullHealthInternal() { }
    /// <summary>
    /// Called when entity was revived.
    /// </summary>
    protected virtual void OnReviveInternal() { }
    /// <summary>
    /// Called after entity died.
    /// </summary>
    protected virtual void OnDeathInternal() { }

    /// <summary>
    /// Produces explosion
    /// </summary>
    public void SelfExplode()
    {
        if (!canSelfExplode) return;
        ExplosionMaster.Create(selfExplosionProps);
        exploded = true;
        InternalSelfExplode();
        onSelfExplode?.Invoke(selfExplosionProps);
    }
    /// <summary>
    /// Tells entity that it is being exploded by other entity
    /// </summary>
    /// <param name="explosionProps">Explosion properties</param>
    public void Explode(ExplosionProperties explosionProps)
    {
        if (!canBeExploded) return;
        exploded = true;
        InternalExplode(explosionProps);
        onExplode?.Invoke(explosionProps);
    }

    /// <summary>
    /// Called after entity produced explosion
    /// </summary>
    protected virtual void InternalSelfExplode() { }
    /// <summary>
    /// Called after entity was exploded by other entity
    /// </summary>
    /// <param name="explosionProps">Explosion properties</param>
    protected virtual void InternalExplode(ExplosionProperties explosionProps) { }
}
