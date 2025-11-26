using UnityEngine;

public interface IVehicleState
{
  void EnterState(CarController carController);
  void UpdateState();
  void FixedUpdateState();
  void ExitState();
  void HandleInput();
  void OnCollisionEnter(Collision collision);
}