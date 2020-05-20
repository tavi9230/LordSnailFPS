using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public static class SkillLoader
{
    private static Dictionary<string, Skill> skills;

    public static Dictionary<string, Skill> LoadSkills()
    {
        skills = new Dictionary<string, Skill>();

        // TODO: Load items from xml
        TextAsset xmlFile = (TextAsset)Resources.Load("Settings/skills");
        MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
        XmlReader reader = XmlReader.Create(assetStream);
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(reader);
            XmlNode allSkills = xmlDoc.ChildNodes[1];
            for (var i = 0; i < allSkills.ChildNodes.Count; i++)
            {
                switch (allSkills.ChildNodes[i].Name)
                {
                    case "spells":
                        LoadSpells(allSkills.ChildNodes[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error loading " + xmlFile.name + ":\n" + ex);
        }
        finally
        {
            Debug.Log(xmlFile.name + " loaded");
        }

        return skills;
    }

    private static void LoadSpells(XmlNode spells)
    {
        for (var i = 0; i < spells.ChildNodes.Count; i++)
        {
            switch (spells.ChildNodes[i].Name)
            {
                case "fire":
                    LoadSkill(spells.ChildNodes[i]);
                    break;
                case "cold":
                    LoadSkill(spells.ChildNodes[i]);
                    break;
                case "lightning":
                    LoadSkill(spells.ChildNodes[i]);
                    break;
                case "poison":
                    LoadSkill(spells.ChildNodes[i]);
                    break;
                default:
                    break;
            }
        }
    }

    private static void LoadSkill(XmlNode itemNode)
    {
        for (var i = 0; i < itemNode.ChildNodes.Count; i++)
        {
            Skill skill = new Skill();
            foreach (XmlNode node in itemNode.ChildNodes[i].ChildNodes)
            {
                switch (node.Name)
                {
                    case "id":
                        skill.Id = node.InnerText;
                        break;
                    case "tier":
                        skill.Tier = int.Parse(node.InnerText);
                        break;
                    case "name":
                        skill.Name = node.InnerText;
                        break;
                    case "description":
                        skill.Description = node.InnerText;
                        break;
                    case "skillType":
                        skill.Type = (SkillTypeEnum)int.Parse(node.InnerText);
                        break;
                    case "gameobject":
                        skill.GameObject = (GameObject)Resources.Load(node.InnerText);
                        break;
                    case "damage":
                        skill.Damage = GetResistances(node);
                        break;
                    case "range":
                        skill.Range = float.Parse(node.InnerText);
                        break;
                    case "requirements":
                        var attr = new Attributes();
                        float exp = 0;
                        for (var j = 0; j < node.ChildNodes.Count; j++)
                        {
                            switch (node.ChildNodes[j].Name)
                            {
                                case "experience":
                                    exp = float.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                case "intelligence":
                                    attr.Intelligence = int.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                case "strength":
                                    attr.Strength = int.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                case "dexterity":
                                    attr.Dexterity = int.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                case "constitution":
                                    attr.Constitution = int.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                case "charisma":
                                    attr.Charisma = int.Parse(node.ChildNodes[j].InnerText);
                                    break;
                                default:
                                    break;
                            }

                        }
                        skill.Requirements = new Requirements()
                        {
                            Attributes = attr,
                            ExperienceCost = exp
                        };
                        break;
                    case "projectile":
                        skill.Projectile = (GameObject)Resources.Load(node.InnerText);
                        break;
                    case "attackRecoveryTime":
                        skill.AttackRecoveryTime = float.Parse(node.InnerText);
                        break;
                    case "manaConsumption":
                        skill.ManaConsumption = float.Parse(node.InnerText);
                        break;
                    case "chargeSpeed":
                        skill.ChargeSpeed = float.Parse(node.InnerText);
                        break;
                    default:
                        break;
                }
            }

            skills.Add(skill.Id, skill);
        }
    }

    private static Dictionary<DamageTypeEnum, Damage> GetResistances(XmlNode resistancesNode)
    {
        Dictionary<DamageTypeEnum, Damage> resistances = new Dictionary<DamageTypeEnum, Damage>()
        {
            {DamageTypeEnum.Piercing, new Damage() { Type=DamageTypeEnum.Piercing } },
            {DamageTypeEnum.Slashing, new Damage() { Type=DamageTypeEnum.Slashing }},
            {DamageTypeEnum.Bludgeoning, new Damage() { Type=DamageTypeEnum.Bludgeoning }},
            {DamageTypeEnum.Fire, new Damage() { Type=DamageTypeEnum.Fire }},
            {DamageTypeEnum.Cold, new Damage() { Type=DamageTypeEnum.Cold }},
            {DamageTypeEnum.Lightning, new Damage() { Type=DamageTypeEnum.Lightning }},
            {DamageTypeEnum.Poison, new Damage() { Type=DamageTypeEnum.Poison }},
        };
        foreach (XmlNode node in resistancesNode.ChildNodes)
        {
            switch (node.Name)
            {
                case "piercing":
                    resistances[DamageTypeEnum.Piercing].Type = DamageTypeEnum.Piercing;
                    resistances[DamageTypeEnum.Piercing].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Piercing].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "slashing":
                    resistances[DamageTypeEnum.Slashing].Type = DamageTypeEnum.Slashing;
                    resistances[DamageTypeEnum.Slashing].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Slashing].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "bludgeoning":
                    resistances[DamageTypeEnum.Bludgeoning].Type = DamageTypeEnum.Bludgeoning;
                    resistances[DamageTypeEnum.Bludgeoning].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Bludgeoning].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "fire":
                    resistances[DamageTypeEnum.Fire].Type = DamageTypeEnum.Fire;
                    resistances[DamageTypeEnum.Fire].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Fire].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "cold":
                    resistances[DamageTypeEnum.Cold].Type = DamageTypeEnum.Cold;
                    resistances[DamageTypeEnum.Cold].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Cold].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "lightning":
                    resistances[DamageTypeEnum.Lightning].Type = DamageTypeEnum.Lightning;
                    resistances[DamageTypeEnum.Lightning].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Lightning].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                case "poison":
                    resistances[DamageTypeEnum.Poison].Type = DamageTypeEnum.Poison;
                    resistances[DamageTypeEnum.Poison].MinValue = int.Parse(node.ChildNodes[0].InnerText);
                    resistances[DamageTypeEnum.Poison].MaxValue = int.Parse(node.ChildNodes[1].InnerText);
                    break;
                default:
                    break;
            }
        }
        return resistances;
    }
}
