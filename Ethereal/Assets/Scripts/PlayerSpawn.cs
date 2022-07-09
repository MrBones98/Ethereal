using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _spawnDelay;

    private void Start()
    {
        StartCoroutine(nameof(SpawnDelay));
        
    }

    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(_spawnDelay);
        _playerPrefab.SetActive(true);

    }
}
