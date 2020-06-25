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
    private GameManager gameManager;

    #endregion

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        projectileController = gameObject.GetComponent<ProjectileController>();
        gameManager = FindObjectOfType<GameManager>();
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
        if (!isProjectile && playerController.gameObjectInSight == other.gameObject && gameObject.GetComponentInParent<PlayerController>() != null)
        {
            // TODO: Don't hit through walls
            Debug.Log("player MELEE attack!");
            PlayerAttack(isProjectile, other);

            //Vector3 tpoint = playerController.fieldOfView.transform.position + playerController.transform.forward * 2f;
            //Debug.DrawLine(playerController.transform.position, tpoint, Color.magenta, 5);
            //RaycastHit hit;
            //if (Physics.Raycast(playerController.transform.position, playerController.transform.forward, out hit, 2))
            //{
            //    Debug.Log(hit.collider);
            //    if (hit.collider.gameObject == other.gameObject)
            //    {
            //        Debug.Log("player MELEE attack!");
            //        PlayerAttack(isProjectile, other);
            //    }
            //}

            //if (!Physics.Raycast(playerController.fieldOfView.transform.position, playerController.transform.forward, 2, playerController.fieldOfView.obstacleMask))
            //{
            //    Debug.Log("player MELEE attack!");
            //    PlayerAttack(isProjectile, other);
            //}

        }
        // if player attacks with projectile
        else if (isProjectile && other.GetComponentInParent<EnemyController>() != null && projectileController.Owner.CompareTag("Player"))
        {
            PlayerAttack(isProjectile, other);
        }

        //if enemy attacks with melee
        if (!isProjectile && gameObject.GetComponentInParent<EnemyController>() != null && other.gameObject.GetComponentInParent<PlayerController>() != null)
        {
            EnemyAttack(isProjectile, other);
        }
        //if enemy attacks with ranged
        else if (isProjectile && other.gameObject.GetComponentInParent<PlayerController>() != null && projectileController.Owner.CompareTag("Enemy"))
        {
            Debug.Log("projectile hit");
            EnemyAttack(isProjectile, other);
        }

        if (other.gameObject.CompareTag("HidingObject"))
        {
            ObjectHealthManager objectHealthManager = other.gameObject.GetComponent<ObjectHealthManager>();
            var dmg = isProjectile ? DamageToGive : GetDamage();
            objectHealthManager.HurtObject(dmg);
        }
    }

    private void PlayerAttack(bool isProjectile, Collider other)
    {
        if (other.GetComponentInParent<EnemyController>() != null && gameObject.GetComponentInParent<EnemyController>() == null)
        {
            EnemyController enemyController = other.GetComponentInParent<EnemyController>();
            if (!enemyController.State.Exists(s => s == StateEnum.Dead))
            {
                enemyController.StartHunt();
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

    private void EnemyAttack(bool isProjectile, Collider other)
    {
        HealthManager hm = other.gameObject.GetComponentInParent<HealthManager>();
        var enemyController = isProjectile ? projectileController.Owner.GetComponent<EnemyController>() : gameObject.GetComponentInParent<EnemyController>();
        if (enemyController != null)
        {
            var dmg = isProjectile ? DamageToGive : enemyController.Stats.RightHandAttack.Damage;
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
