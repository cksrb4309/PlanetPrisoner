using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public List<UnityEvent> events;

    public void Execute(int index)
    {
        events[index].Invoke();
    }
}
