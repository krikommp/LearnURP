using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Camera c1;
    public Camera c2;

    private void Start()
    {
        c2.depth = c1.depth - 1;
    }
}
