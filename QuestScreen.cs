using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Xml;


public class QuestScreen : MonoBehaviour {

    public static DateTime activeQStart = new DateTime();
    


    // Gets the time since player started the quest
    public static int CheckTimeElapsed()
    {
        int timeElapsed;
        if (File.Exists("questStartTime.xml"))
        {
            XmlDocument startTime = new XmlDocument();
            startTime.Load("questStartTime.xml");
            if (startTime.DocumentElement.Attributes["StartTime"].Value != "")
                {
                    activeQStart = Convert.ToDateTime(startTime.DocumentElement.Attributes["StartTime"].Value);
                }
            
            if (activeQStart != DateTime.MinValue) { timeElapsed = Convert.ToInt16(DateTime.Now.Subtract(activeQStart).TotalMinutes); }
            else timeElapsed = 0;
        }
        else timeElapsed = 0;
        return timeElapsed;
    }

    //Checks if its time for reward
    public static bool CheckTimeReward()
    {
        int timeRemaining;

        timeRemaining = GetQuestDuration(QuestGenerator.quest[0].id) - CheckTimeElapsed();
        if (timeRemaining <= 0) { return true; }
        else return false;
    }

    // Makes changes to the RemainingTime Button for handing in Quests.
    public static void ChangeMainQuestButton()
    {
        if (CheckTimeReward())
        {
            GameObject.Find("RemainingTime").GetComponentInChildren<Text>().text = "Turn in the quest";
            GameObject.Find("RemainingTime").GetComponentInChildren<Image>().color = Color.green;
            GameObject.Find("RemainingTime").GetComponentInChildren<Button>().interactable = true;

        }
        else if (!CheckTimeReward() && File.Exists("questStartTime.xml"))
        {
            GameObject.Find("RemainingTime").GetComponentInChildren<Image>().color = Color.yellow;
            GameObject.Find("RemainingTime").GetComponentInChildren<Text>().text = (GetQuestDuration(QuestGenerator.quest[0].id) - CheckTimeElapsed()).ToString() + " min";
            GameObject.Find("RemainingTime").GetComponentInChildren<Button>().interactable = false;
        }
        else if (!File.Exists("questStartTime.xml"))
        {
            GameObject.Find("RemainingTime").GetComponentInChildren<Text>().text = "No active quest";
            GameObject.Find("RemainingTime").GetComponentInChildren<Image>().color = Color.red;
            GameObject.Find("RemainingTime").GetComponentInChildren<Button>().interactable = false;
        }
    }



    // Calculates time necessary to complete called Quest, based on the quest difficulty , Quests wait time can be skipped by editing the questStartTime.xml after starting a quest - for testing purposes of course
    static public int GetQuestDuration(int i)
    {
        int minutesRequired;
        double multiplier = 0.6;

        int questType = QuestGenerator.quest[i].questType;
        int enemyType = QuestGenerator.quest[i].enemyType;
        int envType = QuestGenerator.quest[i].envType;

        // Quest type conditions
        if (questType > 0 && questType <= 70) { multiplier = multiplier * 1.1; }
        else if (questType > 70 && questType <= 95) { multiplier = multiplier * 1.5; }
        else if (questType > 95 && questType <= 110) { multiplier = multiplier * 2; }
        else if (questType > 110 && questType <= 120) { multiplier = multiplier * 3; }

        // Enemy conditions
        if (enemyType > 0 && enemyType <= 2) { multiplier = multiplier * 1; }
        else if (enemyType > 2 && enemyType <= 4) { multiplier = multiplier * 1.25; }
        else if (enemyType == 5) { multiplier = multiplier * 1.5; }

        // Environment conditions
        if (envType < 0 && envType <= 3) { multiplier = multiplier * 1; }
        else if (envType > 3 && envType <= 5) { multiplier = multiplier * 1.15; }
        else if (envType == 6) { multiplier = multiplier * 1.3; }

        minutesRequired = Convert.ToInt16(Math.Floor(60 * multiplier));
        return minutesRequired;
    }
    

