using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public string Id { get; set; }
    public int Level { get; set; }
    public int StoredExperience { get; set; }
    public string Name { get; set; }
    public int ExperiencePoints { get; set; }
    public GameObject GameObject { get; set; }
    public MinMax Speed { get; set; }
    public MinMax Stamina { get; set; }
    public MinMax Health { get; set; }
    public int Mana { get; set; }
    public float StaminaRecoveryTime { get; set; }
    public float HealthRecoveryTime { get; set; }
    public float ManaRecoveryTime { get; set; }
    public MinMax InventorySlots { get; set; }
    public MinMax FoV { get; set; }
    public MinMax ViewDistance { get; set; }
    public int CriticalChance { get; set; }
    public Dictionary<string, InventoryItem> Inventory { get; set; }
    public Attributes Attributes { get; set; }
    public Dictionary<DamageTypeEnum, float> Resistances { get; set; }
    public Dictionary<string, int> Skills { get; set; }
    public int AvailableAttributePoints { get; set; }
    public PreferedFightStyleEnum PreferedFightStyle { get; set; }

    public Enemy() { }
}