using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// Enemy Class
public class Foe
{
    public int hp, dmg, armor;

    public void LoadFoe()
    {
        XmlDocument file = new XmlDocument();
        file.Load("enemies.xml");
        foreach (XmlNode foe in file.DocumentElement)
        {
            if (foe.Attributes["ID"].Value == QuestGenerator.quest[0].enemyType.ToString())
            {
                hp = Convert.ToInt16(int.Parse(foe.Attributes["HP"].Value) * RunStartUpdate.thePlayer.level * 1.3);
                dmg = int.Parse(foe.Attributes["Dmg"].Value) * RunStartUpdate.thePlayer.level * 5;
                armor = int.Parse(foe.Attributes["Armor"].Value) * RunStartUpdate.thePlayer.level;
            }
        }
    }
}

public class FightScreen : MonoBehaviour {
    public static Boolean victory;
    public Boolean finishedFight = false;
    public Boolean finishedDefense = true;
    public GameObject fightButton;
    public GameObject defenseButton;
    public GameObject attackText;
    public GameObject counting;
    public GameObject endFight;
    public System.Random rnd = new System.Random();
    public int countDown = 10;
    public Foe enemy = new Foe();
    public Boolean blockk = false;
    public int playerHP; 

    // Loads Enemy image
    public void LoadEnemyImg()
    {
        string enemyImg;
        if (QuestGenerator.quest[0].enemyType == 1) { enemyImg = "bandit"; }
        else if (QuestGenerator.quest[0].enemyType == 2) { enemyImg = "bear"; }
        else if (QuestGenerator.quest[0].enemyType == 3) { enemyImg = "patrol"; }
        else if (QuestGenerator.quest[0].enemyType == 4) { enemyImg = "apprentice"; }
        else { enemyImg = "master"; }
        GameObject.Find("Enemy").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(enemyImg);
    }

    // Loads Player Image
    public void LoadPlayerImg()
    {
        string playerImg;
        if (RunStartUpdate.thePlayer.karma >= 0) { playerImg = RunStartUpdate.thePlayer.picture.ToString(); }
        else { playerImg = RunStartUpdate.thePlayer.picture.ToString() + "B"; }
        GameObject.Find("Player").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(playerImg);
    }

    // Loads Background
    public void LoadBackgroundImg()
    {
        string background;
        if (QuestGenerator.quest[0].envType >= 1 && QuestGenerator.quest[0].envType <= 3) { background = "aien"; }
        else if (QuestGenerator.quest[0].envType >= 4 && QuestGenerator.quest[0].envType <= 5) { background = "roccati"; }
        else { background = "edran"; }
        GameObject.Find("FightBackground").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(background);
    }

    // Loads enemy HP
    public void LoadEnemyHP()
    {
        GameObject.Find("EnemyHP").GetComponent<Slider>().maxValue = enemy.hp;
        GameObject.Find("EnemyHP").GetComponent<Slider>().value = enemy.hp;
    }

    // Loads Player HP
    public void LoadPlayerHP()
    {
        playerHP = RunStartUpdate.thePlayer.stamina * 60;
        GameObject.Find("PlayerHP").GetComponentInChildren<Slider>().maxValue = playerHP;
        GameObject.Find("PlayerHP").GetComponentInChildren<Slider>().value = playerHP;
    }

    // Generates FightOdds 1-100
    public int RandomizeFightOdds()
    {
        int result;
        result = rnd.Next(1, 100);
        return result;
    }

    // Counts down during player's attack, if the timer gets under 3 seconds, the color of the numbers changes to Red. If it gets to 0, player misses his chance to attack
    public void Timer()
    {
        GameObject.Find("Counting").GetComponentInChildren<Text>().text = countDown.ToString();
        if (countDown <= 3) { GameObject.Find("Counting").GetComponentInChildren<Text>().color = Color.red; }
        else { GameObject.Find("Counting").GetComponentInChildren<Text>().color = Color.white; }

        if (countDown <= 0)
        {
            CancelInvoke("CountDown");
            CancelInvoke("ChangeFightButton");
            GameObject.Find("attackType").GetComponentInChildren<Text>().text = "1";
            fightButton.GetComponentInChildren<Image>().color = Color.gray;
            fightButton.GetComponentInChildren<Text>().text = "Miss";
            Attack();
        }
        
    }

