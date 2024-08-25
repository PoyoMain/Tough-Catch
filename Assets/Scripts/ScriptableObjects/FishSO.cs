using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class FishSO : ScriptableObject
{
    [TextArea(2,3)]
    [SerializeField] private string _description;
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _minWeight;
    [SerializeField] private float _maxWeight;

    public string Description => _description ?? string.Empty;    
    public float MaxHealth => _maxHealth;
    public float MinWeight => _minWeight;
    public float MaxWeight => _maxWeight;
}

public class Fish
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public float Health { get; private set; }
    public float Weight { get; private set; }

    public FishWeightClass WeightClass
    {
        get
        {
            float percentage = ((Weight - _minWeight) * 100) / (_maxWeight - _minWeight);

            if (percentage < 30) return FishWeightClass.Small;
            else if (percentage < 70) return FishWeightClass.Medium;
            else if (percentage < 90) return FishWeightClass.Large;
            else return FishWeightClass.Gigantic;
        }
    }

    private float _maxHealth;
    private float _maxWeight;
    private float _minWeight;

    private FishSO data;

    public Fish(FishSO fishData)
    {
        Name = fishData.name;
        Description = fishData.Description;

        Health = fishData.MaxHealth;
        _maxHealth = fishData.MaxHealth;

        Weight = Random.Range(fishData.MinWeight, fishData.MaxWeight);
        _minWeight = fishData.MinWeight;
        _maxWeight = fishData.MaxWeight;
    }
}

public enum FishWeightClass
{
    Small,
    Medium,
    Large,
    Gigantic,
}