    // Changes values in description
    public void ShowValuesQuestDescription(int id)
    {
        GameObject.Find("TextCrowns").GetComponent<Text>().text = GetQuestCrowns(id).ToString();
        GameObject.Find("TextXp").GetComponent<Text>().text = GetQuestXp(id).ToString();
        GameObject.Find("TextKarma").GetComponent<Text>().text = GetQuestKarma(id).ToString();
        if (QuestGenerator.quest[id].karma == 1) { GameObject.Find("TextKarma").GetComponent<Text>().color = Color.blue; }
        else { GameObject.Find("TextKarma").GetComponent<Text>().color = Color.red; }

    }
    // Calculates XP reward
    public int GetQuestXp(int id)
    {
        int totalXP;
        double step;
        step = RunStartUpdate.thePlayer.level * 15 * (((double)GetQuestDuration(id) / (double)60) * 1.1);
        totalXP = Convert.ToInt16(Math.Floor(step));
        if (QuestGenerator.quest[id].bonusType == 1) { totalXP = Convert.ToInt16(totalXP * 1.1); }
        if (QuestGenerator.quest[id].bonusType == 2) { totalXP = Convert.ToInt16(totalXP * 1.2); }
        return totalXP;
    }

    // Calculates Karma reward
    public int GetQuestKarma(int id)
    {
        int playerKarma=0;
        XmlDocument kar = new XmlDocument();
        kar.Load("mainPlayer.xml");
        foreach (XmlNode extractKarma in kar.DocumentElement)
        {
            if (extractKarma.Attributes["ID"].Value == RunStartUpdate.thePlayer.id.ToString())
            {
                playerKarma = int.Parse(extractKarma.Attributes["Karma"].Value);
            }
        }
        
        int totalKarma;
        double step;
        step = 2.5 * (((double)GetQuestDuration(id) / (double)30));
        totalKarma = Convert.ToInt16(Math.Floor(step));
        if (QuestGenerator.quest[id].karma == 1 && playerKarma < 0) { totalKarma = totalKarma * 3; }
        else if (QuestGenerator.quest[id].karma == 2 && playerKarma >= 0) { totalKarma = -totalKarma * 3; }
        else if (QuestGenerator.quest[id].karma == 2 && playerKarma < 0) { totalKarma = -totalKarma; }
        return totalKarma;
    }



    // Calculates reward in Crowns
    public int GetQuestCrowns(int id)
    {
        int totalCrowns;
        double step;
        step = RunStartUpdate.thePlayer.level * 4 * (((double)GetQuestDuration(id) / (double)60) * 0.6);
        totalCrowns = Convert.ToInt16(Math.Floor(step));
        if (QuestGenerator.quest[id].bonusType == 1) { totalCrowns = Convert.ToInt16(totalCrowns * 1.1); }
        if (QuestGenerator.quest[id].bonusType == 3) { totalCrowns = Convert.ToInt16(totalCrowns * 1.2); }
        return totalCrowns;
    }

    // Creates item as a reward - SOME OTHER DAY
    public void CreateItemReward()
    {
        // get players main stat, level
    }

    


    // -------------------------------
    // Gives player the reward
    public void GrantReward()
    {
        int totalXp, totalCrowns, karma;
        int id = 0;
        



        //Deletion process

        XmlNode toDelete = null;
        XmlDocument file = new XmlDocument();
        file.Load("quests.xml");
        foreach (XmlNode element in file.DocumentElement)
        {

            if (element.Attributes["ID"].Value == 0.ToString())
            {
                toDelete = element;
                QuestGenerator.quest[0].bonusType = 0;
                QuestGenerator.quest[0].enemyType = 0;
                QuestGenerator.quest[0].envType = 0;
                QuestGenerator.quest[0].questType = 0;
                QuestGenerator.quest[0].id = 0;
                QuestGenerator.quest[0].active = 0;
                QuestGenerator.quest[0].karma = 0;
            }
        }
        toDelete.ParentNode.RemoveChild(toDelete);

        foreach (XmlNode element in file.DocumentElement)
        { 
            if (element.Attributes["Active"].Value == 1.ToString())
            {
                
                id = int.Parse(element.Attributes["ID"].Value);
                QuestGenerator.quest[id].active = 2;
                element.Attributes["Active"].Value = 2.ToString();                
            }
        }
        file.Save("quests.xml");
        File.Delete("questStartTime.xml");

        totalXp = GetQuestXp(id);
        totalCrowns = GetQuestCrowns(id);
        karma = GetQuestKarma(id);


        // Reward process
        if (FightScreen.victory == false) { totalXp = totalXp / 2; totalCrowns = totalCrowns / 2; }

        LevelUp(totalXp);
        ChangeInCurrency(totalCrowns, "Crowns");
        ChangeInKarma(karma);
        FinishQuest();


        GameObject.Find("Quest"+id).GetComponentInChildren<Text>().text = AssignQuest.QuestNameGenerator(id) + " (Completed)";
        GameObject.Find("Quest" + id).GetComponentInChildren<Button>().interactable = false;
        AssignQuest.ChangeTextColor(id);


    }

