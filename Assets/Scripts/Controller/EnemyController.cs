using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Not called, used as template for enemies
public class EnemyController : MonoBehaviour
{
    #region Variables
    public Stats Stats;
    public List<StateEnum> State;
    public GameObject Hidespot = null;
    public GameObject DraggedObject = null;
    public bool InCombatFromDamage = false;
    public List<ConditionEnum> Conditions;
    public List<ConditionEnum> ConditionImmunities;
    public List<Vector3> PatrolPoints;
    public EnemyInventoryManager InventoryManager;
    public Vector3 KnockBackDifference = Vector2.zero;
    public Vector3 moveCollideOffset = Vector3.zero;
    public float StunTimer = 0;
    public bool IsHeavyAttack;
    public GameObject PlayerShadow;
    private bool isPlayerClose
    {
        get
        {
            return Vector3.Distance(myPosition + moveCollideOffset, playerPosition) <= 2.5;
        }
    }

    private float staminaRecoveryCounter = 0f;
    private float healthRecoveryCounter = 0f;
    private float manaRecoveryCounter = 0f;

    private GameManager gameManager;
    private AnimationController animationController;
    private PlayerController playerController;

    private Vector3 playerPosition;
    private Vector3 myPosition;
    [SerializeField]
    private float followTimer = 1f;
    private float followCounter = 1f;
    [SerializeField]
    private float knockbackTimer = .25f;
    private float knockbackCounter = .25f;
    private float stunCounter = 0;
    [SerializeField]
    private float waitToEndHuntTimer = 5f;
    private float huntCounter = 5f;
    private float suspicionTimer = 1f;
    private float suspicionCounter = 1f;
    private bool shouldStartHunt = false;
    [SerializeField]
    private float patrolTimer = 2f;
    private int lastPatrolPoint = 0;
    private float patrolCounter = 2f;
    private float inspectTimer = 2f;
    private float inspectCounter = 2f;
    private List<Location> path;

    private float attackCounter = 1.2f;

    private GameObject DeadEnemyInSight;
    private GameObject hitbox;

    private UIManager uiManager;
    private NavMeshAgent navMeshAgent;

    private FieldOfView fieldOfView;
    private float defaultViewDistance;
    private float defaultFoV;

    private float attackRange;
    #endregion Variables

