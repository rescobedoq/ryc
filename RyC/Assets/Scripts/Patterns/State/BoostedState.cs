using UnityEngine;

public class BoostedState : IVehicleState
{
  private CarController carController;
  private float boostTimer;
  private float boostDuration = 3f;
  private float boostMultiplier = 1.5f;

  public void EnterState(CarController controller)
  {
    carController = controller;
    boostTimer = 0f;

    carController.ApplyBoostPhysics();
    Debug.Log("Estado: Boosted - ¡Impulso de velocidad!");
  }

  public void UpdateState()
  {
    boostTimer += Time.deltaTime;

    if (boostTimer >= boostDuration)
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
    Debug.Log("Boost terminado");
  }

  public void HandleInput()
  {
    float verticalInput = Input.GetAxis("Vertical");
    float horizontalInput = Input.GetAxis("Horizontal");

    carController.ApplyAcceleration(verticalInput * boostMultiplier);
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