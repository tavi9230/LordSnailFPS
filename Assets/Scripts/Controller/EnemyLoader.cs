using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class EnemyLoader
{
    private static Dictionary<string, Enemy> enemies;

    public static Dictionary<string, Enemy> LoadEnemies()
    {
        enemies = new Dictionary<string, Enemy>();

        // TODO: Load items from xml
        TextAsset xmlFile = (TextAsset)Resources.Load("Settings/enemies");
        MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
        XmlReader reader = XmlReader.Create(assetStream);
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(reader);
            XmlNode allEnemies = xmlDoc.ChildNodes[1];
            for (var i = 0; i < allEnemies.ChildNodes.Count; i++)
            {
                switch (allEnemies.ChildNodes[i].Name)
                {
                    case "humanoid":
                        LoadHumanoids(allEnemies.ChildNodes[i]);
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

        return enemies;
    }

    private static void LoadHumanoids(XmlNode enemies)
    {
        for (var i = 0; i < enemies.ChildNodes.Count; i++)
        {
            switch (enemies.ChildNodes[i].Name)
            {
                case "human":
                    LoadEnemy(enemies.ChildNodes[i]);
                    break;
                default:
                    break;
            }
        }
    }

    private static void LoadEnemy(XmlNode itemNode)
    {
        for (var i = 0; i < itemNode.ChildNodes.Count; i++)
        {
            Enemy enemy = new Enemy();
            foreach (XmlNode node in itemNode.ChildNodes[i].ChildNodes)
            {
                switch (node.Name)
                {
                    case "id":
                        enemy.Id = node.InnerText;
                        break;
                    case "level":
                        enemy.Level = int.Parse(node.InnerText);
                        break;
                    case "name":
                        enemy.Name = node.InnerText;
                        break;
                    case "experience":
                        enemy.ExperiencePoints = int.Parse(node.InnerText);
                        break;
                    case "gameobject":
                        enemy.GameObject = (GameObject)Resources.Load(node.InnerText);
                        break;
                    case "speed":
                        enemy.Speed = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "stamina":
                        enemy.Stamina = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "health":
                        enemy.Health = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "mana":
                        enemy.Mana = int.Parse(node.InnerText);
                        break;
                    case "staminaRecoveryTime":
                        enemy.StaminaRecoveryTime = int.Parse(node.InnerText);
                        break;
                    case "healthRecoveryTime":
                        enemy.HealthRecoveryTime = int.Parse(node.InnerText);
                        break;
                    case "manaRecoveryTime":
                        enemy.ManaRecoveryTime = int.Parse(node.InnerText);
                        break;
                    case "inventorySlots":
                        enemy.InventorySlots = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "fov":
                        enemy.FoV = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "viewDistance":
                        enemy.ViewDistance = new MinMax()
                        {
                            DefaultMinValue = float.Parse(node.ChildNodes[0].InnerText),
                            DefaultMaxValue = float.Parse(node.ChildNodes[1].InnerText)
                        };
                        break;
                    case "criticalChance":
                        enemy.CriticalChance = int.Parse(node.InnerText);
                        break;
                    case "inventory":
                        enemy.Inventory = GetInventory(node.ChildNodes);
                        break;
                    case "attributes":
                        enemy.Attributes = GetAttributes(node.ChildNodes);
                        break;
                    case "resistances":
                        enemy.Resistances = GetResistances(node.ChildNodes);
                        break;
                    case "skills":
                        enemy.Skills = GetSkills(node.ChildNodes);
                        break;
                    case "storedExperience":
                        enemy.StoredExperience = int.Parse(node.InnerText);
                        break;
                    case "availableAttributePoints":
                        enemy.AvailableAttributePoints = int.Parse(node.InnerText);
                        break;
                    case "preferedFightStyle":
                        enemy.PreferedFightStyle = (PreferedFightStyleEnum)int.Parse(node.InnerText);
                        break;
                    default:
                        break;
                }
            }
            
            enemies.Add(enemy.Id, enemy);
        }
    }

    private static Dictionary<string, InventoryItem> GetInventory(XmlNodeList inventoryNodes)
    {
        Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
        foreach (XmlNode node in inventoryNodes)
        {
            if (node.ChildNodes.Count >= 2)
            {
                var i = new InventoryItem();
                i.Id = node.ChildNodes[0].InnerText;
                i.Tier = int.Parse(node.ChildNodes[1].InnerText);
                inventory.Add(node.Name, i);
            }
        }
        return inventory;
    }

    private static Attributes GetAttributes(XmlNodeList attributeNodes)
    {
        Attributes attributes = new Attributes();
        foreach (XmlNode node in attributeNodes)
        {
            switch (node.Name)
            {
                case "strength":
                    attributes.Strength = int.Parse(node.InnerText);
                    break;
                case "dexterity":
                    attributes.Dexterity = int.Parse(node.InnerText);
                    break;
                case "constitution":
                    attributes.Constitution = int.Parse(node.InnerText);
                    break;
                case "intelligence":
                    attributes.Intelligence = int.Parse(node.InnerText);
                    break;
                case "charisma":
                    attributes.Charisma = int.Parse(node.InnerText);
                    break;
                default:
                    break;
            }
        }
        return attributes;
    }

    private static Dictionary<DamageTypeEnum, float> GetResistances(XmlNodeList resistanceNodes)
    {
        Dictionary<DamageTypeEnum, float> resistances = new Dictionary<DamageTypeEnum, float>();
        foreach (XmlNode node in resistanceNodes)
        {
            switch (node.Name)
            {
                case "piercing":
                    resistances.Add(DamageTypeEnum.Piercing, float.Parse(node.InnerText));
                    break;
                case "slashing":
                    resistances.Add(DamageTypeEnum.Slashing, float.Parse(node.InnerText));
                    break;
                case "bludgeoning":
                    resistances.Add(DamageTypeEnum.Bludgeoning, float.Parse(node.InnerText));
                    break;
                case "fire":
                    resistances.Add(DamageTypeEnum.Fire, float.Parse(node.InnerText));
                    break;
                case "cold":
                    resistances.Add(DamageTypeEnum.Cold, float.Parse(node.InnerText));
                    break;
                case "lightning":
                    resistances.Add(DamageTypeEnum.Lightning, float.Parse(node.InnerText));
                    break;
                case "poison":
                    resistances.Add(DamageTypeEnum.Poison, float.Parse(node.InnerText));
                    break;
                default:
                    break;
            }

        }
        return resistances;
    }

    private static Dictionary<string, int> GetSkills(XmlNodeList skillNodes)
    {
        Dictionary<string, int> skills = new Dictionary<string, int>();
        foreach (XmlNode node in skillNodes)
        {
            skills.Add(node.ChildNodes[0].InnerText, int.Parse(node.ChildNodes[1].InnerText));
        }
        return skills;
    }
}
