using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    private EnemyController enemyController;    

    // Start is called before the first frame update
    public new void Start()
    {
        enemyController = GetComponent<EnemyController>();
        SetAllBodyPartsHitChance();
    }

    public override void Update()
    {
        CheckFlash();
        if (enemyController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
        {
            if (bleedingCounter <= 0)
            {
                enemyController.Stats.TotalHealth -= bleedDamage;
                enemyController.Stats.Health-= bleedDamage;
                enemyController.InventoryManager.UpdateStats(enemyController.Stats);
                bleedingCounter = bleedingTimer;
                bleedIndex++;
                isFlashActive = true;
                flashCounter = flashLength;
                if (bleedIndex >= bleedTimes)
                {
                    enemyController.Conditions.Remove(ConditionEnum.Bleeding);
                    bleedIndex = 0;
                }
            }
            else
            {
                bleedingCounter -= Time.deltaTime;
            }
        }
    }

    public float HurtEnemy(Dictionary<DamageTypeEnum, Damage> damageList, PlayerController playerController, PlayerAttackEnum playerAttackType)
    {
        isFlashActive = true;
        flashCounter = flashLength;
        float damageToGive = Attack.CalculateDamage(enemyController, playerController, damageList, FindObjectOfType<GameManager>(), playerAttackType);
        int hit = Random.Range(1, 100);
        var hitBodyPart = bodyPartHitChance.Find(b => b.MinChance <= hit && b.MaxChance >= hit);
        InventoryItem itemHit = playerController.InventoryManager.GetItemByLocationAndIndex(hitBodyPart.Location, 0);

        SubstractDamage(damageToGive, itemHit, enemyController.Stats);
        enemyController.InventoryManager.RecalculateDefense(enemyController.Stats);

        if (enemyController.Stats.TotalHealth <= 0)
        {
            FindObjectOfType<PlayerController>().GainExperience(enemyController.Stats.ExperiencePoints);
            enemyController.State.RemoveRange(0, enemyController.State.Count);
            if (!enemyController.State.Exists(s => s == StateEnum.Dead))
            {
                enemyController.State.Add(StateEnum.Dead);
            }
        }
        enemyController.InventoryManager.UpdateStats(enemyController.Stats);

        enemyController.InCombatFromDamage = true;
        return damageToGive;
    }
}
