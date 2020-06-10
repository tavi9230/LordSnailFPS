using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public List<StateEnum> State;
    public List<ConditionEnum> Conditions;
    public List<ConditionEnum> ConditionImmunities;
    public Stats Stats;
    public InventoryManager InventoryManager;
    public GameObject DraggedObject = null;
    public GameObject Hidespot = null;
    public PlayerAttackEnum PlayerAttackType;
    public PlayerAnimationController animationController;
    public bool IsHeavyAttack;
    public Vector3 KnockBackDifference = Vector2.zero;
    public float StunTimer = 0;
    public Dictionary<KeyCode, HotbarItem> Hotbar;
    public FieldOfView fieldOfView;

    private float staminaRecoveryCounter = 0f;
    private float healthRecoveryCounter = 0f;
    private float manaRecoveryCounter = 0f;
    private float attackCounter = 0;
    private float chargeAttackCounter = 0;
    [SerializeField]
    private float knockbackTimer = .25f;
    private float knockbackCounter = .25f;
    private float stunCounter = 0;
    private GameManager gameManager;
    private UIManager uiManager;
    #endregion

    void Awake()
    {
        Hotbar = new Dictionary<KeyCode, HotbarItem>
        {
            {KeyCode.Alpha0, new HotbarItem() },
            {KeyCode.Alpha1, new HotbarItem() },
            {KeyCode.Alpha2, new HotbarItem() },
            {KeyCode.Alpha3, new HotbarItem() },
            {KeyCode.Alpha4, new HotbarItem() },
            {KeyCode.Alpha5, new HotbarItem() },
            {KeyCode.Alpha6, new HotbarItem() },
            {KeyCode.Alpha7, new HotbarItem() },
            {KeyCode.Alpha8, new HotbarItem() },
            {KeyCode.Alpha9, new HotbarItem() },
        };
        State = new List<StateEnum>();
        Conditions = new List<ConditionEnum>() { ConditionEnum.Visible };
        ConditionImmunities = new List<ConditionEnum>();
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
        animationController = GetComponent<PlayerAnimationController>();
        fieldOfView = transform.GetChild(1).GetComponent<FieldOfView>();

        Stats = new Stats(
            5f,
            0,
            new Attributes()
            {
                Strength = 0,
                Dexterity = 0,
                Constitution = 0,
                Intelligence = 0,
                Charisma = 0
            },
            10f,
            10f,
            1f,
            new Dictionary<DamageTypeEnum, float>
            {
                { DamageTypeEnum.Slashing, 0},
                { DamageTypeEnum.Piercing, 0},
                { DamageTypeEnum.Bludgeoning, 0},
                { DamageTypeEnum.Cold, 0},
                { DamageTypeEnum.Fire, 0},
                { DamageTypeEnum.Lightning, 0},
                { DamageTypeEnum.Poison, 0},
            },
            10000,
            10000,
            2,
            90,
            5f,
            0,
            10,
            10,
            0);
        foreach (var id in gameManager.Skills.Keys)
        {
            Skill skill = new Skill(gameManager.Skills[id]);
            Stats.Skills[ActivityEnum.Inactive].Add(skill);
        }
        InventoryManager = GetComponent<InventoryManager>();

        PlayerAttackType = PlayerAttackEnum.None;
    }

    void Update()
    {
        CheckInFront();
        if (Input.GetKeyDown(KeyCode.M))
        {
            LevelUp();
        }
        RecalculateStamina(true);
        RecalculateHealth();
        RecalculateMana();

        if (State.Exists(s => s == StateEnum.Attacking))
        {
            attackCounter -= Time.deltaTime;
            if (attackCounter <= 0)
            {
                var itemHolder = new List<GameObject>(GameObject.FindGameObjectsWithTag("LeftItemHolder")).Find(g => g.transform.IsChildOf(transform));
                var hitbox = itemHolder.transform.GetChild(0).GetChild(0);
                //hitbox.transform.position = new Vector3(hitbox.transform.position.x, 0, hitbox.transform.position.z);
                hitbox.gameObject.SetActive(false);

                itemHolder = new List<GameObject>(GameObject.FindGameObjectsWithTag("RightItemHolder")).Find(g => g.transform.IsChildOf(transform));
                hitbox = itemHolder.transform.GetChild(0).GetChild(0);
                //hitbox.transform.position = new Vector3(hitbox.transform.position.x, 0, hitbox.transform.position.z);
                hitbox.gameObject.SetActive(false);

                animationController.SetAttackingLeft(false);
                animationController.SetAttackingRight(false);
                State.Remove(StateEnum.Attacking);
            }
        }

        if (Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        {
            var kbPosition = new Vector3(transform.position.x + KnockBackDifference.x, transform.position.y + KnockBackDifference.y, transform.position.z);

            if (!Conditions.Exists(c => c == ConditionEnum.Stunned))
            {
                Conditions.Add(ConditionEnum.Stunned);
                State.Remove(StateEnum.Attacking);
            }

            if (knockbackCounter <= 0)
            {
                knockbackCounter = knockbackTimer;
                Conditions.Remove(ConditionEnum.KnockedBack);
                Conditions.Remove(ConditionEnum.Stunned);
                KnockBackDifference = Vector3.zero;
            }
            else
            {
                knockbackCounter -= Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, kbPosition, Time.deltaTime);
            }
        }

        if (Conditions.Exists(c => c == ConditionEnum.Staggered))
        {
            Conditions.Remove(ConditionEnum.Staggered);
            //animationController.SetMoveSpeed(0, 0);
            State.Remove(StateEnum.Attacking);
            //animationController.SetIsAttacking(false);
            attackCounter = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!State.Exists(s => s == StateEnum.Inspecting))
            {
                State.Add(StateEnum.Inspecting);
            }
            else
            {
                State.Remove(StateEnum.Inspecting);
                uiManager.CloseEnemyInfo();
                // TODO: Close object inventory
                uiManager.InspectedEnemy = null;
            }
        }

        if (!Conditions.Exists(c => c == ConditionEnum.Stunned) && !Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        {
            if (!State.Exists(s => s == StateEnum.Hidden))
            {
                PlayerAnimateMove();
                if (Input.GetKeyDown(KeyCode.X))
                {
                    PlayerDrop();
                }
            }
            if (!State.Exists(s => s == StateEnum.Hidden)
                && !State.Exists(s => s == StateEnum.Dragging))
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    State.Add(StateEnum.Running);
                    InventoryManager.UpdateStats(Stats);
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    State.Remove(StateEnum.Running);
                    InventoryManager.UpdateStats(Stats);
                }
            }

            if (!State.Exists(s => s == StateEnum.Hidden)
            && !State.Exists(s => s == StateEnum.Dragging))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (State.Exists(s => s == StateEnum.Crouching))
                    {
                        State.Remove(StateEnum.Crouching);
                        InventoryManager.UpdateStats(Stats);
                        //GameObject.Find("Map").transform.GetChild(2).gameObject.layer = Constants.LAYER_DEFAULT;
                    }
                    else
                    {
                        State.Add(StateEnum.Crouching);
                        InventoryManager.UpdateStats(Stats);
                        //GameObject.Find("Map").transform.GetChild(2).gameObject.layer = Constants.LAYER_SOLID_OBJECTS;
                    }
                    uiManager.RefreshInventory();
                }
            }

            if (!State.Exists(s => s == StateEnum.Inspecting)
            && !State.Exists(s => s == StateEnum.Hidden)
            && !State.Exists(s => s == StateEnum.Dragging)
            && uiManager.CanAttack())
            {
                PlayerAttack();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                InventoryManager.SwitchWeapons();
            }

            if (State.Exists(s => s == StateEnum.Hidden))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CheckIfHidingInObject();
                }
            }
        }
        else if (Conditions.Exists(c => c == ConditionEnum.Stunned) && !Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        {
            if (stunCounter <= StunTimer)
            {
                State.Remove(StateEnum.Attacking);
                stunCounter += Time.deltaTime;
            }
            else
            {
                Conditions.Remove(ConditionEnum.Stunned);
                stunCounter = 0;
            }
        }

        if (!State.Exists(s => s == StateEnum.Hidden)
            && State.Exists(s => s == StateEnum.Dragging))
        {
            State.Remove(StateEnum.DraggingStart);
            SetDraggedObjectPosition();
        }
    }

    private void CheckInFront()
    {
        Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        Vector3 direction = new Vector3();

        RaycastHit hitCam;
        if (Physics.Raycast(Camera.main.transform.position, fwd, out hitCam, 500))
        {
            direction = hitCam.point;
        }
        else
        {
            direction = fwd * 500;
        }
        Debug.DrawLine(Camera.main.transform.position, direction, Color.green);

        RaycastHit hit;
        Debug.DrawLine(transform.position, direction, Color.blue);
        if (Physics.Linecast(Camera.main.transform.position, direction, out hit))
        {
            var enemyController = hit.collider.gameObject.GetComponentInParent<EnemyController>();
            if (enemyController != null)
            {
                TooltipHandler.DisplayEnemyTooltip(uiManager, enemyController.Stats.Name, enemyController.Stats.TotalHealth, enemyController.Stats.TotalMaxHealth);
            }
            else
            {
                TooltipHandler.HideTooltip(uiManager);
            }
        }
        else
        {
            TooltipHandler.HideTooltip(uiManager);
        }
    }

    public void GainExperience(float experiencePoints)
    {
        if (Stats.Level < Constants.PLAYER_LEVEL_MAX)
        {
            Stats.ExperiencePoints += experiencePoints;
            if (Stats.ExperiencePoints >= gameManager.LevelRequirements[Stats.Level + 1])
            {
                LevelUp();
            }
        }
    }

    public void LevelUp()
    {
        if (Stats.Level + 1 > Constants.PLAYER_LEVEL_MAX)
        {
            Stats.ExperiencePoints = 0;
        }
        else
        {
            Stats.StoredExperiencePoints += gameManager.LevelRequirements[Stats.Level + 1];
            Stats.ExperiencePoints = Stats.ExperiencePoints - gameManager.LevelRequirements[Stats.Level + 1];
            Stats.Level++;
            Stats.AvailableAttributePoints += Constants.EXPERIENCE_POINTS_PER_LEVEL;
            Stats.Health += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.MaxHealth += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.Stamina += Constants.STAMINA_POINTS_PER_LEVEL;
            Stats.MaxStamina += Constants.STAMINA_POINTS_PER_LEVEL;
            Stats.Mana += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.MaxMana += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.CriticalChance++;
            InventoryManager.UpdateStatsAndRefreshUI(Stats);
        }
    }

    private void RecalculateStamina(bool hasSkill = false)
    {
        if (hasSkill)
        {
            if (staminaRecoveryCounter <= 0)
            {
                staminaRecoveryCounter = Stats.TotalStaminaRecoveryTime;
                if (Stats.TotalStamina < Stats.TotalMaxStamina)
                {
                    Stats.TotalStamina++;
                    Stats.Stamina++;
                    if (Stats.TotalStamina > Stats.TotalMaxStamina)
                    {
                        Stats.TotalStamina = Stats.TotalMaxStamina;
                    }
                    if (Stats.Stamina > Stats.MaxStamina)
                    {
                        Stats.Stamina = Stats.MaxStamina;
                    }
                }
            }
            else
            {
                staminaRecoveryCounter -= Time.deltaTime;
            }
        }
    }

    private void RecalculateHealth(bool hasSkill = false)
    {
        if (hasSkill)
        {
            if (healthRecoveryCounter <= 0)
            {
                healthRecoveryCounter = Stats.TotalHealthRecoveryTime;
                if (Stats.TotalHealth < Stats.TotalMaxHealth)
                {
                    Stats.Health++;
                    Stats.TotalHealth++;
                    if (Stats.TotalHealth > Stats.TotalMaxHealth)
                    {
                        Stats.TotalHealth = Stats.TotalMaxHealth;
                    }
                    if (Stats.Health > Stats.MaxHealth)
                    {
                        Stats.Health = Stats.MaxHealth;
                    }
                }
            }
            else
            {
                healthRecoveryCounter -= Time.deltaTime;
            }
        }
    }

    private void RecalculateMana(bool hasSkill = false)
    {
        if (hasSkill)
        {
            if (manaRecoveryCounter <= 0)
            {
                manaRecoveryCounter = Stats.TotalManaRecoveryTime;
                if (Stats.TotalMana < Stats.TotalMaxMana)
                {
                    Stats.TotalMana++;
                    Stats.Mana++;
                    if (Stats.TotalMana > Stats.TotalMaxMana)
                    {
                        Stats.TotalMana = Stats.TotalMaxMana;
                    }
                    if (Stats.Mana > Stats.MaxMana)
                    {
                        Stats.Mana = Stats.MaxMana;
                    }
                }
            }
            else
            {
                manaRecoveryCounter -= Time.deltaTime;
            }
        }
    }

    public bool PickUp(InventoryItem inventoryItem)
    {
        return InventoryManager.PickUp(inventoryItem);
    }

    private void PlayerAnimateMove()
    {
        var axisH = Input.GetAxisRaw("HorizontalIsometric");
        var axisV = Input.GetAxisRaw("VerticalIsometric");

        Vector2 movement = new Vector2(axisH, axisV);
        //animationController.SetMoveSpeed(movement.x, movement.y);
    }

    private void PlayerAttack()
    {
        if (!State.Exists(s => s == StateEnum.Attacking))
        {
            PlayerAttackType = PlayerAttackEnum.None;
            if (!State.Exists(s => s == StateEnum.Pickup))
            {
                string rightHandText = string.Format("{0}ightHand", InventoryManager.IsUsingAlternateWeapons ? "alternateR" : "r");
                string leftHandText = string.Format("{0}eftHand", InventoryManager.IsUsingAlternateWeapons ? "alternateL" : "l");

                if (Input.GetMouseButton(0) && !State.Exists(s => s == StateEnum.IsChargingRightAttack))
                {
                    if (Stats.ActiveSkill != null
                        || Stats.LeftHandAttack.Attack.Skill != null
                        || Stats.LeftHandAttack.Attack.Item != null && Stats.LeftHandAttack.Attack.Item.Type != ItemTypeEnum.Ammo)
                    {
                        animationController.SetIsPreparingLeftAttack(true);
                        chargeAttackCounter += Time.deltaTime;
                        if (!State.Exists(s => s == StateEnum.IsChargingLeftAttack))
                        {
                            State.Add(StateEnum.IsChargingLeftAttack);
                        }
                    }
                }

                if (Input.GetMouseButton(1) && !State.Exists(s => s == StateEnum.IsChargingLeftAttack))
                {
                    animationController.SetIsPreparingRightAttack(true);
                    chargeAttackCounter += Time.deltaTime;
                    if (!State.Exists(s => s == StateEnum.IsChargingRightAttack))
                    {
                        State.Add(StateEnum.IsChargingRightAttack);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    PlayerAttackType = PlayerAttackEnum.Left;
                    AttackPower attackPower = Stats.LeftHandAttack;
                    IsHeavyAttack = false;
                    if (Stats.ActiveSkill != null)
                    {
                        if (chargeAttackCounter >= Stats.ActiveSkill.ChargeSpeed)
                        {
                            IsHeavyAttack = true;
                        }
                    }
                    else if (attackPower.Attack.Item != null)
                    {
                        if (chargeAttackCounter >= attackPower.ChargeAttackSpeed)
                        {
                            IsHeavyAttack = true;
                        }
                    }
                    else if (attackPower.Attack.Skill != null)
                    {
                        if (chargeAttackCounter >= attackPower.Attack.Skill.ChargeSpeed)
                        {
                            IsHeavyAttack = true;
                        }
                    }

                    chargeAttackCounter = 0;

                    CheckPlayerAttackHand(InventoryManager.Inventory[leftHandText], InventoryManager.Inventory[rightHandText], attackPower, true);

                    animationController.SetIsPreparingLeftAttack(false);
                    animationController.SetIsPreparingRightAttack(false);
                    State.Remove(StateEnum.IsChargingRightAttack);
                    State.Remove(StateEnum.IsChargingLeftAttack);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    PlayerAttackType = PlayerAttackEnum.Right;
                    AttackPower attackPower = Stats.RightHandAttack;
                    IsHeavyAttack = false;
                    if (attackPower.Attack.Item != null)
                    {
                        // TODO: Decrease Charge Speed value by armor weight and special stats if necessary
                        if (chargeAttackCounter >= attackPower.ChargeAttackSpeed)
                        {
                            IsHeavyAttack = true;
                        }
                    }
                    else if (attackPower.Attack.Skill != null)
                    {
                        if (chargeAttackCounter >= attackPower.Attack.Skill.ChargeSpeed)
                        {
                            IsHeavyAttack = true;
                        }
                    }
                    chargeAttackCounter = 0;

                    CheckPlayerAttackHand(InventoryManager.Inventory[rightHandText], InventoryManager.Inventory[leftHandText], attackPower);

                    animationController.SetIsPreparingLeftAttack(false);
                    animationController.SetIsPreparingRightAttack(false);
                    State.Remove(StateEnum.IsChargingRightAttack);
                    State.Remove(StateEnum.IsChargingLeftAttack);
                }
            }
        }
    }

    private void CheckPlayerAttackHand(InventoryItem mainHand, InventoryItem offHand, AttackPower attackPower, bool isLeftMouseButtonUp = false)
    {
        if (Stats.ActiveSkill != null && isLeftMouseButtonUp == true)
        {
            if (Stats.TotalMana >= Stats.ActiveSkill.ManaConsumption)
            {
                AttackWithActiveSkill();
            }
        }
        else
        {
            if (attackPower.Attack.Item != null)
            {
                if (mainHand.Type == ItemTypeEnum.Weapon && mainHand.Durability.Value > 0 && Stats.TotalStamina >= attackPower.StaminaConsumption)
                {
                    if (mainHand.ItemCategory == ItemCategoryEnum.Axe
                        || mainHand.ItemCategory == ItemCategoryEnum.Club
                        || mainHand.ItemCategory == ItemCategoryEnum.Dagger
                        || mainHand.ItemCategory == ItemCategoryEnum.Hammer
                        || mainHand.ItemCategory == ItemCategoryEnum.Spear
                        || mainHand.ItemCategory == ItemCategoryEnum.Sword)
                    {
                        AttackWithMelee(attackPower);
                    }
                    else if (mainHand.ItemCategory == ItemCategoryEnum.Bow)
                    {
                        AttackWithRanged(attackPower, mainHand, offHand, ItemCategoryEnum.BowAmmo);
                    }
                    else if (mainHand.ItemCategory == ItemCategoryEnum.Crossbow)
                    {
                        AttackWithRanged(attackPower, mainHand, offHand, ItemCategoryEnum.CrossbowAmmo);
                    }
                    else if (mainHand.ItemCategory == ItemCategoryEnum.Other)
                    {
                        if (mainHand.Name.ToLowerInvariant().Contains("dart")
                            || mainHand.Name.ToLowerInvariant().Contains("javelin"))
                        {
                            AttackWithOtherRanged(attackPower, mainHand);
                        }
                        else if (mainHand.Name.ToLowerInvariant().Contains("sling")
                            || mainHand.Name.ToLowerInvariant().Contains("blowgun"))
                        {
                            AttackWithOtherRanged(attackPower, mainHand, true);
                        }
                        else if (mainHand.Name.ToLowerInvariant().Contains("sickle")
                            || mainHand.Name.ToLowerInvariant().Contains("flail")
                            || mainHand.Name.ToLowerInvariant().Contains("whip"))
                        {
                            AttackWithMelee(attackPower);
                        }
                    }
                }
                else if (mainHand.Type == ItemTypeEnum.None)
                {
                    AttackWithMelee(attackPower);
                }
            }
            else if (attackPower.Attack.Skill != null)
            {
                if (Stats.TotalMana >= attackPower.Attack.Skill.ManaConsumption)
                {
                    AttackWithSkill(attackPower);
                }
            }
        }
    }

    private void AttackWithActiveSkill()
    {
        Stats.TotalMana -= Stats.ActiveSkill.ManaConsumption;
        Stats.Mana -= Stats.ActiveSkill.ManaConsumption;
        manaRecoveryCounter = Stats.TotalMana <= 0 ? Stats.TotalManaRecoveryTime * 2 : Stats.TotalManaRecoveryTime;
        attackCounter = Stats.ActiveSkill.AttackRecoveryTime;
        State.Add(StateEnum.Attacking);
        if (PlayerAttackType == PlayerAttackEnum.Left || PlayerAttackType == PlayerAttackEnum.ActiveSkill)
        {
            animationController.SetAttackingLeft(true);
            animationController.SetIsPreparingLeftAttack(false);
        }
        else
        {
            animationController.SetAttackingRight(true);
            animationController.SetIsPreparingRightAttack(false);
        }

        var playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 direction = playerPosition;

        var projectileGameObject = Instantiate(Stats.ActiveSkill.Projectile, GameObject.Find("Projectiles").transform);
        ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
        //projectileGameObject.transform.position = moveCollider.transform.position - new Vector3(pc.Offset.x, pc.Offset.y, 0);

        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
        pc.Setup(gameObject, rotation, Stats.ActiveSkill.Range);
        projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(Stats.ActiveSkill.Damage, PlayerAttackEnum.ActiveSkill, null, Stats.ActiveSkill);
        Stats.ActiveSkill = null;
    }

    private void AttackWithSkill(AttackPower attackPower)
    {
        Stats.TotalMana -= attackPower.Attack.Skill.ManaConsumption;
        Stats.Mana -= attackPower.Attack.Skill.ManaConsumption;
        manaRecoveryCounter = Stats.TotalMana <= 0 ? Stats.TotalManaRecoveryTime * 2 : Stats.TotalManaRecoveryTime;
        attackCounter = attackPower.Attack.Skill.AttackRecoveryTime;
        State.Add(StateEnum.Attacking);
        if (PlayerAttackType == PlayerAttackEnum.Left || PlayerAttackType == PlayerAttackEnum.ActiveSkill)
        {
            //animationController.SetAttackingLeft(true);
            animationController.SetIsPreparingLeftAttack(false);
        }
        else
        {
            //animationController.SetAttackingRight(true);
            animationController.SetIsPreparingRightAttack(false);
        }

        var playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 direction = playerPosition;

        var projectileGameObject = Instantiate(attackPower.Attack.Skill.Projectile, GameObject.Find("Projectiles").transform);
        ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
        //projectileGameObject.transform.position = moveCollider.transform.position - new Vector3(pc.Offset.x, pc.Offset.y, 0);

        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
        pc.Setup(gameObject, rotation, attackPower.Attack.Skill.Range);

        projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(attackPower.Attack.Skill.Damage, PlayerAttackType, null, attackPower.Attack.Skill);
    }

    private void AttackWithRanged(AttackPower attackPower, InventoryItem mainHand, InventoryItem offHand, ItemCategoryEnum itemCategory)
    {
        if (offHand.Type == ItemTypeEnum.Ammo
            && offHand.ItemCategory == itemCategory
            && offHand.Quantity.Value > 0)
        {
            Stats.TotalStamina -= attackPower.StaminaConsumption;
            Stats.Stamina -= attackPower.StaminaConsumption;
            staminaRecoveryCounter = Stats.TotalStamina <= 0 ? Stats.TotalStaminaRecoveryTime * 2 : Stats.TotalStaminaRecoveryTime;
            attackCounter = attackPower.AttackRecoveryTime;
            State.Add(StateEnum.Attacking);
            if (PlayerAttackType == PlayerAttackEnum.Left || PlayerAttackType == PlayerAttackEnum.ActiveSkill)
            {
                //animationController.SetAttackingLeft(true);
                animationController.SetIsPreparingLeftAttack(false);
            }
            else
            {
                //animationController.SetAttackingRight(true);
                animationController.SetIsPreparingRightAttack(false);
            }

            var playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 direction = playerPosition;

            var projectileGameObject = Instantiate(offHand.Projectile, GameObject.Find("Projectiles").transform);
            ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
            projectileGameObject.transform.position = playerPosition;

            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
            pc.Setup(gameObject, transform.rotation, mainHand.Range.Value);

            projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(attackPower.Damage, PlayerAttackType, attackPower.Attack.Item);
            offHand.Quantity.Value--;
            mainHand.Durability.Value--;
        }
    }

    private void AttackWithOtherRanged(AttackPower attackPower, InventoryItem mainHand, bool canAttackUnlimited = false)
    {
        if (mainHand.Quantity.Value > 0 || canAttackUnlimited)
        {
            Stats.TotalStamina -= attackPower.StaminaConsumption;
            Stats.Stamina -= attackPower.StaminaConsumption;
            staminaRecoveryCounter = Stats.TotalStamina <= 0 ? Stats.TotalStaminaRecoveryTime * 2 : Stats.TotalStaminaRecoveryTime;
            attackCounter = attackPower.AttackRecoveryTime;
            State.Add(StateEnum.Attacking);
            if (PlayerAttackType == PlayerAttackEnum.Left || PlayerAttackType == PlayerAttackEnum.ActiveSkill)
            {
                //animationController.SetAttackingLeft(true);
                animationController.SetIsPreparingLeftAttack(false);
            }
            else
            {
                //animationController.SetAttackingRight(true);
                animationController.SetIsPreparingRightAttack(false);
            }

            var playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 direction = playerPosition;

            var projectileGameObject = Instantiate(mainHand.Projectile, GameObject.Find("Projectiles").transform);
            ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
            projectileGameObject.transform.position = playerPosition;

            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
            pc.Setup(gameObject, transform.rotation, mainHand.Range.Value);

            projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(attackPower.Damage, PlayerAttackType, attackPower.Attack.Item);
            if (!canAttackUnlimited)
            {
                mainHand.Quantity.Value--;
            }
            mainHand.Durability.Value--;
        }
    }

    private void AttackWithMelee(AttackPower attackPower)
    {
        Stats.TotalStamina -= attackPower.StaminaConsumption;
        Stats.Stamina -= attackPower.StaminaConsumption;
        staminaRecoveryCounter = Stats.TotalStamina == 0 ? Stats.TotalStaminaRecoveryTime * 2 : Stats.TotalStaminaRecoveryTime;

        attackCounter = attackPower.AttackRecoveryTime;
        State.Add(StateEnum.Attacking);
        if (PlayerAttackType == PlayerAttackEnum.Left || PlayerAttackType == PlayerAttackEnum.ActiveSkill)
        {
            animationController.SetAttackingLeft(true);
            animationController.SetIsPreparingLeftAttack(false);
            var itemHolder = new List<GameObject>(GameObject.FindGameObjectsWithTag("LeftItemHolder")).Find(g => g.transform.IsChildOf(transform));
            var hitbox = itemHolder.transform.GetChild(0).GetChild(0);
            //hitbox.transform.position = new Vector3(hitbox.transform.position.x, -.5f, hitbox.transform.position.z);
            hitbox.gameObject.SetActive(true);
        }
        else
        {
            animationController.SetAttackingRight(true);
            animationController.SetIsPreparingRightAttack(false);
            var itemHolder = new List<GameObject>(GameObject.FindGameObjectsWithTag("RightItemHolder")).Find(g => g.transform.IsChildOf(transform));
            var hitbox = itemHolder.transform.GetChild(0).GetChild(0);
            //hitbox.transform.position = new Vector3(hitbox.transform.position.x, .5f, hitbox.transform.position.z);
            hitbox.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.X) && !State.Exists(s => s == StateEnum.Crouching) &&
            (other != null && other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<EnemyController>().State.Exists(s => s == StateEnum.Dead)
            || other.gameObject.CompareTag("HidingObject")) && !State.Exists(s => s == StateEnum.Hidden))
        {
            PlayerDrag(other.gameObject);
        }

        if (other.gameObject.CompareTag("HidingObject") && Input.GetKeyDown(KeyCode.E))
        {
            PlayerToggleHiding(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.X) && !State.Exists(s => s == StateEnum.Crouching) &&
            (other != null && other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<EnemyController>().State.Exists(s => s == StateEnum.Dead)
            || other.gameObject.CompareTag("HidingObject")) && !State.Exists(s => s == StateEnum.Hidden))
        {
            PlayerDrag(other.gameObject);
        }

        if (other.gameObject.CompareTag("HidingObject") && Input.GetKeyDown(KeyCode.E))
        {
            PlayerToggleHiding(other.gameObject);
        }
    }

    private void PlayerToggleHiding(GameObject hidingObject)
    {
        var hidingObjectController = hidingObject.GetComponent<ObjectController>();
        if (!State.Exists(s => s == StateEnum.Hidden) && hidingObjectController.EnemyContent.Count < hidingObjectController.MaxEnemyContent)
        {
            if (State.Exists(s => s == StateEnum.Dragging))
            {
                if (DraggedObject.CompareTag("Enemy"))
                {
                    var enemyController = DraggedObject.GetComponent<EnemyController>();
                    enemyController.HideInObject(hidingObject);
                    DraggedObject = null;
                }
                else
                {
                    DropAtPosition(transform.position);
                    HideInObject(hidingObject);
                }
                State.Remove(StateEnum.Dragging);
            }
            else
            {
                HideInObject(hidingObject);
            }
        }
    }

    public void RemoveCondition(ConditionEnum condition)
    {
        Conditions.Remove(condition);
    }

    public void AddCondition(ConditionEnum condition)
    {
        Conditions.Add(condition);
    }

    private void PlayerDrag(GameObject gameObject)
    {
        bool canDrag = true;
        if (gameObject.CompareTag("Enemy") && gameObject.GetComponent<EnemyController>().Hidespot != null)
        {
            canDrag = false;
        }
        if (gameObject.CompareTag("HidingObject") && Stats.TotalAttributes.Strength < (int)gameObject.GetComponent<ObjectController>().Size)
        {
            canDrag = false;
        }

        if (!State.Exists(s => s == StateEnum.Dragging) && canDrag)
        {
            DraggedObject = gameObject;
            if (gameObject.CompareTag("HidingObject"))
            {
                var positionColliderObject = DraggedObject.transform.GetChild(0).gameObject;
                var positionCollider = positionColliderObject.GetComponent<Collider2D>();
                positionCollider.isTrigger = true;
                gameManager.MapManager.ToggleObjectOnMap(DraggedObject.transform.GetChild(0).transform.position, TileType.Floor);
            }
            SetDraggedObjectPosition();

            State.Add(StateEnum.DraggingStart);
            State.Add(StateEnum.Dragging);
            InventoryManager.UpdateStats(Stats);
        }
    }

    private void PlayerDrop()
    {
        if (State.Exists(s => s == StateEnum.Dragging) && !State.Exists(s => s == StateEnum.DraggingStart))
        {
            var mpsp = new Point(0.6f, transform);
            var x = transform.position.x;
            var y = transform.position.y;
            var z = transform.position.z;
            Vector3 dropPosition = new Vector3(x + mpsp.X, y + mpsp.Y, z);
            DropAtPosition(dropPosition);

            State.Remove(StateEnum.Dragging);
            InventoryManager.UpdateStats(Stats);
        }
    }

    private void DropAtPosition(Vector3 dropPosition)
    {
        DraggedObject.transform.position = Vector3.Lerp(transform.position, dropPosition, Time.deltaTime);
        if (DraggedObject.CompareTag("HidingObject"))
        {
            var positionColliderObject = DraggedObject.transform.GetChild(0).gameObject;
            var positionCollider = positionColliderObject.GetComponent<Collider2D>();
            positionCollider.isTrigger = false;
            gameManager.MapManager.ToggleObjectOnMap(DraggedObject.transform.GetChild(0).transform.position, TileType.HideableObject);
        }
        DraggedObject = null;
    }

    private void SetDraggedObjectPosition()
    {
        float x = transform.position.x;
        float y = transform.position.y + 0.5f;
        float z = transform.position.z;
        DraggedObject.transform.position = new Vector3(x, y, z);
    }

    private void CheckIfHidingInObject()
    {
        GetOutOfHiding();
        if (State.Exists(s => s == StateEnum.Hidden))
        {
            State.Remove(StateEnum.Hiding);
        }
    }

    private void HideInObject(GameObject hidingObject)
    {
        Hidespot = hidingObject;

        Conditions.Remove(ConditionEnum.Visible);
        Conditions.Add(ConditionEnum.Invisible);
        State.Add(StateEnum.Hiding);
        State.Add(StateEnum.Hidden);

        var collider = gameObject.transform.GetChild(0).GetComponent<Collider2D>();
        collider.isTrigger = true;

        //mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b, 0.5f);

        //transform.position = Vector3.Lerp(transform.position, hidingObject.transform.position, Time.deltaTime);
        transform.position = Hidespot.transform.position;

        //animationController.SetMoveSpeed(0, 0);
    }

    private void GetOutOfHiding()
    {
        if (State.Exists(s => s == StateEnum.Hidden) && !State.Exists(s => s == StateEnum.Hiding))
        {
            Conditions.Remove(ConditionEnum.Invisible);
            Conditions.Add(ConditionEnum.Visible);
            State.Remove(StateEnum.Hiding);
            State.Remove(StateEnum.Hidden);

            var collider = gameObject.transform.GetChild(0).GetComponent<Collider2D>();
            collider.isTrigger = false;

            //mySpriteRenderer.color = new Color(mySpriteRenderer.color.r, mySpriteRenderer.color.g, mySpriteRenderer.color.b, 1f);

            var mpsp = new Point(0.6f, transform);
            var x = transform.position.x;
            var y = transform.position.y;
            var z = transform.position.z;
            Vector3 dropPosition = new Vector3(x + mpsp.X, y + mpsp.Y, z);
            transform.position = Vector3.Lerp(transform.position, dropPosition, Time.deltaTime);

            Hidespot = null;
        }
    }
}
