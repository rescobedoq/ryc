using UnityEngine;
using System.Collections;

public enum PlayerIndex { One, Two }

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] public PlayerIndex playerIndex = PlayerIndex.One;

    [Header("Component References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private Transform[] wheelMeshes;

    [Header("Movement Settings")]
    public float BaseSpeed = 50f;
    public float BaseAcceleration = 20000f;
    public float SteeringForce = 30f;
    public float BrakeForce = 500f;
    public float MaxSpeed = 220f;

    [Header("Physics Settings")]
    [SerializeField] private float downForce = 50f;
    [SerializeField] private float normalDrag = 0.00005f;
    [SerializeField] private float boostDrag = 0.03f;
    [SerializeField] private float slowDrag = 0.5f;

    private IVehicleState currentState;
    private bool controlsEnabled = true;

    // ========= QUIZ / PREGUNTAS EN HUD =========
    private string currentQuestionText;

    // Propiedades para los estados
    public Rigidbody VehicleRigidbody => rb;
    public WheelCollider[] WheelColliders => wheelColliders;
    public float CurrentSpeed => rb.velocity.magnitude;

    public (float vertical, float horizontal, bool brake) GetPlayerInput()
    {
        string verticalAxis = playerIndex == PlayerIndex.One ? "VerticalP1" : "VerticalP2";
        string horizontalAxis = playerIndex == PlayerIndex.One ? "HorizontalP1" : "HorizontalP2";

        float verticalInput = Input.GetAxis(verticalAxis);
        float horizontalInput = Input.GetAxis(horizontalAxis);
        bool isBraking = playerIndex == PlayerIndex.One ?
          Input.GetKey(KeyCode.Space) : Input.GetKey(KeyCode.Return);

        return (verticalInput, horizontalInput, isBraking);
    }

    public void SetPlayerIndex(PlayerIndex index)
    {
        playerIndex = index;
    }

    public PlayerIndex GetPlayerIndex() => playerIndex;

    private void Start()
    {
        InitializeComponents();
        InitializeState();
    }

    private void InitializeComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (centerOfMass != null) rb.centerOfMass = centerOfMass.localPosition;

        ApplyNormalPhysics();
    }

    private void InitializeState()
    {
        currentState = new NormalState();
        currentState.EnterState(this);
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            currentState.HandleInput();
        }

        currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
        UpdateWheelMeshes();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(collision);
    }

    // Métodos públicos para cambiar de estado
    public void ChangeState(IVehicleState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    public void ApplyBoost()
    {
        ChangeState(new BoostedState());
    }

    public void ApplyPenalty()
    {
        ChangeState(new SlowState());
    }

    public void ApplyNormalPhysics()
    {
        rb.drag = normalDrag;
        rb.angularDrag = 0.05f;
        ResetWheelFriction();
    }

    public void ApplyBoostPhysics()
    {
        rb.drag = boostDrag;
        rb.angularDrag = 1f;
    }

    public void ApplySlowPhysics()
    {
        rb.drag = slowDrag;
        rb.angularDrag = 3f;
    }

    public void ApplyAcceleration(float input)
    {
        float force = input * BaseAcceleration;

        // Velocidad SOLO en la dirección forward del coche
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        if (forwardSpeed < MaxSpeed)
        {
            // Asume que las ruedas traseras son los índices 2 y 3
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                if (i == 2 || i == 3)
                    wheelColliders[i].motorTorque = force;
                else
                    wheelColliders[i].motorTorque = 0;
            }
        }
    }

    public void ApplySteering(float input)
    {
        float steering = input * SteeringForce;

        // Asume que las ruedas delanteras son los índices 0 y 1
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i == 0 || i == 1)
                wheelColliders[i].steerAngle = steering;
            else
                wheelColliders[i].steerAngle = 0;
        }
    }

    public void ApplyBraking()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.brakeTorque = BrakeForce;
        }
    }

    public void ApplyVehicleStabilization()
    {
        rb.AddForce(-transform.up * downForce * CurrentSpeed);

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 2f));
    }

    public void HandleCollision(Collision collision)
    {
        rb.velocity *= 0.7f;
        Debug.Log($"Colisión detectada. Velocidad reducida: {CurrentSpeed:F1}");
    }

    private void ResetWheelFriction()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 0;
            wheel.brakeTorque = 0;
        }
    }

    private void UpdateWheelPose(WheelCollider collider, Transform mesh, bool allowFullRotation)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        mesh.position = pos;

        if (allowFullRotation)
        {
            mesh.rotation = quat;
        }
    }

    private void UpdateWheelMeshes()
    {
        // Asume que el orden es: 0=FL, 1=FR, 2=RL, 3=RR
        if (wheelMeshes.Length == 4 && wheelColliders.Length == 4)
        {
            UpdateWheelPose(wheelColliders[0], wheelMeshes[0], true);  // FL
            UpdateWheelPose(wheelColliders[1], wheelMeshes[1], true);  // FR
            UpdateWheelPose(wheelColliders[2], wheelMeshes[2], false); // RL
            UpdateWheelPose(wheelColliders[3], wheelMeshes[3], false); // RR
        }
    }

    public void EnableControls() => controlsEnabled = true;
    public void DisableControls() => controlsEnabled = false;

    // ======== MÉTODOS PARA HUD DE PREGUNTAS ========
    public void SetQuestion(string text)
    {
        currentQuestionText = text;
    }

    public void ClearQuestion()
    {
        currentQuestionText = null;
    }

    // Debug GUI
    private void OnGUI()
    {
        Vector2 offset = playerIndex == PlayerIndex.One ?
          new Vector2(10, Screen.height * 0.75f) : new Vector2(10, 10);  // P1 abajo-izq, P2 arriba-izq

        GUI.Label(new Rect(offset.x, offset.y, 300, 20), $"P{playerIndex}: {currentState.GetType().Name}");
        GUI.Label(new Rect(offset.x, offset.y + 20, 300, 20), $"Vel: {CurrentSpeed:F1}");
        GUI.Label(new Rect(offset.x, offset.y + 40, 300, 20), $"Ctrls: {(controlsEnabled ? "ON" : "OFF")}");

        // Pregunta del portal
        if (!string.IsNullOrEmpty(currentQuestionText))
        {
            GUI.Label(new Rect(offset.x, offset.y + 60, 600, 20), currentQuestionText);
        }
    }
}