    void Awake()
    {
        fieldOfView = transform.GetChild(1).GetComponent<FieldOfView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        uiManager = FindObjectOfType<UIManager>();
        Conditions = new List<ConditionEnum>() { ConditionEnum.Visible };
        ConditionImmunities = new List<ConditionEnum>();
        gameManager = FindObjectOfType<GameManager>();
        path = new List<Location>();
        animationController = new AnimationController(GetComponent<Animator>());
        playerController = FindObjectOfType<PlayerController>();
        Stats = new Stats(
            3f,
            0,
            new Attributes()
            {
                Strength = 0,
                Dexterity = 0,
                Constitution = 0,
                Intelligence = 0,
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
            40,
            40,
            0,
            120,
            5f,
            0,
            10,
            10,
            0);

        InventoryManager = GetComponent<EnemyInventoryManager>();

        // row, column
        if (PatrolPoints.Count == 0)
        {
            //PatrolPoints = new List<Vector3> {
            //    new Vector3(5, 0, -3)
            //};
            foreach (Transform pp in gameManager.PatrolPoints)
            {
                if (pp.gameObject.activeSelf)
                {
                    PatrolPoints.Add(pp.transform.position);
                }
            }
        }
    }

    public void SetupEnemyStats(Stats stats, Dictionary<string, int> skills, Dictionary<string, InventoryItem> inventory, List<Vector3> patrolPoints)
    {
        Stats = stats;
        InventoryManager.SetupInventory();

        // TODO: Assign random skills if no skill selected

        for (var i = 1; i < Stats.Level; i++)
        {
            Stats.ExperiencePoints += 10;
            Stats.StoredExperiencePoints += gameManager.LevelRequirements[i + 1];
            Stats.AvailableAttributePoints += Constants.EXPERIENCE_POINTS_PER_LEVEL;
            Stats.Health += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.MaxHealth += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.Stamina += Constants.STAMINA_POINTS_PER_LEVEL;
            Stats.MaxStamina += Constants.STAMINA_POINTS_PER_LEVEL;
            Stats.Mana += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.MaxMana += Constants.HEALTH_POINTS_PER_LEVEL;
            Stats.CriticalChance++;
        }
        for (var i = 0; i < stats.AvailableAttributePoints; i++)
        {
            int rand = UnityEngine.Random.Range(0, 5);
            switch (rand)
            {
                case 0:
                    Stats.Attributes.Strength++;
                    break;
                case 1:
                    Stats.Attributes.Dexterity++;
                    break;
                case 2:
                    Stats.Attributes.Constitution++;
                    break;
                case 3:
                    Stats.Attributes.Intelligence++;
                    break;
                case 4:
                    Stats.Attributes.Charisma++;
                    break;
                default:
                    break;
            }
        }

        if (skills.Keys.Count > 0)
        {
            foreach (string skillId in skills.Keys)
            {
                Skill skill = new Skill(gameManager.Skills[skillId]);
                skill.IsUnlocked = true;
                skill.LevelUp(skills[skillId]);

                Stats.Skills[ActivityEnum.Inactive].Add(skill);
                Stats.Skills[ActivityEnum.Active].Add(skill);
            }
        }
        else
        {
            while (Stats.StoredExperiencePoints > 0)
            {
                // TODO: Check this logic. It's not good because you might not get to level up most skills if you get all of them
                if (Stats.Skills[ActivityEnum.Inactive].Count >= gameManager.Skills.Count)
                {
                    break;
                }

                int rand = UnityEngine.Random.Range(0, gameManager.Skills.Count);
                int index = 0;
                Skill skillToGet = new Skill();
                foreach (var skillId in gameManager.Skills.Keys)
                {
                    if (index == rand)
                    {
                        skillToGet = gameManager.Skills[skillId];
                        break;
                    }
                    index++;
                }

                if (Stats.Attributes.Strength >= skillToGet.Requirements.Attributes.Strength
                        && Stats.Attributes.Dexterity >= skillToGet.Requirements.Attributes.Dexterity
                        && Stats.Attributes.Constitution >= skillToGet.Requirements.Attributes.Constitution
                        && Stats.Attributes.Intelligence >= skillToGet.Requirements.Attributes.Intelligence
                        && Stats.Attributes.Charisma >= skillToGet.Requirements.Attributes.Charisma
                        && Stats.StoredExperiencePoints >= skillToGet.Requirements.ExperienceCost)
                {
                    var idx = Stats.Skills[ActivityEnum.Inactive].FindIndex(s => s.Id == skillToGet.Id);
                    if (idx != -1)
                    {
                        Stats.Skills[ActivityEnum.Inactive][idx].LevelUp(Stats.Skills[ActivityEnum.Inactive][idx].Tier);
                        if (Stats.Skills[ActivityEnum.Active].Count < Mathf.CeilToInt((float)Stats.Level / 2)
                            && Stats.Skills[ActivityEnum.Active].Count < Constants.DEFAULT_MAX_ACTIVE_SPELL_SLOTS)
                        {
                            Stats.Skills[ActivityEnum.Active].Add(Stats.Skills[ActivityEnum.Inactive][idx]);
                        }
                    }
                    else
                    {
                        Skill skill = new Skill(skillToGet);
                        skill.IsUnlocked = true;
                        Stats.StoredExperiencePoints -= skill.Requirements.ExperienceCost;

                        Stats.Skills[ActivityEnum.Inactive].Add(skill);
                        if (Stats.Skills[ActivityEnum.Active].Count < Mathf.CeilToInt((float)Stats.Level / 2)
                            && Stats.Skills[ActivityEnum.Active].Count < Constants.DEFAULT_MAX_ACTIVE_SPELL_SLOTS)
                        {
                            Stats.Skills[ActivityEnum.Active].Add(skill);
                        }
                    }
                }
            }
        }

        if (inventory.Keys.Count > 0)
        {
            foreach (var location in inventory.Keys)
            {
                InventoryManager.SetItemByLocationString(location, inventory[location].Id, inventory[location].Tier);
            }
        }
        else
        {
            InventoryManager.SetupInventory(Stats.Level);
        }

        InventoryManager.UpdateStats(Stats);
        PatrolPoints = patrolPoints;

        if (Stats.MaxInventorySlots > 0)
        {
            for (var i = 0; i < Stats.MaxInventorySlots; i++)
            {
                if (!InventoryManager.Inventory.ContainsKey("inventory" + i))
                {
                    InventoryManager.Inventory.Add("inventory" + i, new InventoryItem());
                }
            }
        }
    }

    private void Start()
    {
        defaultViewDistance = Stats.ViewDistance;
        defaultFoV = Stats.FoV;
        fieldOfView.SetFoV(Stats.FoV);
        fieldOfView.SetViewDistance(Stats.ViewDistance);
        Stats.ExperiencePoints = Stats.ExperiencePoints == 0 ? 1 : Stats.ExperiencePoints;
        if (InventoryManager.Inventory == null)
        {
            InventoryManager.SetupInventory();
        }
    }

    private void Update()
    {
        if (State.Exists(s => s == StateEnum.Dead) || Stats.TotalHealth <= 0)
        {
            State.RemoveRange(0, State.Count);
            State.Add(StateEnum.Dead);
        }
        if (!State.Exists(s => s == StateEnum.Dead) || Stats.TotalHealth > 0)
        {
            playerPosition = playerController.transform.position;// GetChild(0).transform.position;
            myPosition = transform.position;

            CheckAttackState();
            SetAttackPower();

            var isWithinViewRange = playerController.State.Exists(s => s == StateEnum.Dead) ? false : Vector3.Distance(playerPosition, myPosition) <= Stats.ViewDistance;

            RecalculateStamina(true);
            
            CheckKnockback();
            CheckStaggered();
            CheckInspectingState();
            CheckCombatState();
            CheckHealth();

            if (!Conditions.Exists(c => c == ConditionEnum.Stunned) && !State.Exists(s => s == StateEnum.Inspecting))
            {
                // When seeing something suspicious for the first time
                if (PlayerShadow == null && fieldOfView.IsPlayerInSight
                    && !State.Exists(s => s == StateEnum.SawSomethingSuspicious)
                    && !State.Exists(s => s == StateEnum.InCombat))
                {
                    if (Vector3.Distance(myPosition, playerPosition) >= 3f)
                    {
                        State.Remove(StateEnum.Lookout);
                        State.Remove(StateEnum.Patrolling);
                        State.Add(StateEnum.SawSomethingSuspicious);
                        PlayerShadow = Instantiate(gameManager.InventoryItems["ammo1"].GameObject, playerPosition, Quaternion.identity);
                    }
                    else
                    {
                        StartHunt();
                    }
                }

                // Move the player shadow while enemy is suspicious
                if (State.Exists(s => s == StateEnum.SawSomethingSuspicious))
                {
                    State.Remove(StateEnum.Dragging);
                    DraggedObject = null;
                    DeadEnemyInSight = null;
                    //animationController.SetIsMoving(false);
                    suspicionCounter -= Time.deltaTime;
                    if (fieldOfView.IsPlayerInSight)
                    {
                        Destroy(PlayerShadow);
                        PlayerShadow = Instantiate((GameObject)Resources.Load("Prefabs/PlayerShadow"), playerPosition, Quaternion.identity);
                    }

                    if (suspicionCounter <= 0)
                    {
                        State.Remove(StateEnum.SawSomethingSuspicious);
                        suspicionCounter = suspicionTimer;
                    }
                }

                if (State.Exists(s => s == StateEnum.InCombat))
                {
                    State.Remove(StateEnum.Dragging);
                    DraggedObject = null;
                    DeadEnemyInSight = null;

                    //if in combat but not in range, go to player
                    if (fieldOfView.IsPlayerInSight && Vector3.Distance(myPosition, playerPosition) > attackRange)
                    {
                        huntCounter = waitToEndHuntTimer;
                        Destroy(PlayerShadow);
                        PlayerShadow = Instantiate((GameObject)Resources.Load("Prefabs/PlayerShadow"), playerPosition, Quaternion.identity);
                        //animationController.SetIsMoving(true);
                        Vector3 lookAt = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                        transform.LookAt(lookAt);
                        GoToPoint(playerPosition);
                    }
                    // if in combat and in range, attack the player
                    else if (fieldOfView.IsPlayerInSight && Vector3.Distance(myPosition, playerPosition) <= attackRange)
                    {
                        Destroy(PlayerShadow);
                        huntCounter = waitToEndHuntTimer;
                        Vector3 lookAt = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                        transform.LookAt(lookAt);
                        Attack();
                    }
                    // if in combat state but lost player
                    if (!fieldOfView.IsPlayerInSight)
                    {
                        huntCounter -= Time.deltaTime;
                        // go to player last seen position
                        if (PlayerShadow != null)
                        {
                            GoToPlayerLastSeenPosition();
                        }
                        // "follow" the player's footsteps for a while (while huntCounter is > 0)
                        else
                        {
                            // TODO: Search for player or go to random spots?
                            // TODO: Check playerPosition +/- offset
                            //MoveToPoint(gameManager.MapManager.GetGridPosition(playerPosition));
                            GoToPoint(playerPosition);
                            if (Vector3.Distance(myPosition, playerPosition) <= 1.5f)
                            {
                                huntCounter = 0;
                            }
                            //if (Vector3.Distance(myPosition, playerPosition) <= 1)
                            //{
                            //    Attack();
                            //    huntCounter = waitToEndHuntTimer;
                            //}
                        }
                    }
                }
                // if no combat and no suspicion state
                else if (!State.Exists(s => s == StateEnum.InCombat))
                {
                    if (!State.Exists(s => s == StateEnum.SawSomethingSuspicious))
                    {
                        fieldOfView.SetFoV(defaultFoV);

                        if (fieldOfView.IsPlayerInSight)
                        {
                            StartHunt();
                        }
                        // but the player shadow is still there
                        else if (PlayerShadow != null)
                        {
                            GoToPlayerLastSeenPosition();
                        }
                        else
                        {
                            //if (fieldOfView.IsDeadEnemyInSight || DeadEnemyInSight != null)
                            //{
                            //    if (DeadEnemyInSight == null)
                            //    {
                            //        //DeadEnemyInSight = fieldOfView.DeadEnemy;
                            //    }

                            //    if (path.Count > 0 && path[path.Count - 1].x != DeadEnemyInSight.transform.position.x
                            //        && path[path.Count - 1].y != DeadEnemyInSight.transform.position.y
                            //        && !State.Exists(s => s == StateEnum.Dragging))
                            //    {
                            //        path = new List<Location>();
                            //    }

                            //    if (Vector3.Distance(DeadEnemyInSight.transform.position, myPosition) >= 1.2)
                            //    {
                            //        var p = gameManager.MapManager.GetGridPosition(DeadEnemyInSight.transform.position);
                            //        //MoveToPoint(p);
                            //    }
                            //    else
                            //    {
                            //        if (!State.Exists(s => s == StateEnum.Dragging))
                            //        {
                            //            State.Add(StateEnum.Dragging);
                            //            DraggedObject = DeadEnemyInSight;
                            //        }

                            //        if (State.Exists(s => s == StateEnum.Dragging))
                            //        {
                            //            DraggedObject.layer = Constants.LAYER_DEFAULT;
                            //            SetDraggedObjectPosition();
                            //            var ec = DraggedObject.GetComponent<EnemyController>();
                            //            if (ec != null && ec.State.Exists(s => s == StateEnum.Dead))
                            //            {
                            //                if (Vector3.Distance(myPosition, gameManager.MapManager.GetWorldPosition(gameManager.MapManager.DeadSpot)) >= 1.2)
                            //                {
                            //                    //MoveToPoint(gameManager.MapManager.DeadSpot);
                            //                }
                            //                else
                            //                {
                            //                    EnemyEndDragAction();
                            //                    DeadEnemyInSight = null;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            if (!State.Exists(s => s == StateEnum.Lookout))
                            {
                                Patrol();
                            }
                            //}
                        }
                    }
                }
            }


        }
        else
        {
            ShowDeadState();
        }
    }

    private void GoToPoint(Vector3 point)
    {
        navMeshAgent.speed = Stats.TotalSpeed;
        navMeshAgent.SetDestination(point);
    }

    private void Patrol()
    {
        if (!State.Exists(s => s == StateEnum.Patrolling))
        {
            State.Add(StateEnum.Patrolling);
        }
        navMeshAgent.speed = Stats.TotalSpeed;
        Vector3 patrolPoint = PatrolPoints[lastPatrolPoint];
        navMeshAgent.SetDestination(patrolPoint);
        Vector2 thisXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 patrolPointXZ = new Vector2(patrolPoint.x, patrolPoint.z);
        if (Vector2.Distance(thisXZ, patrolPointXZ) <= .5)
        {
            StartCoroutine(EndLookoutCo());
            lastPatrolPoint++;
            if (lastPatrolPoint >= PatrolPoints.Count)
            {
                lastPatrolPoint = 0;
            }
        }
    }

    private IEnumerator EndLookoutCo()
    {
        State.Remove(StateEnum.Patrolling);
        if (!State.Exists(s => s == StateEnum.Lookout))
        {
            State.Add(StateEnum.Lookout);
        }
        yield return new WaitForSeconds(patrolTimer);
        State.Remove(StateEnum.Lookout);
    }

    private void CheckAttackState()
    {
        if (State.Exists(s => s == StateEnum.Attacking))
        {
            attackCounter -= Time.deltaTime;
            // The animation will be stuck if we don't check like this
            if (attackCounter - .2f <= 0)
            {
                //animationController.SetIsAttacking(false);
            }
            if (attackCounter <= 0)
            {
                //animationController.StopMove();
                Destroy(hitbox);
                State.Remove(StateEnum.Attacking);
            }
        }
    }

    private void CheckKnockback()
    {
        if (Conditions.Exists(c => c == ConditionEnum.KnockedBack))
        {
            var kbPosition = new Vector3(transform.position.x + KnockBackDifference.x, transform.position.y + KnockBackDifference.y, transform.position.z);

            if (!Conditions.Exists(c => c == ConditionEnum.Stunned))
            {
                Conditions.Add(ConditionEnum.Stunned);
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
    }

    private void CheckInspectingState()
    {
        if (State.Exists(s => s == StateEnum.Inspecting))
        {
            //animationController.SetIsMoving(false);
            inspectCounter -= Time.deltaTime;
            if (inspectCounter <= 0 || fieldOfView.IsPlayerInSight)
            {
                inspectCounter = inspectTimer;
                State.Remove(StateEnum.Inspecting);
                Debug.Log("End Inspecting");
            }
            if (fieldOfView.IsPlayerInSight)
            {
                State.Remove(StateEnum.Inspecting);
                if (!State.Exists(s => s == StateEnum.InCombat))
                {
                    StartHunt();
                }
            }
        }
    }

    private void CheckStaggered()
    {
        if (Conditions.Exists(c => c == ConditionEnum.Staggered))
        {
            Conditions.Remove(ConditionEnum.Staggered);
            //animationController.StopMove();
            //animationController.SetIsAttacking(false);
            attackCounter = 1.2f;
        }
    }

    private void CheckCombatState()
    {
        if (State.Exists(s => s == StateEnum.InCombat) && huntCounter <= 0)
        {
            State.Remove(StateEnum.InCombat);
            huntCounter = waitToEndHuntTimer;
            Debug.Log("End Hunt");
        }
    }

    private void CheckHealth()
    {
        if (Stats.TotalHealth <= Stats.TotalMaxHealth / 3)
        {
            if (!Conditions.Exists(c => c == ConditionEnum.Stunned) && !Conditions.Exists(c => c == ConditionEnum.Staggered)
            && !Conditions.Exists(c => c == ConditionEnum.KnockedBack && !State.Exists(s => s == StateEnum.Attacking)))
            {
                // TODO: Heal
            }
        }
    }

    private void GoToPlayerLastSeenPosition()
    {
        // go to shadow
        GoToPoint(PlayerShadow.transform.position);
        //MoveToPoint(gameManager.MapManager.GetGridPosition(PlayerShadow.transform.position));
        // once reached, start inspecting
        if (Vector3.Distance(myPosition, PlayerShadow.transform.position) <= 1.5f)
        {
            Destroy(PlayerShadow);
            fieldOfView.SetFoV(defaultFoV);
            if (!State.Exists(s => s == StateEnum.Inspecting))
            {
                State.Add(StateEnum.Inspecting);
            }
        }
    }

    private float GetAttackRange()
    {
        var attackRange = Stats.RightHandAttack.Attack.Item != null
                && Stats.RightHandAttack.Attack.Item.Range.Value != 0
                && Stats.RightHandAttack.Attack.Item.Durability.Value > 0
                ? Stats.RightHandAttack.Attack.Item.Range.Value
                : 1.2f;

        if (Stats.RightHandAttack.Attack.Item != null
            && Stats.RightHandAttack.Attack.Item.Type != ItemTypeEnum.None
            && (Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Bow || Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Crossbow
            ||
            (Stats.RightHandAttack.Attack.Item.ItemCategory == ItemCategoryEnum.Other
            && (Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("dart")
            || Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("sling")
            || Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("javelin")
            || Stats.RightHandAttack.Attack.Item.Name.ToLowerInvariant().Contains("blowgun"))
            )))
        {
            attackRange = Stats.RightHandAttack.Attack.Item.Range.Value + Helpers.GetPercentOfValue(Stats.RightHandAttack.Attack.Item.Range.Value, Stats.TotalAttributes.Dexterity);
        }
        else if (Stats.RightHandAttack.Attack.Skill != null)
        {
            attackRange = Stats.RightHandAttack.Attack.Skill.Range + Helpers.GetPercentOfValue(Stats.RightHandAttack.Attack.Skill.Range, Stats.TotalAttributes.Intelligence);
        }

        return attackRange;
    }

    private void SetAttackPower()
    {
        if (State.Exists(s => s == StateEnum.InCombat) && !State.Exists(s => s == StateEnum.Attacking))
        {
            List<Damage> playerResistances = new List<Damage>();
            foreach (DamageTypeEnum dmgType in playerController.Stats.Resistances.Keys)
            {
                var dmg = new Damage();
                dmg.Type = dmgType;
                dmg.Value = playerController.Stats.Resistances[dmgType];
                playerResistances.Add(dmg);
            }
            playerResistances.OrderBy(r => r.Value);

            Stats.RightHandAttack.Attack.Item = null;
            Stats.RightHandAttack.Attack.Skill = null;
            Stats.LeftHandAttack.Attack.Item = null;
            Stats.LeftHandAttack.Attack.Skill = null;
            // TODO: Also equip the best melee/ranged wpn in the correct hand
            InventoryItem bestMeleeWpn = InventoryManager.GetBestWeapon(Stats, playerResistances, AttackDistanceEnum.Close);
            InventoryItem bestRangedWpn = InventoryManager.GetBestWeapon(Stats, playerResistances, AttackDistanceEnum.Far);
            Skill bestCloseSkill = Stats.GetBestSkill(playerResistances, AttackDistanceEnum.Close);
            Skill bestRangedSkill = Stats.GetBestSkill(playerResistances, AttackDistanceEnum.Far);

            if (!isPlayerClose && Stats.Skills[ActivityEnum.Active].Count > 0)
            {
                if (Stats.TotalMana > 0 && Stats.PreferedFightStyle == PreferedFightStyleEnum.Skill)
                {
                    Stats.RightHandAttack.Attack.Skill = bestRangedSkill;
                }
                if (Stats.RightHandAttack.Attack.Skill == null)
                {
                    if (Stats.PreferedFightStyle == PreferedFightStyleEnum.Ranged)
                    {
                        var item = bestRangedWpn;
                        if (item != null)
                        {
                            Stats.RightHandAttack.Attack.Item = item;
                        }
                    }
                    if (Stats.RightHandAttack.Attack.Item == null)
                    {
                        if (Stats.TotalMana > bestRangedSkill.ManaConsumption)
                        {
                            Stats.RightHandAttack.Attack.Skill = bestRangedSkill;
                        }
                        if (Stats.RightHandAttack.Attack.Skill == null)
                        {
                            Stats.RightHandAttack.Attack.Item = bestRangedWpn;
                            if (Stats.RightHandAttack.Attack.Item == null)
                            {
                                if (Stats.TotalMana > bestCloseSkill.ManaConsumption)
                                {
                                    Stats.RightHandAttack.Attack.Skill = bestCloseSkill;
                                }
                                if (Stats.RightHandAttack.Attack.Skill == null)
                                {
                                    Stats.RightHandAttack.Attack.Item = bestMeleeWpn;
                                }
                            }
                        }
                    }
                }
            }
            else if (!isPlayerClose && Stats.Skills[ActivityEnum.Active].Count <= 0)
            {
                if (bestRangedWpn != null)
                {
                    Stats.RightHandAttack.Attack.Item = bestRangedWpn;
                }
                else
                {
                    Stats.RightHandAttack.Attack.Item = bestMeleeWpn;
                }
            }
            else if (isPlayerClose && Stats.Skills[ActivityEnum.Active].Count > 0)
            {
                if (Stats.TotalMana > 0 && Stats.PreferedFightStyle == PreferedFightStyleEnum.Skill)
                {
                    Stats.RightHandAttack.Attack.Skill = bestCloseSkill;
                }

                if (Stats.RightHandAttack.Attack.Skill == null)
                {
                    if (Stats.PreferedFightStyle == PreferedFightStyleEnum.Melee)
                    {
                        Stats.RightHandAttack.Attack.Item = bestMeleeWpn;
                    }

                    if (Stats.RightHandAttack.Attack.Item == null)
                    {
                        if (Stats.TotalMana >= bestCloseSkill.ManaConsumption)
                        {
                            Stats.RightHandAttack.Attack.Skill = bestCloseSkill;
                        }
                        if (Stats.RightHandAttack.Attack.Skill == null)
                        {
                            Stats.RightHandAttack.Attack.Item = bestMeleeWpn;
                            if (Stats.RightHandAttack.Attack.Item == null)
                            {
                                if (Stats.TotalMana >= bestRangedSkill.ManaConsumption)
                                {
                                    Stats.RightHandAttack.Attack.Skill = bestRangedSkill;
                                }
                                if (Stats.RightHandAttack.Attack.Skill == null)
                                {
                                    Stats.RightHandAttack.Attack.Item = bestRangedWpn;
                                }
                            }
                        }
                    }
                }
            }
            else if (isPlayerClose && Stats.Skills[ActivityEnum.Active].Count <= 0)
            {
                if (bestMeleeWpn != null)
                {
                    Stats.RightHandAttack.Attack.Item = bestMeleeWpn;
                }
                else
                {
                    Stats.RightHandAttack.Attack.Item = bestRangedWpn;
                }
            }

            if (Stats.RightHandAttack.Attack.Skill == null && Stats.RightHandAttack.Attack.Item == null)
            {
                Stats.RightHandAttack.Attack.Item = InventoryManager.GetFist(Stats);
            }

            Stats.LeftHandAttack.Attack.Item = InventoryManager.Inventory["leftHand"];
            InventoryManager.UpdateStats(Stats);
            attackRange = GetAttackRange();
        }
    }

    public void OnMouseDown()
    {
        if (playerController.State.Exists(s => s == StateEnum.Inspecting))
        {
            uiManager.InspectedEnemy = gameObject;
            uiManager.CloseEnemyInfo();
            uiManager.OpenEnemyInventory(InventoryManager, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckIfStashedAways(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //TODO If enemy stays too long stuck in object, do something about it
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ExitStealth(other);
    }

    public void Attack()
    {
        if (!State.Exists(s => s == StateEnum.Attacking))
        {
            if (Stats.RightHandAttack.Attack.Item != null)
            {
                InventoryItem mainHand = Stats.RightHandAttack.Attack.Item;
                InventoryItem offHand = Stats.LeftHandAttack.Attack.Item;
                if (mainHand.Type == ItemTypeEnum.Weapon)
                {
                    if (mainHand.Durability.Value <= 0)
                    {
                        // TODO: Switch weapons instead of drop?
                        if (InventoryManager.IsUsingAlternateWeapons)
                        {
                            InventoryManager.DropItem(InventoryLocationEnum.AlternateRightHand, 0, gameObject);
                        }
                        else
                        {
                            InventoryManager.DropItem(InventoryLocationEnum.RightHand, 0, gameObject);
                        }
                    }

                    if (Stats.TotalStamina >= Stats.RightHandAttack.StaminaConsumption && mainHand.Durability.Value > 0)
                    {
                        if (mainHand.ItemCategory == ItemCategoryEnum.Axe
                        || mainHand.ItemCategory == ItemCategoryEnum.Club
                        || mainHand.ItemCategory == ItemCategoryEnum.Dagger
                        || mainHand.ItemCategory == ItemCategoryEnum.Hammer
                        || mainHand.ItemCategory == ItemCategoryEnum.Spear
                        || mainHand.ItemCategory == ItemCategoryEnum.Sword)
                        {
                            AttackWithMelee();
                        }
                        else if (mainHand.ItemCategory == ItemCategoryEnum.Bow)
                        {
                            AttackWithRanged(mainHand, offHand, ItemCategoryEnum.BowAmmo);
                        }
                        else if (mainHand.ItemCategory == ItemCategoryEnum.Crossbow)
                        {
                            AttackWithRanged(mainHand, offHand, ItemCategoryEnum.CrossbowAmmo);
                        }
                        else if (mainHand.ItemCategory == ItemCategoryEnum.Other)
                        {
                            if (mainHand.Name.ToLowerInvariant().Contains("dart")
                                || mainHand.Name.ToLowerInvariant().Contains("javelin"))
                            {
                                AttackWithOtherRanged(mainHand);
                            }
                            else if (mainHand.Name.ToLowerInvariant().Contains("sling")
                                || mainHand.Name.ToLowerInvariant().Contains("blowgun"))
                            {
                                AttackWithOtherRanged(mainHand, true);
                            }
                            else if (mainHand.Name.ToLowerInvariant().Contains("sickle")
                                || mainHand.Name.ToLowerInvariant().Contains("flail")
                                || mainHand.Name.ToLowerInvariant().Contains("whip"))
                            {
                                AttackWithMelee();
                            }
                        }
                        else
                        {
                            Debug.Log("Enemy Fist");
                            AttackWithMelee();
                        }
                    }
                    else
                    {
                        Debug.Log("Enemy Fist");
                        AttackWithMelee();
                    }
                }
                else
                {
                    Debug.Log("Enemy Fist");
                    AttackWithMelee();
                }
            }
            else if (Stats.RightHandAttack.Attack.Skill != null)
            {
                if (Stats.TotalMana >= Stats.RightHandAttack.Attack.Skill.ManaConsumption)
                {
                    Stats.TotalMana -= Stats.RightHandAttack.Attack.Skill.ManaConsumption;
                    Stats.Mana -= Stats.RightHandAttack.Attack.Skill.ManaConsumption;
                    manaRecoveryCounter = Stats.TotalMana <= 0 ? Stats.TotalManaRecoveryTime * 2 : Stats.TotalManaRecoveryTime;
                    attackCounter = Stats.RightHandAttack.Attack.Skill.AttackRecoveryTime + Constants.ENEMY_ATTACK_RECOVERY;
                    if (!State.Exists(s => s == StateEnum.Attacking))
                    {
                        State.Add(StateEnum.Attacking);
                    }

                    Vector2 difference = GetDifference();
                    //animationController.SetLastMoveSpeed(difference.x, difference.y);

                    Vector2 direction = playerPosition - myPosition;
                    direction.Normalize();

                    var projectileGameObject = Instantiate(Stats.RightHandAttack.Attack.Skill.Projectile, GameObject.Find("Projectiles").transform);
                    ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
                    projectileGameObject.transform.position = myPosition - new Vector3(pc.Offset.x, pc.Offset.y, 0);

                    Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                    pc.Setup(gameObject, rotation, Stats.RightHandAttack.Attack.Skill.Range);

                    projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(Stats.RightHandAttack.Attack.Skill.Damage, PlayerAttackEnum.None, null, Stats.RightHandAttack.Attack.Skill);
                }
            }
        }
    }

    private void AttackWithOtherRanged(InventoryItem mainHand, bool canAttackUnlimited = false)
    {
        if (mainHand.Quantity.Value > 0 || canAttackUnlimited)
        {
            attackCounter = Stats.RightHandAttack.AttackRecoveryTime + Constants.ENEMY_ATTACK_RECOVERY;
            if (!State.Exists(s => s == StateEnum.Attacking))
            {
                State.Add(StateEnum.Attacking);
            }

            Vector2 difference = GetDifference();
            //animationController.SetLastMoveSpeed(difference.x, difference.y);

            Vector2 direction = playerPosition - myPosition;
            direction.Normalize();

            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            var projectileGameObject = Instantiate(mainHand.Projectile, GameObject.Find("Projectiles").transform);
            projectileGameObject.transform.position = myPosition;
            projectileGameObject.GetComponent<ProjectileController>().Setup(gameObject, rotation, mainHand.Range.Value);
            var r = UnityEngine.Random.Range(0, 100);
            IsHeavyAttack = r >= 75;
            projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(Stats.RightHandAttack.Damage, PlayerAttackEnum.None, Stats.RightHandAttack.Attack.Item);
            if (!canAttackUnlimited)
            {
                mainHand.Quantity.Value--;
            }
            mainHand.Durability.Value--;
        }
    }

    private void AttackWithRanged(InventoryItem mainHand, InventoryItem offHand, ItemCategoryEnum itemCategory)
    {
        if (offHand.Type == ItemTypeEnum.Ammo
            && offHand.ItemCategory == itemCategory
            && offHand.Quantity.Value > 0)
        {
            attackCounter = Stats.RightHandAttack.AttackRecoveryTime + Constants.ENEMY_ATTACK_RECOVERY;
            if (!State.Exists(s => s == StateEnum.Attacking))
            {
                State.Add(StateEnum.Attacking);
            }

            //Vector2 difference = GetDifference();
            ////animationController.SetLastMoveSpeed(difference.x, difference.y);

            //Vector2 direction = playerPosition - myPosition;
            //direction.Normalize();

            //Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            //var projectileGameObject = Instantiate(offHand.Projectile, GameObject.Find("Projectiles").transform);
            //projectileGameObject.transform.position = myPosition;
            //projectileGameObject.GetComponent<ProjectileController>().Setup(gameObject, rotation, mainHand.Range.Value);
            //var r = UnityEngine.Random.Range(0, 100);
            //IsHeavyAttack = r >= 75;
            //projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(Stats.RightHandAttack.Damage, PlayerAttackEnum.None, Stats.RightHandAttack.Attack.Item);


            var projectileGameObject = Instantiate(offHand.Projectile, GameObject.Find("Projectiles").transform);
            ProjectileController pc = projectileGameObject.GetComponent<ProjectileController>();
            projectileGameObject.transform.position = playerPosition;

            Vector3 direction = playerPosition - myPosition;
            direction.Normalize();

            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
            pc.Setup(gameObject, transform.rotation, mainHand.Range.Value);

            var r = UnityEngine.Random.Range(0, 100);
            IsHeavyAttack = r >= 75;
            projectileGameObject.GetComponent<HurtEnemy>().SetupHurtObject(Stats.RightHandAttack.Damage, PlayerAttackEnum.None, Stats.RightHandAttack.Attack.Item);

            offHand.Quantity.Value--;
            mainHand.Durability.Value--;
        }
    }

    private void AttackWithMelee()
    {
        attackCounter = Stats.RightHandAttack.AttackRecoveryTime + Constants.ENEMY_ATTACK_RECOVERY;
        if (!State.Exists(s => s == StateEnum.Attacking))
        {
            State.Add(StateEnum.Attacking);
            //animationController.SetIsAttacking(true);
        }

        Vector2 difference = GetDifference();
        hitbox = Instantiate((GameObject)Resources.Load("Prefabs/Hitbox"), transform);
        hitbox.transform.position = transform.position + transform.forward;
        hitbox.transform.rotation = transform.rotation;
        //animationController.SetLastMoveSpeed(difference.x, difference.y);

        Stats.TotalStamina -= Stats.RightHandAttack.StaminaConsumption;
        staminaRecoveryCounter = Stats.TotalStamina <= 0 ? Stats.TotalStaminaRecoveryTime * 2 : Stats.TotalStaminaRecoveryTime;
    }

    private Vector2 GetDifference()
    {
        var distanceToPlayer = playerController.transform.transform.position - transform.position;

        if (distanceToPlayer.x > 0.1f)
        {
            distanceToPlayer.x = 0.1f;
        }
        else if (distanceToPlayer.x < -0.1f)
        {
            distanceToPlayer.x = -0.1f;
        }
        else if (distanceToPlayer.x < 0.1f && distanceToPlayer.x > -0.1f)
        {
            distanceToPlayer.x = 0f;
        }
        if (distanceToPlayer.y > 0.1f)
        {
            distanceToPlayer.y = 0.1f;
        }
        else if (distanceToPlayer.y < -0.1f)
        {
            distanceToPlayer.y = -0.1f;
        }
        else if (distanceToPlayer.y < 0.1f && distanceToPlayer.y > -0.1f)
        {
            distanceToPlayer.y = 0f;
        }
        return distanceToPlayer;
    }

    public void ShowDeadState()
    {
        gameObject.layer = Constants.LAYER_SOLID_OBJECTS;
        if (fieldOfView != null)
        {
            Destroy(fieldOfView.gameObject);
        }

        if (PlayerShadow != null)
        {
            Destroy(PlayerShadow);
        }

        if (!State.Exists(s => s == StateEnum.Dead))
        {
            State.RemoveRange(0, State.Count);
            State.Add(StateEnum.Dead);
        }

        animationController.SetIsDead(true);
        gameManager.CombatEnemyList.Remove(gameObject);
    }

    public void EnemyEndDragAction()
    {
        if (DraggedObject != null)
        {
            DraggedObject.layer = Vector3.Distance(DraggedObject.transform.position, gameManager.MapManager.DeadSpot) <= 1.2 ? Constants.LAYER_SOLID_OBJECTS : Constants.LAYER_DEFAULT;
            DraggedObject.transform.position = myPosition;
            DraggedObject = null;
            State.Remove(StateEnum.Dragging);
        }
    }

    private void SetDraggedObjectPosition()
    {
        float x = transform.position.x;
        float y = transform.position.y + 0.5f;
        float z = transform.position.z;
        DraggedObject.transform.position = new Vector3(x, y, z);
        if (DraggedObject.CompareTag("HidingObject"))
        {
            var objectController = DraggedObject.GetComponentInParent<ObjectController>();
            foreach (var content in objectController.EnemyContent)
            {
                content.transform.position = new Vector3(x, y, z);
            }
        }
    }

    public void StartHunt()
    {
        Debug.Log("START HUNT");
        fieldOfView.SetFoV(360);
        State.Remove(StateEnum.Patrolling);
        State.Remove(StateEnum.Lookout);
        if (!State.Exists(s => s == StateEnum.InCombat))
        {
            State.Add(StateEnum.InCombat);
        }
        huntCounter = waitToEndHuntTimer;
    }

    public void CheckIfStashedAways(Collider2D other)
    {
        // TODO: if gameobject is dragged it means it cannot be "hidden"
        if (other.gameObject.CompareTag("HideableObject") && State.Exists(s => s == StateEnum.Dead))
        {
            var enemySprite = GetComponent<SpriteRenderer>();
            enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 0.5f);
            Conditions.Remove(ConditionEnum.Visible);
            if (!Conditions.Exists(c => c == ConditionEnum.Invisible))
            {
                Conditions.Add(ConditionEnum.Invisible);
            }
        }
    }

    public void HideInObject(GameObject hidingObject)
    {
        var hidingObjectController = hidingObject.GetComponent<ObjectController>();
        Hidespot = hidingObject;
        if (hidingObjectController.EnemyContent.Count == 0)
        {
            hidingObjectController.EnemyContent.Add(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void ExitStealth(Collider2D other)
    {
        if (other.gameObject.CompareTag("HideableObject"))
        {
            var enemySprite = GetComponent<SpriteRenderer>();
            enemySprite.color = new Color(enemySprite.color.r, enemySprite.color.g, enemySprite.color.b, 1f);
            Conditions.Remove(ConditionEnum.Invisible);
            if (!Conditions.Exists(c => c == ConditionEnum.Visible))
            {
                Conditions.Add(ConditionEnum.Visible);
            }
        }
    }

    public void RecalculatePath()
    {
        if (path.Count > 0)
        {
            path = new List<Location>();
            var startPos = gameManager.MapManager.GetGridPosition(myPosition);
            var endPos = PatrolPoints[lastPatrolPoint];
            path = gameManager.MapManager.FindPath(startPos, endPos);
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
                    if (Stats.TotalStamina > Stats.TotalMaxStamina)
                    {
                        Stats.TotalStamina = Stats.TotalMaxStamina;
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
                    if (Stats.TotalMana > Stats.TotalMaxMana)
                    {
                        Stats.TotalMana = Stats.TotalMaxMana;
                    }
                }
            }
            else
            {
                manaRecoveryCounter -= Time.deltaTime;
            }
        }
    }
}
