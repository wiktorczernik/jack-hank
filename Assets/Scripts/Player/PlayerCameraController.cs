using UnityEngine;

public class PlayerCameraController : PlayerModule
{
    // Referencja do kamery gracza
    public PlayerCamera playerCamera => parent.playerCamera;
    // Czułość ruchu myszy
    public Vector2 sensitivity = new Vector2(5f, 5f);
    // Krzywa reprezentująca zmianę w przyśpieszniu myszy w zalezności od długości delty ruchu myszy
    public AnimationCurve accelerationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public bool accelerate = true;


    public override void OnLateUpdate(float deltaTime)
    {
        Vector3 newPos = parent.GetPosition();
        newPos.y += parent.height;
        newPos.y -= parent.eyesHeight;

        playerCamera.SetPosition(newPos);

        Look();
    }
    void Look()
    {
        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (accelerate)
            {
            input *= accelerationCurve.Evaluate(input.magnitude);
        }
        input *= sensitivity;
            
        Vector3 viewAngles = playerCamera.viewAngles;
        viewAngles.y += input.x;
        viewAngles.x = Mathf.Clamp(viewAngles.x - input.y, -90f, 90f);

        playerCamera.SetViewAngles(viewAngles);
    }
}
