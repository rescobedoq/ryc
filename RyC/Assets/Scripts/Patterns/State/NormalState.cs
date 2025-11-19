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
    carController.AppvehicleStabilization();
  }

  public void ExitState() { }

  public void HandleInput()
  {
    float verticalInput = Input.GetAxis("Vertical");
    float horizontalInput = Input.GetAxis("Horizontal");

    carController.ApplyAcceleration(verticalInput);
    carController.ApplySteering(horizontalInput);

    if (Input.GetKey(KeyCode.Space))
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