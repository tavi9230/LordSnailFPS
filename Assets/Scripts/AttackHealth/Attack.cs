using System.Collections.Generic;
using UnityEngine;

public static class Attack
{
    // When enemy attacks
    public static float CalculateDamage(PlayerController playerController, EnemyController enemyController, Dictionary<DamageTypeEnum, Damage> damageList)
    {
        float damageToGive = 0;

        foreach (var dmgType in damageList.Keys)
        {
            float d;
            if (enemyController.IsHeavyAttack)
            {
                d = damageList[dmgType].MaxValue;
            }
            else
            {
                d = Random.Range(damageList[dmgType].MinValue, damageList[dmgType].MaxValue);
            }
            if (playerController.Stats.TotalResistances.ContainsKey(damageList[dmgType].Type))
            {
                var dmg = d - (d * (playerController.Stats.TotalResistances[damageList[dmgType].Type] / 100));
                damageToGive += dmg > 0 ? dmg : 0;
            }
            else
            {
                damageToGive += d;
            }
        }

        enemyController.IsHeavyAttack = false;

        damageToGive = CalculateCriticalDamage(damageToGive, enemyController.Stats.TotalCriticalChance);

        if (enemyController.Stats.RightHandAttack.Attack.Item != null)
        {
            switch (enemyController.Stats.RightHandAttack.Attack.Item.Type)
            {
                case ItemTypeEnum.Weapon:
                    if (enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Axe
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Club
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Dagger
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Hammer
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Spear
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Sword)
                    {
                        damageToGive += (float)enemyController.Stats.TotalAttributes.Strength / 2;
                        enemyController.Stats.RightHandAttack.Attack.Item.Durability.Value--;
                    }
                    else if (enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Bow
                        || enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Crossbow)
                    {
                        damageToGive += (float)enemyController.Stats.TotalAttributes.Dexterity / 2;
                    }
                    else if (enemyController.Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Other)
                    {
                        if (enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("dart")
                            || enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("javelin"))
                        {
                            var stat = enemyController.Stats.TotalAttributes.Strength > enemyController.Stats.TotalAttributes.Dexterity
                                ? enemyController.Stats.TotalAttributes.Strength
                                : enemyController.Stats.TotalAttributes.Dexterity;
                            damageToGive += (float)stat / 2;
                        }
                        else if (enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("sling")
                            || enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("blowgun"))
                        {
                            damageToGive += (float)enemyController.Stats.TotalAttributes.Dexterity / 2;
                        }
                        else if (enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("sickle")
                            || enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("flail")
                            || enemyController.Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("whip"))
                        {
                            damageToGive += (float)enemyController.Stats.TotalAttributes.Strength / 2;
                        }
                    }
                    break;
                case ItemTypeEnum.None:
                    damageToGive += (float)enemyController.Stats.TotalAttributes.Strength / 2;
                    break;
            }
        }
        else if (enemyController.Stats.RightHandAttack.Attack.Skill != null)
        {
            damageToGive += (float)enemyController.Stats.TotalAttributes.Intelligence / 2;
        }

        Debug.Log(damageToGive);
        return damageToGive;
    }

