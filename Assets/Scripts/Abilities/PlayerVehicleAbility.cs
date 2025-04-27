using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base ability class for player's vehicle
/// </summary>
public class PlayerVehicleAbility : MonoBehaviour
{
    public string displayName => _displayName;
    /// <summary>
    /// Required amount of mana points to use this ability
    /// </summary>
    public int requiredMana => _requiredMana;

    public AbilityState state => _state;
    /// <summary>
    /// Tells if ability not ready for use
    /// </summary>
    public bool isBusy => !isReady;
    /// <summary>
    /// Tells if ability is ready for use
    /// </summary>
    public bool isReady => state == AbilityState.Ready && UsageConditionsSatisfied();
    /// <summary>
    /// Tells if ability is in preparation stage (before doing some effect)
    /// </summary>
    public bool isPreparing => state == AbilityState.Prepare;
    /// <summary>
    /// Tells if ability is during working stage (showing some effect)
    /// </summary>
    public bool isWorking => state == AbilityState.Work;
    /// <summary>
    /// tells if ability is cooldowned
    /// </summary>
    public bool isCooldowned => state == AbilityState.Cooldown;

    /// <summary>
    /// Determines how long does it take for ability to prepare for work stage
    /// </summary>
    public float prepareDuration => _prepareDuration;
    /// <summary>
    /// Tells how many time already passed during prepare stage
    /// </summary>
    public float prepareTime => _prepareTime;
    /// <summary>
    /// Tells what fraction of preparation stage duration already passed
    /// </summary>
    public float prepareFraction => prepareTime / prepareDuration;

    /// <summary>
    /// Determines how long does ability work
    /// </summary>
    public float workDuration => _workDuration;
    /// <summary>
    /// Tells how many time already passed during work stage
    /// </summary>
    public float workTime => _workTime;
    /// <summary>
    /// Tells what fraction of work stage duration already passed
    /// </summary>
    public float workFraction => workTime / workDuration;

    /// <summary>
    /// Determines how long will cooldown take
    /// </summary>
    public float cooldownDuration => _cooldownDuration;
    /// <summary>
    /// Tells how many time already passed during prepare stage
    /// </summary>
    public float cooldownTime => _cooldownTime;
    /// <summary>
    /// Tells what fraction of cooldown stage duration already passed
    /// </summary>
    public float cooldownFraction => cooldownTime / cooldownDuration;

    /// <summary>
    /// Called at the beginning of preparation stage
    /// </summary>
    public UnityEvent onPrepareBegin;
    /// <summary>
    /// Called every tick of preparation stage
    /// </summary>
    public UnityEvent onPrepareTick;
    /// <summary>
    /// Called at the end of preparation stage
    /// </summary>
    public UnityEvent onPrepareEnd;

    /// <summary>
    /// Called at the beginning of working stage
    /// </summary>
    public UnityEvent onWorkBegin;
    /// <summary>
    /// called every tick of working stage
    /// </summary>
    public UnityEvent onWorkTick;
    /// <summary>
    /// Called at the end of working stage
    /// </summary>
    public UnityEvent onWorkEnd;

    /// <summary>
    /// Called at the start of cooldown
    /// </summary>
    public UnityEvent onCooldownBegin;
    /// <summary>
    /// Called every tick of cooldown
    /// </summary>
    public UnityEvent onCooldownTick;
    /// <summary>
    /// Called at the end of cooldown
    /// </summary>
    public UnityEvent onCooldownEnd;

    /// <summary>
    /// Called whenether state of ability changes. Arguments are: previous state, new state
    /// </summary>
    public UnityEvent<AbilityState, AbilityState> onStateUpdate;


    public bool Use()
    {
        if (isReady)
        {
            ForceUse();
            return true;
        }
        return false;
    }
    public void ForceUse()
    {
        _prepareTime = 0;
        _workTime = 0;
        _cooldownTime = 0;
        var oldState = _state;
        _state = AbilityState.Prepare;
        onStateUpdate?.Invoke(oldState, _state);
    }
    public void Init(PlayerVehicle p)
    {
        vehicle = p;
    }

    /// <summary>
    /// Were conditions defined for this ability satisfied
    /// </summary>
    public virtual bool UsageConditionsSatisfied() => true;
    public virtual bool ContinueWorkWhile() => false;
    /// <summary>
    /// Called at the beginning of preparation stage
    /// </summary>
    protected virtual void OnPrepareBegin() { }
    /// <summary>
    /// Called every tick of preparation stage
    /// </summary>
    protected virtual void OnPrepareTick() { }
    /// <summary>
    /// Called at the end of preparation stage
    /// </summary>
    protected virtual void OnPrepareEnd() { }
    /// <summary>
    /// Called at the beginning of working stage
    /// </summary>
    protected virtual void OnWorkBegin() { }
    /// <summary>
    /// called every tick of working stage
    /// </summary>
    protected virtual void OnWorkTick() { }
    /// <summary>
    /// Called at the end of working stage
    /// </summary>
    protected virtual void OnWorkEnd() { }
    /// <summary>
    /// Called at the start of cooldown
    /// </summary>
    protected virtual void OnCooldownBegin() { }
    /// <summary>
    /// Called every tick of cooldown
    /// </summary>
    protected virtual void OnCooldownTick() { }
    /// <summary>
    /// Called at the end of cooldown
    /// </summary>
    protected virtual void OnCooldownEnd() { }
    /// <summary>
    /// Called when ability state changed
    /// </summary>
    protected virtual void OnStateChange(AbilityState previousState, AbilityState newState) { }

