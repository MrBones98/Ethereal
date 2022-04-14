using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> _potionVariations;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<GameObject> _spawnedPotions = new List<GameObject>();

    private int _collectedPotions = 0;
    private bool _isSpawned = false;

    public delegate void PotionsCollected();
    public static event PotionsCollected onAllPotionsCollected;
    private void Awake()
    {
        PotionSeller.onPotionsSpawned += OnSpawnPotions;
        Potion.onPotionCollected += OnPotionListUpdate;
    }

    private void OnPotionListUpdate()
    {
        _collectedPotions++;
    }

    private void OnSpawnPotions()
    {
        if (_isSpawned == false)
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            int potionIndex = Random.Range(0, _potionVariations.Count);

            GameObject potion = Instantiate(_potionVariations[potionIndex], _spawnPoints[i]);
            _spawnedPotions.Add(potion);
        }
        _isSpawned = true;
    }
    private void Update()
    {
        if (_collectedPotions == _spawnedPotions.Count) onAllPotionsCollected();
        //Debug.Log($"Collected potions: {_collectedPotions}");
    }
    private void OnDestroy()
    {
        PotionSeller.onPotionsSpawned -= OnSpawnPotions;
        Potion.onPotionCollected -= OnPotionListUpdate;
    }
}
