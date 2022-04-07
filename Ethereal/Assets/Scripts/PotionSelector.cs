using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> _potionVariations;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<GameObject> _spawnedPotions = new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            int potionIndex = Random.Range(0, _potionVariations.Count);

            GameObject potion = Instantiate(_potionVariations[potionIndex], _spawnPoints[i]);
            _spawnedPotions.Add(potion);
        }
    }
}
