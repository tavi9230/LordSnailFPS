using System.Collections.Generic;
using UnityEngine;

public class HurtEnemy : MonoBehaviour
{
    #region Variables
    public Dictionary<DamageTypeEnum, Damage> DamageToGive = new Dictionary<DamageTypeEnum, Damage>();
    public InventoryItem Item;
    public Skill Skill;

    private PlayerController playerController;
    private ProjectileController projectileController;
    private PlayerAttackEnum playerAttackType;

    #endregion

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        projectileController = gameObject.GetComponent<ProjectileController>();
    }

    public void SetupHurtObject(Dictionary<DamageTypeEnum, Damage> damageToGive, PlayerAttackEnum playerAttackType = PlayerAttackEnum.None, InventoryItem item = null, Skill skill = null)
    {
        DamageToGive = damageToGive;
        Item = item;
        Skill = skill;
        this.playerAttackType = playerAttackType;
    }

    private void OnTriggerEnter(Collider other)
    {
        var isProjectile = DamageToGive.Count > 0;

        // if player attacks with melee
        if (playerController.fieldOfView.visibleTargets.Count > 0 && playerController.fieldOfView.visibleTargets.Find(t => t.GetComponentInParent<EnemyController>() != null && t.GetComponentInParent<EnemyController>().Equals(other.GetComponentInParent<EnemyController>())))
        {
            if (other.GetComponentInParent<EnemyController>() != null && !isProjectile && gameObject.GetComponentInParent<EnemyController>() == null)
            {
                EnemyController enemyController = other.GetComponentInParent<EnemyController>();
                if (!enemyController.State.Exists(s => s == StateEnum.Dead))
                {
                    EnemyHealthManager enemyHealthManager = other.GetComponentInParent<EnemyHealthManager>();
                    var dmg = isProjectile ? DamageToGive : GetDamage();
                    var dmgToGive = enemyHealthManager.HurtEnemy(dmg, playerController, playerAttackType);

                    if (dmg[DamageTypeEnum.Bludgeoning].MinValue > 0 && dmg[DamageTypeEnum.Bludgeoning].MaxValue > 0)
                    {
                        if (!enemyController.Conditions.Exists(c => c == ConditionEnum.KnockedBack))
                        {
                            enemyController.Conditions.Add(ConditionEnum.KnockedBack);
                            enemyController.KnockBackDifference = enemyController.gameObject.transform.position - transform.position;
                        }
                    }
                    if (dmg[DamageTypeEnum.Piercing].MinValue > 0 && dmg[DamageTypeEnum.Piercing].MaxValue > 0)
                    {
                        if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
                        {
                            enemyHealthManager.bleedDamage = dmgToGive / 2;
                            enemyController.Conditions.Add(ConditionEnum.Bleeding);
                        }
                    }
                    if (dmg[DamageTypeEnum.Slashing].MinValue > 0 && dmg[DamageTypeEnum.Slashing].MaxValue > 0)
                    {
                        if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Staggered))
                        {
                            enemyController.StunTimer = 1.2f;
                            enemyController.Conditions.Add(ConditionEnum.Staggered);
                        }
                    }
                }
            }
        }

        //Debug.Log("Hit Enemy");
        //var isProjectile = DamageToGive.Count > 0;

        //// Check owner of attack and if enemy, shouldn't hurt
        //if ((other.GetComponentInParent<EnemyController>() != null && isProjectile && !projectileController.Owner.CompareTag("Enemy"))
        //    || (other.GetComponentInParent<EnemyController>() != null && !isProjectile && gameObject.GetComponentInParent<EnemyController>() == null))
        //{
        //    EnemyController enemyController = other.GetComponentInParent<EnemyController>();
        //    if (!enemyController.State.Exists(s => s == StateEnum.Dead))
        //    {
        //        EnemyHealthManager enemyHealthManager = other.gameObject.GetComponent<EnemyHealthManager>();
        //        var dmg = isProjectile ? DamageToGive : GetDamage();
        //        var dmgToGive = enemyHealthManager.HurtEnemy(dmg, playerController, playerAttackType);

        //        if (dmg[DamageTypeEnum.Bludgeoning].MinValue > 0 && dmg[DamageTypeEnum.Bludgeoning].MaxValue > 0)
        //        {
        //            if (!enemyController.Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        //            {
        //                enemyController.Conditions.Add(ConditionEnum.KnockedBack);
        //                enemyController.KnockBackDifference = enemyController.gameObject.transform.position - transform.position;
        //            }
        //        }
        //        if (dmg[DamageTypeEnum.Piercing].MinValue > 0 && dmg[DamageTypeEnum.Piercing].MaxValue > 0)
        //        {
        //            if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
        //            {
        //                enemyHealthManager.bleedDamage = dmgToGive / 2;
        //                enemyController.Conditions.Add(ConditionEnum.Bleeding);
        //            }
        //        }
        //        if (dmg[DamageTypeEnum.Slashing].MinValue > 0 && dmg[DamageTypeEnum.Slashing].MaxValue > 0)
        //        {
        //            if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Staggered))
        //            {
        //                enemyController.StunTimer = 1.2f;
        //                enemyController.Conditions.Add(ConditionEnum.Staggered);
        //            }
        //        }
        //    }
        //}

        //if (other.gameObject.CompareTag("HidingObject"))
        //{
        //    ObjectHealthManager objectHealthManager = other.gameObject.GetComponent<ObjectHealthManager>();
        //    var dmg = isProjectile ? DamageToGive : GetDamage();
        //    objectHealthManager.HurtObject(dmg);
        //}

        //// Check owner of attack and if player, shouldn't hurt
        //if ((other.GetComponentInParent<PlayerController>() != null && isProjectile && !projectileController.Owner.CompareTag("Player"))
        //    || (other.GetComponentInParent<PlayerController>() != null && !isProjectile && gameObject.GetComponentInParent<PlayerController>() == null))
        //{
        //    HealthManager hm = other.gameObject.GetComponent<HealthManager>();
        //    var enemyController = projectileController != null ? projectileController.Owner.GetComponent<EnemyController>() : gameObject.GetComponentInParent<EnemyController>();
        //    if (enemyController != null)
        //    {
        //        var dmg = DamageToGive.Count > 0 ? DamageToGive : enemyController.Stats.RightHandAttack.Damage;
        //        var dmgToGive = hm.HurtPlayer(dmg, enemyController);

        //        if (dmg[DamageTypeEnum.Bludgeoning].MinValue > 0 && dmg[DamageTypeEnum.Bludgeoning].MaxValue > 0)
        //        {
        //            if (!playerController.Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        //            {
        //                playerController.Conditions.Add(ConditionEnum.KnockedBack);
        //                playerController.KnockBackDifference = playerController.gameObject.transform.position - transform.position;
        //            }
        //        }
        //        if (dmg[DamageTypeEnum.Piercing].MinValue > 0 && dmg[DamageTypeEnum.Piercing].MaxValue > 0)
        //        {
        //            if (!playerController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
        //            {
        //                hm.bleedDamage = dmgToGive / 2;
        //                playerController.Conditions.Add(ConditionEnum.Bleeding);
        //            }
        //        }
        //        if (dmg[DamageTypeEnum.Slashing].MinValue > 0 && dmg[DamageTypeEnum.Slashing].MaxValue > 0)
        //        {
        //            if (!playerController.Conditions.Exists(c => c == ConditionEnum.Staggered))
        //            {
        //                playerController.StunTimer = 1.2f;
        //                playerController.Conditions.Add(ConditionEnum.Staggered);
        //            }
        //        }
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var isProjectile = DamageToGive.Count > 0;

        // Check owner of attack and if enemy, shouldn't hurt
        if ((other.gameObject.CompareTag("Enemy") && isProjectile && !projectileController.Owner.CompareTag("Enemy"))
            || (other.gameObject.CompareTag("Enemy") && !isProjectile && gameObject.GetComponentInParent<EnemyController>() == null))
        {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (!enemyController.State.Exists(s => s == StateEnum.Dead))
            {
                EnemyHealthManager enemyHealthManager = other.gameObject.GetComponent<EnemyHealthManager>();
                var dmg = isProjectile ? DamageToGive : GetDamage();
                var dmgToGive = enemyHealthManager.HurtEnemy(dmg, playerController, playerAttackType);

                if (dmg[DamageTypeEnum.Bludgeoning].MinValue > 0 && dmg[DamageTypeEnum.Bludgeoning].MaxValue > 0)
                {
                    if (!enemyController.Conditions.Exists(c => c == ConditionEnum.KnockedBack))
                    {
                        enemyController.Conditions.Add(ConditionEnum.KnockedBack);
                        enemyController.KnockBackDifference = enemyController.gameObject.transform.position - transform.position;
                    }
                }
                if (dmg[DamageTypeEnum.Piercing].MinValue > 0 && dmg[DamageTypeEnum.Piercing].MaxValue > 0)
                {
                    if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
                    {
                        enemyHealthManager.bleedDamage = dmgToGive / 2;
                        enemyController.Conditions.Add(ConditionEnum.Bleeding);
                    }
                }
                if (dmg[DamageTypeEnum.Slashing].MinValue > 0 && dmg[DamageTypeEnum.Slashing].MaxValue > 0)
                {
                    if (!enemyController.Conditions.Exists(c => c == ConditionEnum.Staggered))
                    {
                        enemyController.StunTimer = 1.2f;
                        enemyController.Conditions.Add(ConditionEnum.Staggered);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("HidingObject"))
        {
            ObjectHealthManager objectHealthManager = other.gameObject.GetComponent<ObjectHealthManager>();
            var dmg = isProjectile ? DamageToGive : GetDamage();
            objectHealthManager.HurtObject(dmg);
        }

        // Check owner of attack and if player, shouldn't hurt
        if ((other.gameObject.CompareTag("Player") && isProjectile && !projectileController.Owner.CompareTag("Player"))
            || (other.gameObject.CompareTag("Player") && !isProjectile && gameObject.GetComponentInParent<PlayerController>() == null))
        {
            HealthManager hm = other.gameObject.GetComponent<HealthManager>();
            var enemyController = projectileController != null ? projectileController.Owner.GetComponent<EnemyController>() : gameObject.GetComponentInParent<EnemyController>();
            if (enemyController != null)
            {
                var dmg = DamageToGive.Count > 0 ? DamageToGive : enemyController.Stats.RightHandAttack.Damage;
                var dmgToGive = hm.HurtPlayer(dmg, enemyController);

                if (dmg[DamageTypeEnum.Bludgeoning].MinValue > 0 && dmg[DamageTypeEnum.Bludgeoning].MaxValue > 0)
                {
                    if (!playerController.Conditions.Exists(c => c == ConditionEnum.KnockedBack))
                    {
                        playerController.Conditions.Add(ConditionEnum.KnockedBack);
                        playerController.KnockBackDifference = playerController.gameObject.transform.position - transform.position;
                    }
                }
                if (dmg[DamageTypeEnum.Piercing].MinValue > 0 && dmg[DamageTypeEnum.Piercing].MaxValue > 0)
                {
                    if (!playerController.Conditions.Exists(c => c == ConditionEnum.Bleeding))
                    {
                        hm.bleedDamage = dmgToGive / 2;
                        playerController.Conditions.Add(ConditionEnum.Bleeding);
                    }
                }
                if (dmg[DamageTypeEnum.Slashing].MinValue > 0 && dmg[DamageTypeEnum.Slashing].MaxValue > 0)
                {
                    if (!playerController.Conditions.Exists(c => c == ConditionEnum.Staggered))
                    {
                        playerController.StunTimer = 1.2f;
                        playerController.Conditions.Add(ConditionEnum.Staggered);
                    }
                }
            }
        }
    }

    public Dictionary<DamageTypeEnum, Damage> GetDamage()
    {
        string rightHandText = string.Format("{0}ightHand", playerController.InventoryManager.IsUsingAlternateWeapons ? "alternateR" : "r");
        string leftHandText = string.Format("{0}eftHand", playerController.InventoryManager.IsUsingAlternateWeapons ? "alternateL" : "l");
        Dictionary<DamageTypeEnum, Damage> damageList;
        switch (playerController.PlayerAttackType)
        {
            case PlayerAttackEnum.Right:
                if (playerController.Stats.RightHandAttack.Attack.Item != null)
                {
                    damageList = playerController.Stats.RightHandAttack.Damage;
                    Item = playerController.Stats.RightHandAttack.Attack.Item;
                }
                else if (playerController.Stats.RightHandAttack.Attack.Skill != null)
                {
                    damageList = playerController.Stats.RightHandAttack.Attack.Skill.Damage;
                    Skill = playerController.Stats.RightHandAttack.Attack.Skill;
                }
                else
                {
                    damageList = new Dictionary<DamageTypeEnum, Damage>();
                    Skill = null;
                    Item = null;
                }
                break;
            case PlayerAttackEnum.Left:
                if (playerController.Stats.LeftHandAttack.Attack.Item != null)
                {
                    damageList = playerController.Stats.LeftHandAttack.Damage;
                    Item = playerController.Stats.LeftHandAttack.Attack.Item;
                }
                else if (playerController.Stats.LeftHandAttack.Attack.Skill != null)
                {
                    damageList = playerController.Stats.LeftHandAttack.Attack.Skill.Damage;
                    Skill = playerController.Stats.LeftHandAttack.Attack.Skill;
                }
                else
                {
                    damageList = new Dictionary<DamageTypeEnum, Damage>();
                    Skill = null;
                    Item = null;
                }
                break;
            case PlayerAttackEnum.None:
            default:
                damageList = new Dictionary<DamageTypeEnum, Damage> { };
                break;
        }
        return damageList;
    }
}