    // When player attacks
    public static float CalculateDamage(EnemyController enemyController, PlayerController playerController, Dictionary<DamageTypeEnum, Damage> damageList, GameManager gameManager, PlayerAttackEnum playerAttackType)
    {
        InventoryItem itemUsedForAttack = null;
        float damageToGive = 0;

        foreach (var dmgType in damageList.Keys)
        {
            float d;
            if (playerController.IsHeavyAttack)
            {
                d = damageList[dmgType].MaxValue;
            }
            else
            {
                d = Random.Range(damageList[dmgType].MinValue, damageList[dmgType].MaxValue);
            }
            if (enemyController.Stats.TotalResistances.ContainsKey(damageList[dmgType].Type))
            {
                var dmg = d - (d * (enemyController.Stats.TotalResistances[damageList[dmgType].Type] / 100));
                damageToGive += dmg > 0 ? dmg : 0;
            }
            else
            {
                damageToGive += d;
            }
        }

        playerController.IsHeavyAttack = false;

        damageToGive = CalculateCriticalDamage(damageToGive, playerController.Stats.TotalCriticalChance);

        // if stealth => triple damage
        if (!gameManager.CombatEnemyList.Contains(enemyController.gameObject))
        {
            damageToGive *= 3;
            Debug.Log("Stealth attack!");
        }

        string rightHandText = string.Format("{0}ightHand", playerController.InventoryManager.IsUsingAlternateWeapons ? "alternateR" : "r");
        string leftHandText = string.Format("{0}eftHand", playerController.InventoryManager.IsUsingAlternateWeapons ? "alternateL" : "l");
        Skill skillUsedForAttack = null;
        var pat = playerAttackType != PlayerAttackEnum.None && playerAttackType != PlayerAttackEnum.ActiveSkill
            ? playerAttackType
            : playerController.PlayerAttackType;
        switch (pat)
        {
            case PlayerAttackEnum.Right:
                if (playerController.Stats.RightHandAttack.Attack.Item != null)
                {
                    itemUsedForAttack = playerController.InventoryManager.Inventory[rightHandText];
                }
                else if (playerController.Stats.RightHandAttack.Attack.Skill != null)
                {
                    skillUsedForAttack = playerController.Stats.RightHandAttack.Attack.Skill;
                }
                break;
            case PlayerAttackEnum.Left:
                if (playerController.Stats.LeftHandAttack.Attack.Item != null)
                {
                    itemUsedForAttack = playerController.InventoryManager.Inventory[leftHandText];
                }
                else if (playerController.Stats.LeftHandAttack.Attack.Skill != null)
                {
                    skillUsedForAttack = playerController.Stats.LeftHandAttack.Attack.Skill;
                }
                break;
            case PlayerAttackEnum.ActiveSkill:
                skillUsedForAttack = playerController.Stats.ActiveSkill;
                break;
            case PlayerAttackEnum.None:
            default:
                break;
        }

        if (itemUsedForAttack != null)
        {
            switch (itemUsedForAttack.Type)
            {
                case ItemTypeEnum.Weapon:
                    if (itemUsedForAttack.ItemCategory == ItemCategoryEnum.Axe
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Club
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Dagger
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Hammer
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Spear
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Sword)
                    {
                        damageToGive += (float)playerController.Stats.TotalAttributes.Strength / 2;
                        itemUsedForAttack.Durability.Value--;
                    }
                    else if (itemUsedForAttack.ItemCategory == ItemCategoryEnum.Bow
                        || itemUsedForAttack.ItemCategory == ItemCategoryEnum.Crossbow)
                    {
                        damageToGive += (float)playerController.Stats.TotalAttributes.Dexterity / 2;
                    }
                    else if (itemUsedForAttack.ItemCategory == ItemCategoryEnum.Other)
                    {
                        if (itemUsedForAttack.Name.ToLowerInvariant().Contains("dart")
                            || itemUsedForAttack.Name.ToLowerInvariant().Contains("javelin"))
                        {
                            var stat = playerController.Stats.TotalAttributes.Strength > playerController.Stats.TotalAttributes.Dexterity
                                ? playerController.Stats.TotalAttributes.Strength
                                : playerController.Stats.TotalAttributes.Dexterity;
                            damageToGive += (float)stat / 2;
                        }
                        else if (itemUsedForAttack.Name.ToLowerInvariant().Contains("sling")
                            || itemUsedForAttack.Name.ToLowerInvariant().Contains("blowgun"))
                        {
                            damageToGive += (float)playerController.Stats.TotalAttributes.Dexterity / 2;
                        }
                        else if (itemUsedForAttack.Name.ToLowerInvariant().Contains("sickle")
                            || itemUsedForAttack.Name.ToLowerInvariant().Contains("flail")
                            || itemUsedForAttack.Name.ToLowerInvariant().Contains("whip"))
                        {
                            damageToGive += (float)playerController.Stats.TotalAttributes.Strength / 2;
                        }
                    }
                    break;
                case ItemTypeEnum.None:
                    damageToGive += (float)playerController.Stats.TotalAttributes.Strength / 2;
                    break;
            }
        }
        if (skillUsedForAttack != null)
        {
            damageToGive += (float)playerController.Stats.TotalAttributes.Intelligence / 2;
        }

        Debug.Log(damageToGive);
        return damageToGive;
    }

    private static float CalculateCriticalDamage(float damageToGive, int criticalChance)
    {
        int critNumber = Random.Range(1, 100) + criticalChance;
        if (critNumber >= 95)
        {
            /*
             * 
             * leftHand/rightHand: drop
             * feet: speed--;
             * hip: inventory drop;
             */
            damageToGive = damageToGive * 2;
        }
        return damageToGive;
    }
}