    //Changes the fight button color and values for attack based on the generated odds 0-25 Miss, 25-50 Weak attack, 50-80 fair, 80-100 strong attack
    public void ChangeFightButton()
    {
        
        int odds = RandomizeFightOdds();
        if (odds <= 25)
        {
            GameObject.Find("attackType").GetComponentInChildren<Text>().text = "1";
            fightButton.GetComponentInChildren<Image>().color = Color.gray;
            fightButton.GetComponentInChildren<Text>().text = "Miss";
        }
        else if (odds > 25 && odds <= 50)
        {
            GameObject.Find("attackType").GetComponentInChildren<Text>().text = "2";
            fightButton.GetComponentInChildren<Image>().color = Color.red;
            fightButton.GetComponentInChildren<Text>().text = "Weak";
        }
        else if (odds > 50 && odds <= 80)
        {
            GameObject.Find("attackType").GetComponentInChildren<Text>().text = "3";
            fightButton.GetComponentInChildren<Image>().color = Color.yellow;
            fightButton.GetComponentInChildren<Text>().text = "Fair";
        }
        else
        {
            GameObject.Find("attackType").GetComponentInChildren<Text>().text = "4";
            fightButton.GetComponentInChildren<Image>().color = Color.green;
            fightButton.GetComponentInChildren<Text>().text = "Strong";
        }
        Timer();

    }

    // Calculates the damage the player can do based on attributes and class, gives hard chance of 10% to score a critial chance (default is no bonus), later on crit % chance will be incorporated
    public int CalculateDamage()
    {
        int baseDamage;
        int crit, attributeBonus = 1;
        if (rnd.Next(1, 10) == 4) { crit = RunStartUpdate.thePlayer.crit; }
        else crit = 100;

        if(RunStartUpdate.thePlayer.characterClass == "Warbringer")
        {
            attributeBonus = 2 * RunStartUpdate.thePlayer.strength;
        }
        else if (RunStartUpdate.thePlayer.characterClass == "Assassin")
        {
            attributeBonus = 2 * RunStartUpdate.thePlayer.agility;
        }
        else if (RunStartUpdate.thePlayer.characterClass == "Occultist")
        {
            attributeBonus = 2 * RunStartUpdate.thePlayer.intellect;
        }


        baseDamage = Convert.ToInt32(Math.Floor(RunStartUpdate.thePlayer.dmg * ((double)crit / 100) * attributeBonus));
        baseDamage = rnd.Next(Convert.ToInt16(baseDamage * 0.9), Convert.ToInt16(baseDamage * 1.1)); // 90-110% of the damage so the numbers aren't the same all the time
        return baseDamage;
    }

    
    // Calculate Damage based on the strength of the swing
    public int CalculateSwingDamage()
    {

        int damageDo;
        string swingValue = (GameObject.Find("attackType").GetComponentInChildren<Text>().text).ToString();
        if (swingValue == "2") { damageDo = Convert.ToInt16(0.3 * CalculateDamage()); }
        else if (swingValue == "3") { damageDo = Convert.ToInt16(0.6 * CalculateDamage()); }
        else if (swingValue == "4") { damageDo = CalculateDamage(); }
        else damageDo = 0;
        damageDo = damageDo - enemy.armor;
        if (damageDo < 0) { damageDo = 0; }
        return damageDo;
    }
    
    // Checks whether anyone died in the round
    public Boolean CheckForEnd()
    {
        Boolean end = false;
        if (playerHP <= 0 || enemy.hp <= 0) { end = true; }
        return end;
    }

    // Gets player out of the fight back to the quest screen
    public void GoBackToQuests()
    {
        SceneManager.LoadScene("QuestTab");
    }

    // Switch between Attack and Defense, start new rounds, conclude fight
    public void SwitchFightButtons()
    {
        if (!CheckForEnd())
        {
            if (finishedFight)
            {
                counting.SetActive(false);
                attackText.GetComponent<Text>().text = "DEFEND!";
                fightButton.SetActive(false);
                defenseButton.SetActive(true);
                StartDefense();
            }
            else if (finishedDefense)
            {
                counting.SetActive(true);
                attackText.GetComponent<Text>().text = "ATTACK!";
                fightButton.SetActive(true);
                defenseButton.SetActive(false);
                countDown = 10;
                InvokeRepeating("ChangeFightButton", 0, 0.5f);
                InvokeRepeating("CountDown", 0, 1);
            }
        }
        else
        {
            AssignQuest.clickedQuestID = 0;
            endFight.SetActive(true);
            if (playerHP <= 0) { victory = false; GameObject.Find("EndText").GetComponent<Text>().text = "You lost. You will only get half the reward as a penalty for medical expenses."; }
            else if (enemy.hp <= 0) { victory = true; GameObject.Find("EndText").GetComponent<Text>().text = "You won! Enjoy your reward."; }

        }
    }

