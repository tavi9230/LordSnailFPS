using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    #region Variables
    
    public List<BodyPartHitChance> bodyPartHitChance = new List<BodyPartHitChance>();

    public float bleedingCounter = 0f;
    public float bleedingTimer = 1f;
    public int bleedTimes = 3;
    public int bleedIndex = 0;
    public float bleedDamage = 0;

    private float aHue = 1f;
    private PlayerController playerController;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        playerController = GetComponent<PlayerController>();
        SetAllBodyPartsHitChance();
    }

    public virtual void SetHealth(float health, float maxHealth, Stats stats)
    {
        if (stats.Health == -1)
        {
            stats.Health = health;
        }
        if (stats.MaxHealth == -1)
        {
            stats.MaxHealth = maxHealth;
        }
    }

    public virtual void SetAllBodyPartsHitChance()
    {
        SetBodyPartHitChance(InventoryLocationEnum.Torso, 1, 30);
        SetBodyPartHitChance(InventoryLocationEnum.Head, 31, 50);
        SetBodyPartHitChance(InventoryLocationEnum.Hands, 51, 60);
        SetBodyPartHitChance(InventoryLocationEnum.LeftHand, 61, 70);
        SetBodyPartHitChance(InventoryLocationEnum.RightHand, 71, 80);
        SetBodyPartHitChance(InventoryLocationEnum.Feet, 81, 90);
        SetBodyPartHitChance(InventoryLocationEnum.Hip, 91, 100);
    }

    private void SetBodyPartHitChance(InventoryLocationEnum location, int minValue, int maxValue)
    {
        bodyPartHitChance.Add(new BodyPartHitChance(location, minValue, maxValue));
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (playerController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
        {
            if (bleedingCounter <= 0)
            {
                playerController.Stats.TotalHealth -= bleedDamage;
                playerController.Stats.Health -= bleedDamage;
                playerController.InventoryManager.UpdateStats(playerController.Stats);
                bleedingCounter = bleedingTimer;
                bleedIndex++;
                if (bleedIndex >= bleedTimes)
                {
                    playerController.Conditions.Remove(ConditionEnum.Bleeding);
                    bleedIndex = 0;
                }
            }
            else
            {
                bleedingCounter -= Time.deltaTime;
            }
        }
    }

    public float HurtPlayer(Dictionary<DamageTypeEnum, Damage> damageList, EnemyController enemyController)
    {
        float damageToGive = Attack.CalculateDamage(playerController, enemyController, damageList);

        int hit = UnityEngine.Random.Range(1, 100);
        var hitBodyPart = bodyPartHitChance.Find(b => b.MinChance <= hit && b.MaxChance >= hit);
        InventoryItem item = playerController.InventoryManager.GetItemByLocationAndIndex(hitBodyPart.Location, 0);

        SubstractDamage(damageToGive, item, playerController.Stats);

        playerController.InventoryManager.RecalculateDefense(playerController.Stats);

        if (playerController.Stats.TotalHealth <= 0)
        {
            if (!playerController.State.Exists(s => s == StateEnum.Dead))
            {
                playerController.State.Add(StateEnum.Dead);
            }
            gameObject.SetActive(false);
        }
        playerController.InventoryManager.UpdateStats(playerController.Stats);
        return damageToGive;
    }

    public void SubstractDamage(float damageToGive, InventoryItem item, Stats stats)
    {
        stats.TotalHealth -= damageToGive;
        stats.Health -= damageToGive;
        if (item.Type != ItemTypeEnum.None && item.Durability.Value != 0)
        {
            item.Durability.Value--;
            if (item.Durability.Value < 0)
            {
                item.Durability.Value = 0;
            }
        }

        if (stats.TotalHealth < 0)
        {
            stats.TotalHealth = 0;
        }
    }
}
