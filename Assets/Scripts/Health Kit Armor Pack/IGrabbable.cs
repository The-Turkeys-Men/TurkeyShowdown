using UnityEngine.Events;

public interface IGrabbable
{
    UnityEvent OnGrab { get; set; }
}
