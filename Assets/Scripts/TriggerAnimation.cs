using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
