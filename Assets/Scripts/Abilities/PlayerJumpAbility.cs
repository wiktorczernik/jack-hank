using UnityEngine;

public class PlayerJumpAbility : PlayerVehicleAbility
{
    [Header("State")]
    [SerializeField] float speedBeforeJump;
    [SerializeField] float startPitch;

    [Header("Jump settings")]
    [SerializeField] AnimationCurve pitchCurve;
    [SerializeField] float upwardSpeed = 30f;
    [SerializeField] float minForwardSpeed = 50f;

    [Header("Torque settings")]
    [SerializeField] float almostGroundedTorqueForce = 10f;
    [SerializeField] float pitchTorqueForce = 12f;
    [SerializeField] float rollStabilizationForce = 40f;
    [SerializeField] float pitchDampingFactor = 1f;
    [SerializeField] float rollDampingFactor = 1f;

    public override bool ContinueWorkWhile()
    {
        return !physics.isGrounded;
    }

    protected override void OnWorkBegin()
    {
        physics.onLand += OnLand;

        Rigidbody useRigidbody = physics.bodyRigidbody;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(useRigidbody.linearVelocity, physics.transform.up);
        speedBeforeJump = Mathf.Max(minForwardSpeed, horizontalVelocity.magnitude);

        Vector3 jumpVelocity = vehicle.transform.forward * speedBeforeJump
                              + vehicle.transform.up * upwardSpeed;
        // Сброс угловой скорости по X и Z
        Vector3 angular = useRigidbody.angularVelocity;
        angular.x = 0f;
        angular.z = 0f;
        useRigidbody.angularVelocity = angular;

        // Фиксация начального тангажа для кривой
        Vector3 currentEuler = vehicle.transform.eulerAngles;
        startPitch = NormalizeAngle(currentEuler.x);
        // Обновляем первый ключ кривой (необязательно, но можно подстроить)
        Keyframe firstKey = pitchCurve.keys[0];
        firstKey.value = startPitch;
        pitchCurve.MoveKey(0, firstKey);

        useRigidbody.linearVelocity = jumpVelocity;
    }

    protected override void OnWorkTick()
    {
        if (physics.isGrounded) return;

        float desiredPitch = pitchCurve.Evaluate(workTime);

        // Текущие мировые углы в локальных осях
        Vector3 euler = vehicle.transform.eulerAngles;
        float currentPitch = NormalizeAngle(euler.x);
        float currentRoll = NormalizeAngle(euler.z);

        // Ошибки (целевой крен = 0)
        float pitchError = desiredPitch - currentPitch;
        float rollError = 0f - currentRoll;

        // Локальная угловая скорость
        Vector3 localAngVel = vehicle.transform.InverseTransformDirection(
            physics.bodyRigidbody.angularVelocity
        );

        // Формируем локальный момент: по X — для тангажа, по Z — для крена
        float torquePitch = pitchError * pitchTorqueForce
                            - localAngVel.x * pitchDampingFactor;
        float torqueRoll = rollError * rollStabilizationForce
                            - localAngVel.z * rollDampingFactor;

        Vector3 localTorque = new Vector3(torquePitch, 0f, torqueRoll);
        // Переводим в мировой и применяем с учетом фиксированного шага
        Vector3 worldTorque = vehicle.transform.TransformDirection(localTorque)
                              * Time.fixedDeltaTime;
        physics.bodyRigidbody.AddTorque(worldTorque, ForceMode.VelocityChange);
    }

    public override bool UsageConditionsSatisfied()
    {
        return physics.isGrounded;
    }

    private void OnLand(VehiclePhysics.AirTimeState airTime)
    {
        if (workTime < workDuration) return;

        Rigidbody rb = physics.bodyRigidbody;
        rb.linearVelocity = vehicle.transform.forward * speedBeforeJump;
        rb.angularVelocity = Vector3.zero;

        physics.onLand -= OnLand;
    }

    // Приводит угол из [0,360) к диапазону [–180, +180]
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
