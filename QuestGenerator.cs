using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;



public class Quest
{
    public static System.Random rnd = new System.Random();
    public int questType, enemyType, envType, bonusType, id, active, karma;
    
    // Actually generates random attributes for the quests in the array
    public void SetRandomStats(int i, int x)
    {
        questType = rnd.Next(1, 120);
        enemyType = rnd.Next(1, 5);
        envType = rnd.Next(1, 6);
        bonusType = rnd.Next(1, 5);
        id = i;
        active = 0;
        if (i > 0 && i <= x / 2) { karma = 1; }
        else karma = 2;
    }
}




public class QuestGenerator : MonoBehaviour {
    public static int nmbrOfQuests = 12;
    public static Quest[] quest = new Quest[13];
    public static DateTime now = DateTime.Now;
    
    // Fills the quest array
    private void Start()
    {
        int i;
        for(i=0; i<13; i++)
        {
            quest[i] = new Quest();
        }
    }



    // Copies active quest over to new quest file
    public static void CopyAQuest()
    {
        if (File.Exists("quests.xml") && quest[0].bonusType == 0)
        {
            XmlDocument questsRead = new XmlDocument();
            questsRead.Load("quests.xml");
            foreach (XmlElement node in questsRead.DocumentElement)
            {
                if (node.Attributes["Active"].Value == "1")
                {
                    quest[0].bonusType = int.Parse(node.GetAttribute("Bonus"));
                    quest[0].questType = int.Parse(node.GetAttribute("Type"));
                    quest[0].enemyType = int.Parse(node.GetAttribute("Enemy"));
                    quest[0].envType = int.Parse(node.GetAttribute("Env"));
                    quest[0].id = 0;
                    quest[0].active = int.Parse(node.GetAttribute("Active"));
                    quest[0].karma = int.Parse(node.GetAttribute("Karma"));

                    XmlNode copy;
                    copy = node.CloneNode(true);
                    copy.Attributes["ID"].Value = "0";
                    questsRead.DocumentElement.PrependChild(copy);
                }
            }
            questsRead.Save("quests.xml");
        }
    }


    // Makes x number of Quests and saves them into an XML file for future reference and saves them into an array
    public static void MakeNewQuests(int x)
    {
        string timeNow = now.ToString();
        XmlWriter nQuests = XmlWriter.Create("quests.xml");
        nQuests.WriteStartDocument();
        nQuests.WriteStartElement("Quests");
        nQuests.WriteAttributeString("timeGenerated", timeNow);

        // If there is an active quest going on, it copies it over as the 13th quest so the player doesn't lose the reward.
        if (quest[0].bonusType > 0)
        {
            nQuests.WriteStartElement("Quest");
            nQuests.WriteAttributeString("Karma", quest[0].karma.ToString());
            nQuests.WriteAttributeString("Bonus", quest[0].bonusType.ToString());
            nQuests.WriteAttributeString("Type", quest[0].questType.ToString());
            nQuests.WriteAttributeString("Enemy", quest[0].enemyType.ToString());
            nQuests.WriteAttributeString("Env", quest[0].envType.ToString());
            nQuests.WriteAttributeString("ID", 0.ToString());
            nQuests.WriteAttributeString("Active", "1");
            nQuests.WriteEndElement();
        }
        // Load up the array with fresh quests
        for (int i = 1; i <= x; i++)
        {
            quest[i].SetRandomStats(i, x);

            nQuests.WriteStartElement("Quest");
            if (i >= 1 && i <= (x / 2)) { nQuests.WriteAttributeString("Karma", "1"); }
            else { nQuests.WriteAttributeString("Karma", "2"); }
            nQuests.WriteAttributeString("Bonus", quest[i].bonusType.ToString());
            nQuests.WriteAttributeString("Type", quest[i].questType.ToString());
            nQuests.WriteAttributeString("Enemy", quest[i].enemyType.ToString());
            nQuests.WriteAttributeString("Env", quest[i].envType.ToString());
            nQuests.WriteAttributeString("ID", i.ToString());
            nQuests.WriteAttributeString("Active", "0");
            nQuests.WriteEndElement();
        }
        nQuests.WriteEndElement();
        nQuests.WriteEndDocument();
        nQuests.Close();
    }



    //Check time when the quests were generated, returns if quests need to be generated. They generate every 6 hrs
    public static bool CheckTimeGenerated()
    {
        DateTime questsTimeGenerated;
        if (File.Exists("quests.xml")){
            
                XmlDocument file = new XmlDocument();
                file.Load("quests.xml");

                XmlElement root = file.DocumentElement;
                if (root.GetAttribute("timeGenerated") != "")
                {
                    questsTimeGenerated = Convert.ToDateTime(root.GetAttribute("timeGenerated"));
                }
                else { questsTimeGenerated = DateTime.MinValue; }
            
            if ((now > questsTimeGenerated) && (questsTimeGenerated.Hour >= 0 && questsTimeGenerated.Hour < 6) && ((now.DayOfYear > questsTimeGenerated.DayOfYear) || (now.Hour >= 6))) { return true; }
            else if ((now > questsTimeGenerated) && (questsTimeGenerated.Hour >= 6 && questsTimeGenerated.Hour < 12) && ((now.DayOfYear > questsTimeGenerated.DayOfYear) || (now.Hour >= 12))) { return true; }
            else if ((now > questsTimeGenerated) && (questsTimeGenerated.Hour >= 12 && questsTimeGenerated.Hour < 18) && ((now.DayOfYear > questsTimeGenerated.DayOfYear) || (now.Hour >= 18))) { return true; }
            else if ((now > questsTimeGenerated) && (questsTimeGenerated.Hour >= 18 && questsTimeGenerated.Hour < 24) && (now.DayOfYear > questsTimeGenerated.DayOfYear)) { return true; }
            else { return false; }
            
        }
        else { return true; }
        
    }



    // Check if quests are in memory, if not load them
    public static void CheckMemoryQ()
    {
        if (File.Exists("quests.xml") && quest[1].bonusType == 0)
        {
            XmlDocument file = new XmlDocument();
            file.Load("quests.xml");
            foreach (XmlElement questr in file.DocumentElement)
            {
                int id = int.Parse(questr.Attributes["ID"].Value);
                quest[id].bonusType = int.Parse(questr.Attributes["Bonus"].Value);
                quest[id].enemyType = int.Parse(questr.Attributes["Enemy"].Value);
                quest[id].envType = int.Parse(questr.Attributes["Env"].Value);
                quest[id].questType = int.Parse(questr.Attributes["Type"].Value);
                quest[id].id = int.Parse(questr.Attributes["ID"].Value);
                quest[id].active = int.Parse(questr.Attributes["Active"].Value);
                quest[id].karma = int.Parse(questr.Attributes["Karma"].Value);
            }
            file.Save("quests.xml");
        }
    }

    // Checks whether the quests were already generated for the period of time, if not generates new quests
    public static void CheckAndGenerateQ()
    {
        //CopyAQuest();
        CheckTimeGenerated();
        if (CheckTimeGenerated()) { MakeNewQuests(nmbrOfQuests); }
    }

    // Unity Button can't call static method, thus instance method wrap
    public void ButtonCheckAndGenerateQ()
    {
        CheckAndGenerateQ();
    }
    
}
