using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesTimer : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3f);
    }
}
