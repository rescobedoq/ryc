using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
  private static EventBus _instance;
  public static EventBus Instance => _instance ??= new EventBus();

  private Dictionary<Type, List<Delegate>> eventHandlers;

  private EventBus()
  {
    eventHandlers = new Dictionary<Type, List<Delegate>>();
  }

  public void Subscribe<T>(Action<T> handler)
  {
    Type eventType = typeof(T);

    if (!eventHandlers.ContainsKey(eventType))
    {
      eventHandlers[eventType] = new List<Delegate>();
    }

    eventHandlers[eventType].Add(handler);
  }

  public void Unsubscribe<T>(Action<T> handler)
  {
    Type eventType = typeof(T);

    if (eventHandlers.ContainsKey(eventType))
    {
      eventHandlers[eventType].Remove(handler);
    }
  }

  public void Publish<T>(T eventData)
  {
    Type eventType = typeof(T);

    if (eventHandlers.ContainsKey(eventType))
    {
      foreach (var handler in eventHandlers[eventType])
      {
        (handler as Action<T>)?.Invoke(eventData);
      }
    }
  }
}

// Eventos de ejemplo para el vehículo
public class VehicleBoostEvent
{
  public float BoostDuration { get; set; }
  public float BoostMultiplier { get; set; }
}

public class VehiclePenaltyEvent
{
  public float PenaltyDuration { get; set; }
  public float SpeedReduction { get; set; }
}