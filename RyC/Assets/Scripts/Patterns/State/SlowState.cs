using UnityEngine;

public class SlowState : IVehicleState
{
  private CarController carController;
  private float slowTimer;
  private float slowDuration = 2f;
  private float slowMultiplier = 0.85f;

  public void EnterState(CarController controller)
  {
    carController = controller;
    slowTimer = 0f;

    carController.ApplySlowPhysics();
    Debug.Log("Estado: Slow - Penalización por respuesta incorrecta");
  }

  public void UpdateState()
  {
    slowTimer += Time.deltaTime;

    if (slowTimer >= slowDuration)
    {
      carController.ChangeState(new NormalState());
    }
  }

  public void FixedUpdateState()
  {
    carController.ApplyVehicleStabilization();
  }

  public void ExitState()
  {
    carController.ApplyNormalPhysics();
    Debug.Log("Penalización terminada");
  }

  public void HandleInput()
  {
    var (verticalInput, horizontalInput, isBraking) = carController.GetPlayerInput();
    
    float multiplier = slowMultiplier;
    
    carController.ApplyAcceleration(verticalInput * multiplier);
    carController.ApplySteering(horizontalInput);
    
    if (isBraking)
    {
      carController.ApplyBraking();
    }
  }

  public void OnCollisionEnter(Collision collision)
  {
    carController.HandleCollision(collision);
  }
}