using UnityEngine;

public class SlowState : IVehicleState
{
  private CarController carController;
  private float slowTimer;
  private float slowDuration = 2f;
  private float slowMultiplier = 0.5f;

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
    carController.AppvehicleStabilization();
  }

  public void ExitState()
  {
    carController.ApplyNormalPhysics();
    Debug.Log("Penalización terminada");
  }

  public void HandleInput()
  {
    float verticalInput = Input.GetAxis("Vertical");
    float horizontalInput = Input.GetAxis("Horizontal");

    carController.ApplyAcceleration(verticalInput * slowMultiplier);
    carController.ApplySteering(horizontalInput);

    if (Input.GetKey(KeyCode.Space))
    {
      carController.ApplyBraking();
    }
  }

  public void OnCollisionEnter(Collision collision)
  {
    carController.HandleCollision(collision);
  }
}