    // TODO: Those methods are very similar, maybe create generic one?
    void PrepareSeq()
    {
        if (_prepareTime > float.Epsilon)
        {
            if (_prepareTime < _prepareDuration) // stay
            {
                OnPrepareTick();
                onPrepareTick?.Invoke();
            }
            else // end
            {
                _prepareTime = 0;
                var oldState = _state;
                _state = AbilityState.Work;
                onStateUpdate?.Invoke(oldState, _state);
                OnPrepareEnd();
                onPrepareEnd?.Invoke();
            }
        }
        else // begin
        {
            OnPrepareBegin();
            onPrepareBegin?.Invoke();
        }
        _prepareTime += Time.fixedDeltaTime;
    }
    void WorkSeq()
    {
        if (_workTime > float.Epsilon)
        {
            if (_workTime < _workDuration || ContinueWorkWhile()) // stay
            {
                OnWorkTick();
                onWorkTick?.Invoke();
            }
            else // end
            {
                _workTime = 0;
                var oldState = _state;
                _state = AbilityState.Cooldown;
                onStateUpdate?.Invoke(oldState, _state);
                OnWorkEnd();
                onWorkEnd?.Invoke();
            }
        }
        else // begin
        {
            OnWorkBegin();
            onWorkBegin?.Invoke();
        }
        _workTime += Time.fixedDeltaTime;
    }
    void CooldownSeq()
    {
        if (_cooldownTime > float.Epsilon)
        {
            if (_cooldownTime < _cooldownDuration) // stay
            {
                OnCooldownTick();
                onCooldownTick?.Invoke();
            }
            else // end
            {
                _cooldownTime = 0;
                var oldState = _state;
                _state = AbilityState.Ready;
                onStateUpdate?.Invoke(oldState, _state);
                OnCooldownEnd();
                onCooldownEnd?.Invoke();
            }
        }
        else // begin
        {
            OnCooldownBegin();
            onCooldownBegin?.Invoke();
        }
        _cooldownTime += Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case AbilityState.Prepare:
                PrepareSeq();
                break;
            case AbilityState.Work:
                WorkSeq();
                break;
            case AbilityState.Cooldown:
                CooldownSeq();
                break;
        }
    }

    /// <summary>
    /// Reference to player vehicle
    /// </summary>
    protected PlayerVehicle vehicle { get; private set; }
    /// <summary>
    /// Reference to player's vehicle physics
    /// </summary>
    protected VehiclePhysics physics => vehicle.physics;

    [Header("General ability state")]
    /// <summary>
    /// Required mana points to use this ability
    /// </summary>
    [SerializeField] int _requiredMana = 100;
    /// <summary>
    /// Tells how many time already passed during preparation stage
    /// </summary>
    [SerializeField] float _prepareTime = 0f;
    /// <summary>
    /// Tells how many time already passed during work stage
    /// </summary>
    [SerializeField] float _workTime = 0f;
    /// <summary>
    /// Tells how many time already passed during prepare stage
    /// </summary>
    [SerializeField] float _cooldownTime = 0f;
    /// <summary>
    /// Current state of the ability
    /// </summary>
    [SerializeField] AbilityState _state;

    [Header("General ability settings")]
    [SerializeField] string _displayName = "Invalid Ability";
    /// <summary>
    /// Determines how long will preparation stage take
    /// </summary>
    [SerializeField] float _prepareDuration = 1f;
    /// <summary>
    /// Determines how long will working stage take
    /// </summary>
    [SerializeField] float _workDuration = 1f;
    /// <summary>
    /// Determines how long will cooldown take
    /// </summary>
    [SerializeField] float _cooldownDuration = 1f;

    /// <summary>
    /// Describes state of the ability
    /// </summary>
    public enum AbilityState
    {
        /// <summary>
        /// When ability is ready for use
        /// </summary>
        Ready,
        /// <summary>
        /// When ability is being prepared for work (before taking effect)
        /// </summary>
        Prepare,
        /// <summary>
        /// When ability is working (showing some effect)
        /// </summary>
        Work,
        /// <summary>
        /// When ability is cooldowned (resting)
        /// </summary>
        Cooldown
    }
}