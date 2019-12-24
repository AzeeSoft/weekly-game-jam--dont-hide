using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationCheck : MonoBehaviour
{
    public bool stopConfusion = false;

    void CanMove()
    {
        stopConfusion = true;
    }
}
