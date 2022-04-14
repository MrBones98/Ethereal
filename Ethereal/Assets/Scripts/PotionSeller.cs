using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSeller : MonoBehaviour
{
    [SerializeField] private GameObject _interactTextObject;
    [SerializeField] private GameObject _collectedPotionsTextObject;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private List<GameObject> _text;

    private bool _potionsCollected = false;
    private bool _interacting = false;
    private int _index = 0;

    public delegate void PotionSpawned();
    public static event PotionSpawned onPotionsSpawned;
    private void Awake()
    {
        PotionSelector.onAllPotionsCollected += OnAllPotionsCollected;
    }

    private void OnAllPotionsCollected()
    {
        _potionsCollected = true;  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            _interactTextObject.SetActive(true);
        }
        //else if (_potionsCollected == true)
        //{
        //    _collectedPotionsTextObject.SetActive(true);
        //}
    }
    private void Update()
    {
        if( _interacting)
        {
            CheckPlayerInteraction();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if(playerController != null)
        {
            _interacting = true;
        }
    }

    private void CheckPlayerInteraction()
    {
        //&& _potionsCollected == false events doing something fucky oh no daniel ahhhhh
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_interactTextObject.activeSelf == true) _interactTextObject.SetActive(false);
            if (_index == _text.Count - 1) _index = 0;

            //this is for later use when the text list is implemented, written, or whatever the actual behavior is (?)
            //Instantiate(_text[_index], _canvas.transform.position, Quaternion.identity);
            onPotionsSpawned();
            _index += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _interactTextObject.SetActive(false);
        //_collectedPotionsTextObject.SetActive(false);
        _interacting = false;
    }
    private void OnDestroy()
    {
        PotionSelector.onAllPotionsCollected -= OnAllPotionsCollected;
    }
}