    // Increases count of finished quests after completion
    public static void FinishQuest()
    {
        XmlDocument file = new XmlDocument();
        file.Load("mainPlayer.xml");
        foreach (XmlNode node in file.DocumentElement)
        {
            if (node.Attributes["ID"].Value == RunStartUpdate.thePlayer.id.ToString())
            {
                node.Attributes["CompletedSQ"].Value = (int.Parse(node.Attributes["CompletedSQ"].Value) + 1).ToString();
            }
        }
        file.Save("mainPlayer.xml");
    }
    
    // When you hand in a quest you have to fight an enemy
    public void GoToFightQuest()
    {
        SceneManager.LoadScene("QuestFight");
    }

    // Change in Karma
    public static void ChangeInKarma(int karmaChange)
    {
        RunStartUpdate.thePlayer.karma = RunStartUpdate.thePlayer.karma + karmaChange;
        XmlDocument player = new XmlDocument();
        player.Load("mainPlayer.xml");
        foreach (XmlNode search in player.DocumentElement)
        {
            if (search.Attributes["ID"].Value == RunStartUpdate.thePlayer.id.ToString())
            {
                search.Attributes["Karma"].Value = RunStartUpdate.thePlayer.karma.ToString();
            }
        }
        player.Save("mainPlayer.xml");
    }

    // Change of Currency, adding or subtracting
    public static void ChangeInCurrency(int amount, string currency)
    {
        XmlDocument inventorySearch = new XmlDocument();
        inventorySearch.Load("inventory.xml");
        foreach (XmlNode item in inventorySearch.DocumentElement)
        {
            if (item.Attributes["ID"].Value == RunStartUpdate.thePlayer.id.ToString())
            {
                int total;
                total = int.Parse(item.Attributes[currency].Value) + amount;
                item.Attributes[currency].Value = total.ToString();
                if (currency == "Crowns") { RunStartUpdate.thePlayer.crowns = total; }
            }
        }
        inventorySearch.Save("inventory.xml");
    }

    // Level up method, ups a level if the received XP in the parameter is high enough
    public static void LevelUp(int addedXp)
    {
        double level;
        RunStartUpdate.thePlayer.xp = RunStartUpdate.thePlayer.xp + addedXp;
        level = Math.Floor(Math.Sqrt((double)RunStartUpdate.thePlayer.xp / (double)120))+1;
        RunStartUpdate.thePlayer.level = Convert.ToInt32(level);

        XmlDocument save = new XmlDocument();
        save.Load("mainPlayer.xml");
        foreach (XmlNode write in save.DocumentElement)
        {
            if (write.Attributes["ID"].Value == RunStartUpdate.thePlayer.id.ToString())
            {
                string previousLevel = write.Attributes["Lvl"].Value;
                // if the level before is smaller than the new one, then give the player attribute and talent point
                if (int.Parse(previousLevel) < RunStartUpdate.thePlayer.level)
                {
                    write.Attributes["AtrPt"].Value = (int.Parse(write.Attributes["AtrPt"].Value) + 3).ToString();
                    if (RunStartUpdate.thePlayer.level >= 5) { write.Attributes["TalentPt"].Value = (int.Parse(write.Attributes["TalentPt"].Value) + 1).ToString(); }
                }
                write.Attributes["Lvl"].Value = RunStartUpdate.thePlayer.level.ToString();
                write.Attributes["XP"].Value = RunStartUpdate.thePlayer.xp.ToString();
            }
        }
        save.Save("mainPlayer.xml");
    }


