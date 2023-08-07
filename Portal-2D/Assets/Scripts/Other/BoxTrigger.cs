using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple helper class that forwards OnTriggerEnter/Leave events
/// </summary>

public class BoxTrigger : MonoBehaviour
{
    [SerializeField] public UnityEvent<Collider2D> OnTriggerEnter;
    [SerializeField] public UnityEvent<Collider2D> OnTriggerLeave;

    void OnTriggerEnter2D( Collider2D collision )
    {
        OnTriggerEnter?.Invoke(collision);
    }

    void OnTriggerExit2D( Collider2D collision )
    {
        OnTriggerLeave?.Invoke(collision);
    }
}