    // Subtracts 1 second from the countdown
    public void CountDown()
    {
        countDown--;
    }

    // Changes enemy's Health Bar TEXT
    public void ChangeEnemySliderValue()
    {
        GameObject.Find("EnemyHPValue").GetComponent<Text>().text = enemy.hp + " / " + GameObject.Find("EnemyHP").GetComponent<Slider>().maxValue;
    }
    // Changes player's Health Bar TEXT
    public void ChangePlayerSliderValue()
    {
        GameObject.Find("PlayerHPValue").GetComponent<Text>().text = playerHP + " / " + GameObject.Find("PlayerHP").GetComponentInChildren<Slider>().maxValue;
    }
    // Takes health away from the enemy
    public void TakeEnemyHealth()
    {
        enemy.hp = enemy.hp - CalculateSwingDamage();
        GameObject.Find("EnemyHP").GetComponent<Slider>().value = enemy.hp;

    }
    // Takes health away from Player
    public void TakeDamage(Boolean wasHit)
    {
        if (wasHit)
        {
            playerHP = playerHP - DamageDealtToPlayer();
            GameObject.Find("PlayerHP").GetComponent<Slider>().value = playerHP;
        }
    }
    // Calculates how much damage is player taking
    public int DamageDealtToPlayer()
    {
        int damageDealt;
        damageDealt = enemy.dmg;
        damageDealt = rnd.Next(Convert.ToInt16(damageDealt * 0.9), Convert.ToInt16(damageDealt * 1.1));
        return damageDealt;
    }

    // Invoked by StartDefense, after it's invoked it gives player 0.38 seconds to click Defend button, after the 0.38 seconds it invokes EnemyAttacked, which means player missed block
    public void EnemyAttack()
    {
        defenseButton.GetComponentInChildren<Image>().color = Color.green;
        defenseButton.GetComponent<Button>().interactable = true;
        defenseButton.GetComponentInChildren<Text>().text = "NOW!";
        GameObject.Find("attackType").GetComponentInChildren<Text>().text = "5";
        Invoke("EnemyAttacked", 0.38f);
    }

    // Player missed block and enemy attacked
    public void EnemyAttacked()
    {
        defenseButton.GetComponentInChildren<Image>().color = Color.grey;
        defenseButton.GetComponent<Button>().interactable = true;
        defenseButton.GetComponentInChildren<Text>().text = "Too late";
        GameObject.Find("attackType").GetComponentInChildren<Text>().text = "6";
    }

    // Method called after the attack button is clicked
    public void Attack()
    {
        CancelInvoke("ChangeFightButton");
        CancelInvoke("CountDown");
        TakeEnemyHealth();
        finishedFight = true;
        finishedDefense = false;
        SwitchFightButtons();   
    }

    // Ran when it's time for defense, Invokes Enemy Attack
    public void StartDefense()
    {
        float time = float.Parse(((rnd.NextDouble() * (6 - 3)) + 3).ToString());
        defenseButton.GetComponent<Button>().interactable = false;
        defenseButton.GetComponentInChildren<Image>().color = Color.red;
        defenseButton.GetComponentInChildren<Text>().text = "Stand ready!";
        Invoke("EnemyAttack", time);
    }

    // Method is called after Defend button is clicked
    public void Defend()
    {
        CancelInvoke("EnemyAttacked");
        if (GameObject.Find("attackType").GetComponentInChildren<Text>().text == "6")
        {
            TakeDamage(true);
        }
        else if (GameObject.Find("attackType").GetComponentInChildren<Text>().text == "5")
        {
            TakeDamage(false);
        }
        finishedDefense = true;
        finishedFight = false;
        SwitchFightButtons();        
    }

    // Starts the fight after "Fight" button is clicked and hides the Fight button
    public void StartFight()
    {
        SwitchFightButtons();
        GameObject.Find("StartFightButton").SetActive(false);
    }

    // Use this for initialization
    void Start () {
        enemy.LoadFoe();
        LoadEnemyHP();
        LoadPlayerHP();
        endFight = GameObject.Find("EndOfFight");
        endFight.SetActive(false);
        fightButton = GameObject.Find("FightButton");
        defenseButton = GameObject.Find("DefenseButton");
        attackText = GameObject.Find("AttackText");
        counting = GameObject.Find("Counting");
        defenseButton.SetActive(false);

            LoadBackgroundImg();
            LoadPlayerImg();
            LoadEnemyImg();
                   
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
