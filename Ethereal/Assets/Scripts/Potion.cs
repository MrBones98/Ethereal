using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public delegate void PotionCollected();
    public static event PotionCollected onPotionCollected;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if(playerController != null)
        {
            onPotionCollected();
            Destroy(gameObject);
        }
    }
}
