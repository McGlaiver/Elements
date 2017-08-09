using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class AssignQuest : MonoBehaviour
{
    public int id;
    public static int clickedQuestID = 0;
    public Boolean switchVar;
    public GameObject coverDescription;


   
    // Generates Names of the location for the environment based on the quest values
    public string DescriptionEnvironment(int id)
    {
        string description;
        if (QuestGenerator.quest[id].envType >= 1 && QuestGenerator.quest[id].envType <= 3) { description = "Aien Forest  "; }
        else if (QuestGenerator.quest[id].envType >= 4 && QuestGenerator.quest[id].envType <= 5) { description = "Roccati Highlands  "; }
        else { description = "Edran Desert "; }

        return description;
    }
    // Generates random enemy name so it would make some sense, based on quest values
    public string DescriptionEnemy(int id)
    {
        string description = "";
        if ((QuestGenerator.quest[id].questType >= 0 && QuestGenerator.quest[id].questType <= 28) || (QuestGenerator.quest[id].questType > 56 && QuestGenerator.quest[id].questType <= 95))
        {
            if (QuestGenerator.quest[id].enemyType == 1) { description = "bandits "; }
            else if (QuestGenerator.quest[id].enemyType == 2) { description = "some predatory wildlife  "; }
            else if (QuestGenerator.quest[id].enemyType == 3) { description = "enemy patrol "; }
            else if (QuestGenerator.quest[id].enemyType == 4) { description = "elementalist apprentice "; }
            else { description = "enemy Journeyman elementalist "; }
        }
        else if (QuestGenerator.quest[id].questType > 95 && QuestGenerator.quest[id].questType <= 110)
        {
            description = "Etwar the Rogue Master Elementalist ";
        }
        else if (QuestGenerator.quest[id].questType > 28 && QuestGenerator.quest[id].questType <= 56)
        {
            if (QuestGenerator.quest[id].enemyType >= 1 && QuestGenerator.quest[id].enemyType <= 3) { description = "our citizens  "; }
            if (QuestGenerator.quest[id].enemyType == 4) { description = "messenger "; }
            if (QuestGenerator.quest[id].enemyType == 5) { description = "our clan's leader's offspring  "; }
        }
        else { description = " "; }
        return description;
    }

    // Generates dynamic quest description
    public void GenerateQuestDescription(int id)
    {
        int duration = QuestScreen.GetQuestDuration(id);
        int questType = QuestGenerator.quest[id].questType;

        string questDescription;
        if (questType > 0 && questType <= 14)
        {
            questDescription = "We need you to kill ";
            questDescription = questDescription + DescriptionEnemy(id) + ", it has been on our to do list for quite some time. It will help our cause. Please travel to ";
            questDescription = questDescription + DescriptionEnvironment(id) + "and take care of this.";
        }
        else if (questType > 14 && questType <= 28)
        {
            questDescription = "I am looking for some lost goods, they were last seen in presence of ";
            questDescription = questDescription + DescriptionEnemy(id) + "when our caravan was ambushed. Please travel to " + DescriptionEnvironment(id) + "and retrieve these goods for me. It is very important.";
        }
        else if (questType > 28 && questType <= 42)
        {
            questDescription = "It is of the utmost importance that you aid " + DescriptionEnemy(id) + "fighting off the attackers in " + DescriptionEnvironment(id) + "as we cannot afford to lose any more allies.";
        }
        else if (questType > 42 && questType <= 56)
        {
            questDescription = "I'm asking you to escort " + DescriptionEnemy(id) + "on their mission in " + DescriptionEnvironment(id) + "to recruit more allies. Should they fail their quest, it would be a devastating blow to our numbers.";
        }
        else if (questType > 56 && questType <= 70)
        {
            questDescription = "We need you to sabotage enemy base in " + DescriptionEnvironment(id) + "since enemy activity in this region is growing every day. To get in the base you will have to get through " + DescriptionEnemy(id) + ", but if you choose a stealthy approach, I am sure that you will succeed.";
        }
        else if (questType > 70 && questType <= 95)
        {
            questDescription = "I have great news. We've received a tip that an enemy convoy will be passing through " + DescriptionEnvironment(id) + ". The biggest problem for you will be dealing with " + DescriptionEnemy(id) + " when stopping this convoy.";
        }
        else if (questType > 95 && questType <= 110)
        {
            questDescription = "I've had enough! " + DescriptionEnemy(id) + "has been a thorn in our side for far too long. We've put a bounty on their head. Travel to " + DescriptionEnvironment(id) + " and bring me their head. Make this world a better place.";
        }
        else { questDescription = "This is your lucky day my friend. I've got a tip from a reliable source about a treasure hidden in " + DescriptionEnvironment(id) + ". I know exactly where it is, but I cannot leave my spot of a quartermaster. I can draw you a map, you go get it for me, we will split the loot in half. Try crossing me and I will make sure the next bounty on the board is on your head."; }

        GameObject.Find("TextDescription").GetComponentInChildren<Text>().text = questDescription;
        GameObject.Find("TextQuestName").GetComponentInChildren<Text>().text = QuestNameGenerator(id);
        GameObject.Find("TextQuestDuration").GetComponentInChildren<Text>().text = duration + " min";


    }

    
    

    // Finalize and show all descriptions, forwards id of the quest so the tab knows which description to show
    public void ShowQuestDescription(int id)
    {
        QuestScreen.ButtonsInteractable();
        clickedQuestID = id;
        GenerateQuestDescription(id);
    }


    // Changes colors of the quests based on karma, or the quest being active or completed
    public static void ChangeTextColor(int id)
    {
        if (QuestGenerator.quest[id].karma == 1 && QuestGenerator.quest[id].active != 1) { GameObject.Find("Quest" + id).GetComponent<Text>().color = Color.blue; }
        else if (QuestGenerator.quest[id].karma == 2 && QuestGenerator.quest[id].active != 1) { GameObject.Find("Quest" + id).GetComponent<Text>().color = Color.red; }
        else if (QuestGenerator.quest[id].active == 1) { GameObject.Find("Quest" + id).GetComponent<Text>().color = Color.yellow; }
        else if (QuestGenerator.quest[id].active == 2) { GameObject.Find("Quest" + id).GetComponent<Text>().color = Color.gray; }
        else { GameObject.Find("Quest"+id).GetComponent<Text>().color = Color.black; }
    }


    // Generates Quest Names
    public static string QuestNameGenerator(int id)
    {     
        int questType = QuestGenerator.quest[id].questType;
        string questName;

        if (questType > 0 && questType <= 14) { questName = "Time to kill"; }
        else if (questType > 14 && questType <= 28) { questName = "Lost goods"; }
        else if (questType > 28 && questType <= 42) { questName = "To serve and protect"; }
        else if (questType > 42 && questType <= 56) { questName = "Escort VIP"; }
        else if (questType > 56 && questType <= 70) { questName = "Sabotage"; }
        else if (questType > 70 && questType <= 95) { questName = "Guarded Convoy"; }
        else if (questType > 95 && questType <= 110) { questName = "Bounty"; }
        else { questName = "Treasure Hunt"; }

        return questName;
    }

    // Write the generated quest in the Quest tab
    public static void GenerateQuestName(int id)
    {
        int duration = QuestScreen.GetQuestDuration(id);
        if (QuestGenerator.quest[id].active == 2)
        {
            GameObject.Find("Quest" + id).GetComponentInChildren<Text>().text = QuestNameGenerator(id) + " (Completed)";
            GameObject.Find("Quest" + id).GetComponentInChildren<Button>().interactable = false;
        }
        else
        {
            GameObject.Find("Quest" + id).GetComponentInChildren<Text>().text = QuestNameGenerator(id) + " (" + duration + " minutes)";
            GameObject.Find("Quest" + id).GetComponentInChildren<Button>().interactable = true;
        }

        ChangeTextColor(id); 
    }

    // Wrap for refresh button, GenerateQuestName
    public void RefreshGenerateQuestName(int idi)
    {
        GenerateQuestName(idi);
    }

    // Loads the initial quest description cover panel, so it can be deactivated and reactivated upon return to the page

    void Awake()
    {
        coverDescription = GameObject.Find("coverDescription");
    }
    
    //if this is not called by Button Refresh, then check memory and generate new quest names, fill the quest tab
    void Start()
    {
        
        switchVar = true;
        if (GetComponentInChildren<Text>().text != "Refresh")
        {
            if (QuestGenerator.quest[1].bonusType == 0) { QuestGenerator.CheckMemoryQ(); }
            GenerateQuestName(id);
        }
    }

    // if someone clicked on a quest, hide the description cover and keep it deactivated. If people come back from the fight reactivate it until they click on a quest again
    void Update()
    {
        if (clickedQuestID > 0 && switchVar) { coverDescription.SetActive(false); switchVar = false; }
        else if (clickedQuestID < 1 && !switchVar) { coverDescription.SetActive(true); }
    }
}

