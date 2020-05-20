using UnityEngine;
using UnityEngine.EventSystems;

public class SkillStatus : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerController playerController;
    private UIManager uiManager;
    private Skill hoveredSkill;

    public void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void Update()
    {
        if (transform.parent.name.Contains("ActiveSkillSlot")
            && hoveredSkill != null
            && hoveredSkill.IsUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AddSkillToHotbar(KeyCode.Alpha1, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AddSkillToHotbar(KeyCode.Alpha2, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                AddSkillToHotbar(KeyCode.Alpha3, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                AddSkillToHotbar(KeyCode.Alpha4, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                AddSkillToHotbar(KeyCode.Alpha5, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                AddSkillToHotbar(KeyCode.Alpha6, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                AddSkillToHotbar(KeyCode.Alpha7, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                AddSkillToHotbar(KeyCode.Alpha8, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                AddSkillToHotbar(KeyCode.Alpha9, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                AddSkillToHotbar(KeyCode.Alpha0, hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.F1))
            {
                playerController.Stats.LeftHandAttack.Attack.Replace(hoveredSkill);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                playerController.Stats.RightHandAttack.Attack.Replace(hoveredSkill);
            }
        }
    }

    public Skill skill { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left
            && skill.Tier < Constants.TIER_MAX_LEVEL
            && playerController.Stats.StoredExperiencePoints >= skill.Requirements.ExperienceCost
            && playerController.Stats.TotalAttributes.Charisma >= skill.Requirements.Attributes.Charisma
            && playerController.Stats.TotalAttributes.Constitution >= skill.Requirements.Attributes.Constitution
            && playerController.Stats.TotalAttributes.Dexterity >= skill.Requirements.Attributes.Dexterity
            && playerController.Stats.TotalAttributes.Intelligence >= skill.Requirements.Attributes.Intelligence
            && playerController.Stats.TotalAttributes.Strength >= skill.Requirements.Attributes.Strength)
        {
            var ps = playerController.Stats.Skills[ActivityEnum.Inactive].Find(s => s.Id == skill.Id);
            playerController.Stats.StoredExperiencePoints -= ps.Requirements.ExperienceCost;
            ps.Requirements.ExperienceCost *= 2;

            if (!ps.IsUnlocked)
            {
                ps.IsUnlocked = true;
            }
            else
            {
                ps.Tier++;
            }

            uiManager.RefreshInventory();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var ps = playerController.Stats.Skills[ActivityEnum.Inactive].Find(s => s.Id == skill.Id);
            if (ps.IsUnlocked
                && playerController.Stats.Skills[ActivityEnum.Active].Count < Mathf.CeilToInt((float)playerController.Stats.Level / 2)
                && playerController.Stats.Skills[ActivityEnum.Active].Count < Constants.DEFAULT_MAX_ACTIVE_SPELL_SLOTS)
            {
                if (!playerController.Stats.Skills[ActivityEnum.Active].Exists(st => st.Id == ps.Id))
                {
                    playerController.Stats.Skills[ActivityEnum.Active].Add(ps);
                }
            }
            else
            {
                Debug.Log("Not enought spell slots!");
            }
            uiManager.RefreshInventory();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveredSkill = playerController.Stats.Skills[ActivityEnum.Inactive].Find(s => s.Id == skill.Id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveredSkill = null;
    }

    private void AddSkillToHotbar(KeyCode kc, Skill skill)
    {
        playerController.Hotbar[kc] = new HotbarItem()
        {
            Skill = new Skill(skill)
        };
        playerController.Hotbar[kc].Skill.IsUnlocked = true;
        uiManager.ResetHotbar();
    }

    public void SetSkill(Skill skill)
    {
        this.skill = new Skill(skill);
    }
}