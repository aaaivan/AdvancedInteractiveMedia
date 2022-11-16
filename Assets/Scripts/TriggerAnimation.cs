using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    public void Trigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
}
