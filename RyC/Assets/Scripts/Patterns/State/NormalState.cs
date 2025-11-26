using UnityEngine;

public class NormalState : IVehicleState
{
  private CarController carController;
  private float normalSpeed;
  private float normalAcceleration;

  public void EnterState(CarController controller)
  {
    carController = controller;
    normalSpeed = carController.BaseSpeed;
    normalAcceleration = carController.BaseAcceleration;

    carController.ApplyNormalPhysics();
    Debug.Log("Estado: Normal");
  }

  public void UpdateState() { }

  public void FixedUpdateState()
  {
    carController.ApplyVehicleStabilization();
  }

  public void ExitState() { }

  public void HandleInput()
  {
    var (verticalInput, horizontalInput, isBraking) = carController.GetPlayerInput();
    
    float multiplier = 1f;
    
    carController.ApplyAcceleration(verticalInput * multiplier);
    carController.ApplySteering(horizontalInput);
    
    if (isBraking)
    {
      carController.ApplyBraking();
    }
  }

  public void OnCollisionEnter(Collision collision)
  {
    if (collision.relativeVelocity.magnitude > 5f)
    {
      carController.HandleCollision(collision);
    }
  }
}