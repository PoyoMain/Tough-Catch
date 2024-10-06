using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class FishSO : ScriptableObject
{
    [TextArea(2,3)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _minWeight;
    [SerializeField] private float _maxWeight;
    [SerializeField] private float _minLength;
    [SerializeField] private float _maxLength;

    public string Description => _description ?? string.Empty;    
    public Sprite Sprite => _sprite;
    public float MaxHealth => _maxHealth;
    public float MinWeight => _minWeight;
    public float MaxWeight => _maxWeight;
    public float MinLength => _minLength;
    public float MaxLength => _maxLength;
}

public class Fish
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Sprite Image { get; private set; }
    public float Health { get; private set; }
    public float Weight { get; private set; }
    public float Length { get; private set; }

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

    private readonly float _maxHealth;
    private readonly float _maxWeight;
    private readonly float _minWeight;
    private readonly float _maxLength;
    private readonly float _minLength;

    public Fish(FishSO fishData)
    {
        Name = fishData.name;
        Description = fishData.Description;
        Image = fishData.Sprite;

        _maxHealth = fishData.MaxHealth;
        Health = _maxHealth;
        
        _minWeight = fishData.MinWeight;
        _maxWeight = fishData.MaxWeight;
        Weight = Mathf.Round(Random.Range(fishData.MinWeight, fishData.MaxWeight) * 100) / 100;

        _minLength = fishData.MinLength;
        _maxLength = fishData.MaxLength;
        Length = Mathf.Round(((((Weight - _minWeight) / (_maxWeight - _minWeight)) * (_maxLength - _minLength)) + _minLength) * 100) / 100;
    }
}

public enum FishWeightClass
{
    Small,
    Medium,
    Large,
    Gigantic,
}
