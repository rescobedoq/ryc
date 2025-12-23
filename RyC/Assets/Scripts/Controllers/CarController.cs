using UnityEngine;
using System.Collections;

public enum PlayerIndex { One, Two }

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour, IQuizObserver
{
  [Header("Player Settings")]
  [SerializeField] public PlayerIndex playerIndex = PlayerIndex.One;

  [Header("Component References")]
  [SerializeField] private Rigidbody rb;
  [SerializeField] private Transform centerOfMass;
  [SerializeField] private WheelCollider[] wheelColliders;
  [SerializeField] private Transform[] wheelMeshes;

  [Header("Movement Settings")]
  public float BaseSpeed = 1f;
  public float BaseAcceleration = 700f;
  public float SteeringForce = 15f;
  public float BrakeForce = 500f;
  public float MaxSpeed = 100f;

  [Header("Physics Settings")]
  [SerializeField] private float downForce = 70f;
  [SerializeField] private float normalDrag = 0.02f;
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

    // Suscribirse al nuevo QuizManager1
    if (QuizManager1.Instance != null)
      QuizManager1.Instance.AddObserver(this);
  }

  private void InitializeComponents()
  {
    if (rb == null) rb = GetComponent<Rigidbody>();
    if (centerOfMass != null) rb.centerOfMass = centerOfMass.localPosition;
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
    ApplyAutoFriction();
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

  private void ApplyAutoFriction()
  {
    // Obtenemos el input de ESTE auto específico (sea P1 o P2)
    var input = GetPlayerInput();

    // Si el input vertical es casi cero (no está acelerando ni retrocediendo)
    if (Mathf.Abs(input.vertical) < 0.1f)
    {
      // Aplicamos la fricción para detenerlo suavemente
      rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 2f);
      // Puedes subir el "2f" para frenar más rápido (recomendado 2–4)
    }
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
    rb.angularDrag = 2.0f;
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
    if (Mathf.Abs(input) < 0.01f)
    {
      for (int i = 2; i < wheelColliders.Length; i++)
      {
        wheelColliders[i].motorTorque = 0;
        wheelColliders[i].brakeTorque = 0; // Aseguramos soltar frenos
      }
      return;
    }

    float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);

    // Si vamos hacia adelante (> 1m/s) y el jugador presiona atrás (input < 0)
    if (forwardSpeed > 1f && input < -0.1f)
    {
      // El valor '1.5f' controla la fuerza del frenado. Súbelo a 2f o 3f si quieres que frene más rápido.
      rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 1.5f);

      // Aplicar frenos físicos a las ruedas en lugar de motor inverso
      foreach (var wheel in wheelColliders)
      {
        wheel.brakeTorque = BrakeForce * 1.2f;
        wheel.motorTorque = 0;
      }
      return; // Salimos aquí para no aplicar motorTorque hasta que el auto se detenga casi por completo
    }
    else
    {
      // Si no estamos frenando, aseguramos que los frenos estén libres
      foreach (var wheel in wheelColliders) wheel.brakeTorque = 0;
    }

    float force = input * BaseAcceleration;

    if (forwardSpeed < MaxSpeed)
    {
      for (int i = 2; i < wheelColliders.Length; i++)
        wheelColliders[i].motorTorque = force;
    }
  }

  public void ApplySteering(float input)
  {
    float steering = input * SteeringForce;

    // Aplicar ángulo físico a las ruedas
    for (int i = 0; i < wheelColliders.Length; i++)
    {
      if (i == 0 || i == 1)
        wheelColliders[i].steerAngle = steering;
      else
        wheelColliders[i].steerAngle = 0;
    }

    // Si el auto se mueve y estamos girando
    if (Mathf.Abs(input) > 0.1f && CurrentSpeed > 5f)
    {
      // Calculamos la dirección hacia donde mira el auto con la velocidad actual
      Vector3 forwardVelocity = transform.forward * CurrentSpeed;

      // Interpolamos suavemente la velocidad actual hacia la dirección frontal
      // El valor '10f' define qué tanto agarre tiene (más alto = menos derrape)
      rb.velocity = Vector3.Lerp(rb.velocity, forwardVelocity, Time.fixedDeltaTime * 10f);

      // Ayuda de giro (Torque) para que responda mejor
      float turnAssist = input * 3950f;
      rb.AddTorque(Vector3.up * turnAssist, ForceMode.Force);
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
    // Downforce (mantiene el auto pegado al piso)
    rb.AddForce(-transform.up * downForce * CurrentSpeed);

    // Calcular la rotación "ideal" (totalmente plana)
    Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

    // Evitamos errores si el auto está perfectamente vertical (raro, pero posible)
    if (flatForward.sqrMagnitude < 0.001f) return;

    Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);

    // Calcular cuánto se está inclinando el auto (Ángulo entre el techo del auto y el cielo)
    float tiltAngle = Vector3.Angle(transform.up, Vector3.up);

    // Si se inclina menos de 20 grados: Corrección suave (2f) -> Permite que se vea la suspensión trabajando.
    // Si se inclina más de 20 grados: Corrección fuerte (15f) -> Actúa como un tope para no chocar el suelo.
    float stabilizationSpeed = (tiltAngle > 20f) ? 15f : 2f;

    // Aplicamos la rotación
    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * stabilizationSpeed));
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
    if (collider == null || mesh == null)
    {
      Debug.LogError($"Null en Pose: Collider {collider?.name}, Mesh {mesh?.name}");
      return;
    }

    Vector3 pos;
    Quaternion quat;
    collider.GetWorldPose(out pos, out quat);

    mesh.position = pos;
    if (allowFullRotation)
    {
      mesh.rotation = quat;
    }

    Debug.Log($"Pose {mesh.name}: Pos {pos}, Rot {quat.eulerAngles}, AllowFull: {allowFullRotation}");  // TEMPORAL – Muestra si cambia al acelerar
  }

  // private void UpdateWheelPose(WheelCollider collider, Transform mesh, bool allowFullRotation)
  // {
  //   Vector3 pos;
  //   Quaternion quat;
  //   collider.GetWorldPose(out pos, out quat);
  //   mesh.position = pos;

  //   if (allowFullRotation)
  //   {
  //     mesh.rotation = quat;
  //   }
  // }

  private void UpdateWheelMeshes()
  {
    if (wheelColliders == null || wheelMeshes == null || wheelColliders.Length != 4 || wheelMeshes.Length != 4)
    {
      Debug.LogError("Arrays inválidos – Colliders: " + (wheelColliders?.Length ?? 0) + ", Meshes: " + (wheelMeshes?.Length ?? 0));
      return;
    }

    Debug.Log("UpdateWheelMeshes: Procesando 4 ruedas");  // TEMPORAL

    // Orden fijo: 0=FL (steer/true), 1=FR, 2=RL (torque/false), 3=RR
    UpdateWheelPose(wheelColliders[0], wheelMeshes[0], true);   // FL
    UpdateWheelPose(wheelColliders[1], wheelMeshes[1], true);   // FR
    UpdateWheelPose(wheelColliders[2], wheelMeshes[2], false);  // RL
    UpdateWheelPose(wheelColliders[3], wheelMeshes[3], false);  // RR
  }

  // private void UpdateWheelMeshes()
  // {
  //   // Asume que el orden es: 0=FL, 1=FR, 2=RL, 3=RR
  //   if (wheelMeshes.Length == 4 && wheelColliders.Length == 4)
  //   {
  //     UpdateWheelPose(wheelColliders[0], wheelMeshes[0], true);  // FL
  //     UpdateWheelPose(wheelColliders[1], wheelMeshes[1], true);  // FR
  //     UpdateWheelPose(wheelColliders[2], wheelMeshes[2], false); // RL
  //     UpdateWheelPose(wheelColliders[3], wheelMeshes[3], false); // RR
  //   }
  // }

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

  private void OnDestroy()
  {
    if (QuizManager1.Instance != null)
      QuizManager1.Instance.RemoveObserver(this);
  }

  // --- Métodos de IQuizObserver ---
  public void OnQuestionLoaded(string questionText, string[] answers, int portalId)
  {
    SetQuestion(questionText); // Muestra en HUD del auto
  }

  public void OnAnswerCorrect(PlayerIndex player, int portalId)
  {
    if (player == this.playerIndex)
    {
      ApplyBoost();
      ClearQuestion();
    }
  }

  // Actualizamos la firma agregando int portalId
  public void OnAnswerWrong(PlayerIndex player, int portalId)
  {
    if (player == this.playerIndex)
    {
      ApplyPenalty();
      ClearQuestion();
    }
  }

  public void OnQuizFinished()
  {
    ClearQuestion();
  }

  public void OnRaceFinished(PlayerIndex winner)
  {
    Debug.Log($"Juego terminado. Ganador: {winner}");
    DisableControls(); // Detener el auto
    ApplyBraking();    // Frenar
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
