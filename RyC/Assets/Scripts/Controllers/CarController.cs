using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
  [Header("Component References")]
  [SerializeField] private Rigidbody rb;
  [SerializeField] private Transform centerOfMass;
  [SerializeField] private WheelCollider[] wheelColliders;
  [SerializeField] private Transform[] wheelMeshes;

  [Header("Movement Settings")]
  public float BaseSpeed = 50f;
  public float BaseAcceleration = 1000f;
  public float SteeringForce = 30f;
  public float BrakeForce = 500f;
  public float MaxSpeed = 60f;

  [Header("Physics Settings")]
  [SerializeField] private float downForce = 50f;
  [SerializeField] private float normalDrag = 0.3f;
  [SerializeField] private float boostDrag = 0.1f;
  [SerializeField] private float slowDrag = 0.5f;

  private IVehicleState currentState;
  private bool controlsEnabled = true;

  // Propiedades para los estados
  public Rigidbody VehicleRigidbody => rb;
  public WheelCollider[] WheelColliders => wheelColliders;
  public float CurrentSpeed => rb.velocity.magnitude;

  private void Start()
  {
    InitializeComponents();
    InitializeState();
  }

  private void InitializeComponents()
  {
    if (rb == null) rb = GetComponent<Rigidbody>();
    //if (centerOfMass != null) rb.centerOfMass = centerOfMass.localPosition;

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

  // Metodos publicos para cambiar de estado
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

    if (CurrentSpeed < MaxSpeed)
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

  public void AppvehicleStabilization()
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
    else
    {
      Vector3 euler = quat.eulerAngles;
      mesh.localRotation = Quaternion.Euler(euler.x, mesh.localRotation.eulerAngles.y, mesh.localRotation.eulerAngles.z);
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

  // Debug GUI
  private void OnGUI()
  {
    GUI.Label(new Rect(10, 10, 300, 20), $"Estado: {currentState.GetType().Name}");
    GUI.Label(new Rect(10, 30, 300, 20), $"Velocidad: {CurrentSpeed:F1}");
    GUI.Label(new Rect(10, 50, 300, 20), $"Controles: {(controlsEnabled ? "Activados" : "Desactivados")}");
  }
}