    // Accepts Quest
    public void AcceptQuest()
    {
        int id = AssignQuest.clickedQuestID;
        XmlDocument file = new XmlDocument();
        file.Load("quests.xml");
        if (QuestGenerator.quest[id].bonusType != 0)
        {
            foreach (XmlNode element in file.DocumentElement)
            {
                if (element.Attributes["ID"].Value == id.ToString())
                {
                    element.Attributes["Active"].Value = 1.ToString();

                    XmlNode copy = element.CloneNode(true);
                    copy.Attributes["ID"].Value = "0";
                    file.DocumentElement.PrependChild(copy);

                    QuestGenerator.quest[0].bonusType = int.Parse(element.Attributes["Bonus"].Value);
                    QuestGenerator.quest[0].enemyType = int.Parse(element.Attributes["Enemy"].Value);
                    QuestGenerator.quest[0].envType = int.Parse(element.Attributes["Env"].Value);
                    QuestGenerator.quest[0].questType = int.Parse(element.Attributes["Type"].Value);
                    QuestGenerator.quest[0].id = int.Parse(element.Attributes["ID"].Value);
                    QuestGenerator.quest[0].active = 1;
                    QuestGenerator.quest[0].karma = int.Parse(element.Attributes["Karma"].Value);
                    QuestGenerator.quest[id].active = 1;
                }
            }
            AssignQuest.ChangeTextColor(id);
            file.Save("quests.xml");
            ButtonsInteractable();
            SaveActiveQuestTime();
        }
    }

    // Saves the time, when player started the quest
    public void SaveActiveQuestTime()
    {
        string timeNow = DateTime.Now.ToString();
        XmlWriter saveTime = XmlWriter.Create("questStartTime.xml");
        saveTime.WriteStartDocument();
        saveTime.WriteStartElement("Time");
        saveTime.WriteAttributeString("StartTime", timeNow);
        saveTime.WriteEndElement();
        saveTime.WriteEndDocument();
        saveTime.Close();
    }

    // Make Accept and Cancel interactable
    public static void ButtonsInteractable()
    {
        if (QuestGenerator.quest[0].bonusType > 0)
        {
            GameObject.Find("ButtonAccept").GetComponentInChildren<Text>().text = "You are busy";
            GameObject.Find("ButtonAccept").GetComponentInChildren<Button>().interactable = false;
            GameObject.Find("ButtonCancel").GetComponentInChildren<Button>().interactable = true;

        }
        else
        {
            GameObject.Find("ButtonAccept").GetComponentInChildren<Text>().text = "Accept";
            GameObject.Find("ButtonAccept").GetComponentInChildren<Button>().interactable = true;
            GameObject.Find("ButtonCancel").GetComponentInChildren<Button>().interactable = false;
        }
    }

    // Cancels current active quest
    public void CancelQuest()
    {
        int id = 0;
        XmlNode toDelete = null;
        XmlDocument file = new XmlDocument();
        file.Load("quests.xml");
         foreach (XmlNode element in file.DocumentElement)
         {
            if (element.Attributes["ID"].Value == 0.ToString() && toDelete == null)
            {
                toDelete = element;
            }
            else if (element.Attributes["Active"].Value == 1.ToString() && element.Attributes["ID"].Value != 0.ToString())
            {
                element.Attributes["Active"].Value = 0.ToString();

                id = int.Parse(element.Attributes["ID"].Value);
                QuestGenerator.quest[0].bonusType = 0;
                QuestGenerator.quest[0].enemyType = 0;
                QuestGenerator.quest[0].envType = 0;
                QuestGenerator.quest[0].questType = 0;
                QuestGenerator.quest[0].id = 0;
                QuestGenerator.quest[0].active = 0;
                QuestGenerator.quest[0].karma = 0;
                QuestGenerator.quest[id].active = 0;
            }
            
         }
        toDelete.ParentNode.RemoveChild(toDelete);
        AssignQuest.ChangeTextColor(id); 
        file.Save("quests.xml");
        ButtonsInteractable();
        File.Delete("questStartTime.xml");
    }
}
