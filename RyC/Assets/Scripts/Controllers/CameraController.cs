using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("Camera Settings")]
  [SerializeField] private Transform target;
  [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -3.5f);
  [SerializeField] private float smoothSpeed = 1.5f;
  [SerializeField] private float rotationSpeed = 2f;

  private PlayerIndex playerIndex;
  public Camera cam;
  
  private void LateUpdate()
  {
    if (target == null) return;

    Vector3 desiredPosition = target.position + target.TransformDirection(offset);
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

    transform.position = smoothedPosition;

    Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
  }

  private void Start()
  {
    cam = GetComponent<Camera>();
    if (target != null)
    {
      var carCtrl = target.GetComponent<CarController>();
      if (carCtrl != null)
      {
        playerIndex = carCtrl.GetPlayerIndex();
        SetViewport();
      }
    }
  }

  private void SetViewport()
  {
    // Lee modo de PlayerPrefs (set por menú)
    bool isSinglePlayer = PlayerPrefs.GetInt("GameMode", (int)GameMode.SinglePlayer) == (int)GameMode.SinglePlayer;
  
    if (isSinglePlayer)
    {
      cam.rect = new Rect(0f, 0f, 1f, 1f);  // Full screen para single (ignora playerIndex)
    }
    else if (playerIndex == PlayerIndex.One)
    {
      cam.rect = new Rect(0f, 0f, 0.5f, 1f);  // Izquierda para P1 en multi
    }
    else  // PlayerIndex.Two en multi
    {
      cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);  // Derecha para P2 en multi
    }
  
    // Debug temporal (borra después)
    Debug.Log($"Viewport para {playerIndex} en modo {(isSinglePlayer ? "Single" : "Multi")}: {cam.rect}");
  }

  public void SetTarget(Transform newTarget)
  {
    target = newTarget;
  }
}