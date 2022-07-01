using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField]private SceneLoader _sceneLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        if(controller != null)
        {
            RespawnPlayer();
        }
    }
    private void RespawnPlayer()
    {
        _sceneLoader.ReloadScene();
    }
}
