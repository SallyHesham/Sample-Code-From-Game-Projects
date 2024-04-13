using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScript : MonoBehaviour
{
    public GameObject battleUI;
    public GameObject playerInfoObject;
    public GameObject enemyInfoObject;
    public GameObject buttonsObject;
    public TMP_Text desc;
    public EnemyAI enemyAI;
    public CharacterScript playerCharacter;
    public CharacterScript enemyCharacter;
    public GameObject warningUI;
    public TMP_Text warningText;
    public TMP_Text logText;
    public GameObject gameOverUI;
    public GameObject doorUI;

    private List<TMP_Text> playerInfo;
    private List<TMP_Text> enemyInfo;
    private List<ActionButtonScript> buttons;

    private PartScript playerSource;
    private PartScript playerTarget;
    private PartData.Ability playerAction;
    private bool isActionSelected;
    private PartScript enemySource;
    private PartScript enemyTarget;
    private PartData.Ability enemyAction;
    private bool hasPlayerBitten;
    public bool hasEnemyBitten;
    private int manaCost;
    private float gazeThreshhold;

    private void Start()
    {
        manaCost = 5;
        gazeThreshhold = 15f;

        // build reference lists

        playerInfo = new List<TMP_Text>();
        foreach (Transform child in playerInfoObject.transform)
        {
            playerInfo.Add(child.gameObject.GetComponent<TMP_Text>());
        }

        enemyInfo = new List<TMP_Text>();
        foreach (Transform child in enemyInfoObject.transform)
        {
            enemyInfo.Add(child.gameObject.GetComponent<TMP_Text>());
        }

        buttons = new List<ActionButtonScript>();
        foreach (Transform child in buttonsObject.transform)
        {
            buttons.Add(child.gameObject.GetComponent<ActionButtonScript>());
        }

        // clear all
        ClearAll();
        hasPlayerBitten = false;
        hasEnemyBitten = false;

        OverlordScript.inst.onStartBattle += StartBattle;
        OverlordScript.inst.onPlayerDeath += EndPlayerBattle;
        OverlordScript.inst.onEnemyDeath += EndEnemyBattle;
    }

    public void StartBattle()
    {
        battleUI.SetActive(true);
        AudioScript.inst.PlayBGM();
    }

    public void EndPlayerBattle()
    {
        battleUI.SetActive(false);
        if (SceneManager.GetActiveScene().name != "PostludeScene")
        {
            playerCharacter.gameObject.SetActive(false);
            gameOverUI.SetActive(true);
        }
        else
        {
            OverlordScript.inst.EnemyDeath();
        }
        AudioScript.inst.StopBGM();
    }

    public void EndEnemyBattle()
    {
        battleUI.SetActive(false);
        if (SceneManager.GetActiveScene().name != "PostludeScene")
        {
            enemyCharacter.gameObject.SetActive(false);
            doorUI.SetActive(true);
        }
        else
        {
            // load serpent in his original form
            enemyCharacter.LoadSerpent();
        }
        AudioScript.inst.StopBGM();
    }

    public void SetPlayerSource(PartScript part)
    {
        playerSource = part;
    }

    public void ClearPlayerSource()
    {
        playerSource = null;
    }

    public void SetPlayerTarget(PartScript part)
    {
        playerTarget = part;
    }

    public void ClearPlayerTarget()
    {
        playerTarget = null;
    }

    public void SetPlayerAction(PartData.Ability ability)
    {
        playerAction = ability;
        isActionSelected = true;
    }

    public void ClearPlayerAction()
    {
        isActionSelected = false;
    }

    public void SetEnemySource(PartScript part)
    {
        enemySource = part;
    }

    public void SetEnemyTarget(PartScript part)
    {
        enemyTarget = part;
    }

    public void SetEnemyAction(PartData.Ability ability)
    {
        enemyAction = ability;
    }

    // resolves all battle actions chosen
    public void Resolve()
    {
        ClearLogText();

        // checks ////////////////////////////////////////////////////////
        if (!isActionSelected)
        {
            // warning to select action
            SetWarning("Select an action");
            return;
        }
        else if ((playerAction == PartData.Ability.Swap ||
                playerAction == PartData.Ability.Attack ||
                playerAction == PartData.Ability.Bite) &&
                playerTarget == null)
        {
            // warning to select target
            SetWarning("Select a target");
            return;
        }
        else if ((playerAction == PartData.Ability.Attack || playerAction == PartData.Ability.Bite) &&
            playerTarget.destroyed)
        {
            // warning invalid target
            SetWarning("Invalid Target");
            return;
        }
        else if (playerAction == PartData.Ability.Bite && hasPlayerBitten)
        {
            // warning that you are full
            SetWarning("You are full and can no longer bite");
            return;
        }
        else if (playerAction == PartData.Ability.See && enemyCharacter.FindPart(PartData.Type.Head).destroyed)
        {
            // warning cannot peer into with head destroyed
            SetWarning("You cannot use See as the head is destroyed.");
            return;
        }
        else
        {
            // bau
            // prompt enemy to decide on defense
            bool def = enemyAI.DefendOrNot();

            // RESOLVE //////////////////////////////////////////////////////////////////////////////////

            // resolve DEFEND action for ENEMY
            if (def && playerAction != PartData.Ability.Swap && playerAction != PartData.Ability.Defend &&
                playerAction != PartData.Ability.Nothing)
            {
                playerTarget = enemySource;
                AddLogText("The Boss uses its " + enemySource.type.ToString() + " to defend itself.");
            }
            else
            {
                def = false;
            }

            // resolve PLAYER action /////////////////////////////////////

            switch (playerAction)
            {
                // resolve ATTACK action
                case PartData.Ability.Attack:
                    int ddmg = playerSource.currentPart.atk - playerTarget.currentPart.def;
                    if (ddmg < 0)
                    {
                        ddmg = 0;
                    }
                    AddLogText("You use your " +  playerSource.type.ToString() + " to attack the Boss's " +
                        playerTarget.type.ToString() + " and do " +  
                        (ddmg).ToString() +
                        " damage.");
                    playerTarget.Damage(playerSource.currentPart.atk);
                    break;
                // resolve AREA ATTACK action
                case PartData.Ability.AreaAttack:
                    AddLogText("You use your " + playerSource.type.ToString() + " to attack all the Boss's parts.");
                    enemyCharacter.DamageAllParts(playerSource.currentPart.atk);
                    break;
                // resolve BITE action
                case PartData.Ability.Bite:
                    int dmg = playerSource.currentPart.hp - playerSource.currentHP;
                    AddLogText("You use your " + playerSource.type.ToString() + " to bite the Boss's " +
                        playerTarget.type.ToString() + " and drain " +
                        dmg.ToString() + " HP.");
                    playerTarget.Drain(dmg);
                    playerSource.Heal(dmg);
                    hasPlayerBitten = true;
                    break;
                // resolve SWAP action
                case PartData.Ability.Swap:
                    // the serpent's tail exception
                    PartScript playerTest = playerCharacter.FindPart(PartData.Type.Tail);
                    PartScript enemyTest = enemyCharacter.FindPart(PartData.Type.Tail);
                    if (true)
                    {
                        if ((playerTarget.type == PartData.Type.Tail ||
                        playerTarget.type == PartData.Type.LeftLeg ||
                        playerTarget.type == PartData.Type.RightLeg) &&
                        enemyTest.currentPart != null &&
                        enemyTest.currentPart.name == "The Serpent's Tail")
                        {
                            // if the target is the serpent's tail
                            // mana check
                            if (!playerSource.gameObject.GetComponent<ManaScript>().UseMana(manaCost))
                            {
                                // warning no mana
                                SetWarning("Not enough Mana");
                                return;
                            }
                            PartData playersNewTail = null;
                            int playersNewTailHP = 0;
                            bool playersNewTailIsDestroyed = true;
                            PartData enemysNewTail = null;
                            int enemysNewTailHP = 0;
                            bool enemysNewTailIsDestroyed = true;
                            PartData enemysNewLLeg = null;
                            int enemysNewLLegHP = 0;
                            bool enemysNewLLegIsDestroyed = true;
                            PartData enemysNewRLeg = null;
                            int enemysNewRLegHP = 0;
                            bool enemysNewRLegIsDestroyed = true;

                            playersNewTail = enemyTest.currentPart;
                            playersNewTailHP = enemyTest.currentHP;
                            playersNewTailIsDestroyed = enemyTest.destroyed;

                            PartScript playerTail = playerCharacter.FindPart(PartData.Type.Tail);
                            PartScript playerLLeg = playerCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript playerRLeg = playerCharacter.FindPart(PartData.Type.RightLeg);
                            PartScript enemyTail = enemyCharacter.FindPart(PartData.Type.Tail);
                            PartScript enemyLLeg = enemyCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript enemyRLeg = enemyCharacter.FindPart(PartData.Type.RightLeg);

                            if (playerTail.currentPart != null)
                            {
                                enemysNewTail = playerTail.currentPart;
                                enemysNewTailHP = playerTail.currentHP;
                                enemysNewTailIsDestroyed = playerTail.destroyed;
                            }
                            if (playerLLeg.currentPart != null)
                            {
                                enemysNewLLeg = playerLLeg.currentPart;
                                enemysNewLLegHP = playerLLeg.currentHP;
                                enemysNewLLegIsDestroyed = playerLLeg.destroyed;
                            }
                            if (playerRLeg.currentPart != null)
                            {
                                enemysNewRLeg = playerRLeg.currentPart;
                                enemysNewRLegHP = playerRLeg.currentHP;
                                enemysNewRLegIsDestroyed = playerRLeg.destroyed;
                            }
                            // finally swap
                            playerTail.Swap(playersNewTail, playersNewTailHP, playersNewTailIsDestroyed);
                            playerLLeg.Swap(null, 0, true);
                            playerRLeg.Swap(null, 0, true);
                            enemyTail.Swap(enemysNewTail, enemysNewTailHP, enemysNewTailIsDestroyed);
                            enemyLLeg.Swap(enemysNewLLeg, enemysNewLLegHP, enemysNewLLegIsDestroyed);
                            enemyRLeg.Swap(enemysNewRLeg, enemysNewRLegHP, enemysNewRLegIsDestroyed);
                            AddLogText("You swap lower bodies with the Boss.");
                            break;
                        }
                        
                    }
                    if (playerTest.currentPart != null)
                    {
                        if ((playerTarget.type == PartData.Type.Tail ||
                        playerTarget.type == PartData.Type.LeftLeg ||
                        playerTarget.type == PartData.Type.RightLeg)
                        && playerTest.currentPart.name == "The Serpent's Tail")
                        {
                            // if the current part is the serpent's tail
                            // mana check
                            if (!playerSource.gameObject.GetComponent<ManaScript>().UseMana(manaCost))
                            {
                                // warning no mana
                                SetWarning("Not enough Mana");
                                return;
                            }
                            PartData enemysNewTail = null;
                            int enemysNewTailHP = 0;
                            bool enemysNewTailIsDestroyed = true;
                            PartData playersNewTail = null;
                            int playersNewTailHP = 0;
                            bool playersNewTailIsDestroyed = true;
                            PartData playersNewLLeg = null;
                            int playersNewLLegHP = 0;
                            bool playersNewLLegIsDestroyed = true;
                            PartData playersNewRLeg = null;
                            int playersNewRLegHP = 0;
                            bool playersNewRLegIsDestroyed = true;

                            enemysNewTail = playerTest.currentPart;
                            enemysNewTailHP = playerTest.currentHP;
                            enemysNewTailIsDestroyed = playerTest.destroyed;

                            PartScript playerTail = playerCharacter.FindPart(PartData.Type.Tail);
                            PartScript playerLLeg = playerCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript playerRLeg = playerCharacter.FindPart(PartData.Type.RightLeg);
                            PartScript enemyTail = enemyCharacter.FindPart(PartData.Type.Tail);
                            PartScript enemyLLeg = enemyCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript enemyRLeg = enemyCharacter.FindPart(PartData.Type.RightLeg);

                            if (enemyTail.currentPart != null)
                            {
                                playersNewTail = enemyTail.currentPart;
                                playersNewTailHP = enemyTail.currentHP;
                                playersNewTailIsDestroyed = enemyTail.destroyed;
                            }
                            if (enemyLLeg.currentPart != null)
                            {
                                playersNewLLeg = enemyLLeg.currentPart;
                                playersNewLLegHP = enemyLLeg.currentHP;
                                playersNewLLegIsDestroyed = enemyLLeg.destroyed;
                            }
                            if (enemyRLeg.currentPart != null)
                            {
                                playersNewRLeg = enemyRLeg.currentPart;
                                playersNewRLegHP = enemyRLeg.currentHP;
                                playersNewRLegIsDestroyed = enemyRLeg.destroyed;
                            }
                            // finally swap
                            enemyTail.Swap(enemysNewTail, enemysNewTailHP, enemysNewTailIsDestroyed);
                            enemyLLeg.Swap(null, 0, true);
                            enemyRLeg.Swap(null, 0, true);
                            playerTail.Swap(playersNewTail, playersNewTailHP, playersNewTailIsDestroyed);
                            playerLLeg.Swap(playersNewLLeg, playersNewLLegHP, playersNewLLegIsDestroyed);
                            playerRLeg.Swap(playersNewRLeg, playersNewRLegHP, playersNewRLegIsDestroyed);
                            AddLogText("You swap lower bodies with the Boss.");
                            break;
                        }
                    }
                    // bau
                    PartData enemyPart = null;
                    int enemyHP = 0;
                    bool enemyIsDestroyed = true;
                    PartData playerPart = null;
                    int playerHP = 0;
                    bool playerIsDestroyed = true;
                    if (playerTarget.currentPart != null)
                    {
                        enemyPart = playerTarget.currentPart;
                        enemyHP = playerTarget.currentHP;
                        enemyIsDestroyed = playerTarget.destroyed;
                    }
                    PartScript playerPartScript = playerCharacter.FindPart(playerTarget.type);
                    if (playerPartScript.currentPart != null)
                    {
                        playerPart = playerPartScript.currentPart;
                        playerHP = playerPartScript.currentHP;
                        playerIsDestroyed = playerPartScript.destroyed;
                    }
                    if (playerPart == null && enemyPart == null)
                    {
                        // warning that there is nothing to swap
                        SetWarning("There is nothing to swap");
                        return;
                    }
                    // mana check
                    if (!playerSource.gameObject.GetComponent<ManaScript>().UseMana(manaCost))
                    {
                        // warning no mana
                        SetWarning("Not enough Mana");
                        return;
                    }
                    // and finally swap
                    playerTarget.Swap(playerPart, playerHP, playerIsDestroyed);
                    playerPartScript.Swap(enemyPart, enemyHP, enemyIsDestroyed);
                    AddLogText("You swap " + playerTarget.type.ToString() + "s with the Boss.");
                    break;
                // resolve SEE action
                case PartData.Ability.See:
                    // eyes animation here
                    playerSource.gameObject.GetComponent<ManaScript>().ExposeEyes();
                    AddLogText("You gaze into the Boss's eyes, attempting to reach their soul.");
                    bool success = GazeOfChaos();
                    if (success)
                    {
                        AddLogText("You succeed and the Boss's soul is lost.");
                        PartScript part = enemyCharacter.FindPart(PartData.Type.Body);
                        part.Drain(part.currentPart.hp + 100);
                    }
                    else
                    {
                        AddLogText("You fail and the Boss is unaffected.");
                    }
                    break;
                // resolve NOTHING action
                case PartData.Ability.Nothing:
                    AddLogText("You do nothing.");
                    break;
                default: break;
            }

            // add time stone sound effect here
            AudioScript.inst.PlayTimeSound();

            // prompt enemy to take proper action
            if (!def)
            {
                enemyAI.TakeAction();
            }

            // resolve DEFEND action for PLAYER
            if (playerAction == PartData.Ability.Defend && enemyAction != PartData.Ability.Swap)
            {
                enemyTarget = playerSource;
                AddLogText("You use your " + playerSource.type.ToString() + " to defend yourself.");
            }

            // resolve ENEMY action /////////////////////////////////////////

            switch (enemyAction)
            {
                // resolve ATTACK action
                case PartData.Ability.Attack:
                    int ddmg = enemySource.currentPart.atk - enemyTarget.currentPart.def;
                    if (ddmg < 0)
                    {
                        ddmg = 0;
                    }
                    AddLogText("The Boss uses its " + enemySource.type.ToString() + " to attack your " +
                        enemyTarget.type.ToString() + " and does " +
                        (ddmg).ToString() +
                        " damage.");
                    enemyTarget.Damage(enemySource.currentPart.atk);
                    break;
                // resolve AREA ATTACK action
                case PartData.Ability.AreaAttack:
                    AddLogText("The Boss uses its " + enemySource.type.ToString() + " to attack all your parts.");
                    playerCharacter.DamageAllParts(enemySource.currentPart.atk);
                    break;
                // resolve BITE action
                case PartData.Ability.Bite:
                    int dmg = enemySource.currentPart.hp - enemySource.currentHP;
                    AddLogText("The Boss uses its " + enemySource.type.ToString() + " to bite your " +
                        enemyTarget.type.ToString() + " and drains " +
                        dmg.ToString() + " HP.");
                    enemyTarget.Drain(dmg);
                    enemySource.Heal(dmg);
                    hasEnemyBitten = true;
                    break;
                // resolve SWAP action
                case PartData.Ability.Swap:
                    // the serpent's tail exception
                    PartScript playerTest = playerCharacter.FindPart(PartData.Type.Tail);
                    PartScript enemyTest = enemyCharacter.FindPart(PartData.Type.Tail);
                    if (enemyTest.currentPart != null)
                    {
                        if ((enemyTarget.type == PartData.Type.Tail ||
                        enemyTarget.type == PartData.Type.LeftLeg ||
                        enemyTarget.type == PartData.Type.RightLeg)
                        && enemyTest.currentPart.name == "The Serpent's Tail")
                        {
                            // if the current part is the serpent's tail
                            PartData playersNewTail = null;
                            int playersNewTailHP = 0;
                            bool playersNewTailIsDestroyed = true;
                            PartData enemysNewTail = null;
                            int enemysNewTailHP = 0;
                            bool enemysNewTailIsDestroyed = true;
                            PartData enemysNewLLeg = null;
                            int enemysNewLLegHP = 0;
                            bool enemysNewLLegIsDestroyed = true;
                            PartData enemysNewRLeg = null;
                            int enemysNewRLegHP = 0;
                            bool enemysNewRLegIsDestroyed = true;

                            playersNewTail = enemyTest.currentPart;
                            playersNewTailHP = enemyTest.currentHP;
                            playersNewTailIsDestroyed = enemyTest.destroyed;

                            PartScript playerTail = playerCharacter.FindPart(PartData.Type.Tail);
                            PartScript playerLLeg = playerCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript playerRLeg = playerCharacter.FindPart(PartData.Type.RightLeg);
                            PartScript enemyTail = enemyCharacter.FindPart(PartData.Type.Tail);
                            PartScript enemyLLeg = enemyCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript enemyRLeg = enemyCharacter.FindPart(PartData.Type.RightLeg);
                            if (playerTail.currentPart != null)
                            {
                                enemysNewTail = playerTail.currentPart;
                                enemysNewTailHP = playerTail.currentHP;
                                enemysNewTailIsDestroyed = playerTail.destroyed;
                            }
                            if (playerLLeg.currentPart != null)
                            {
                                enemysNewLLeg = playerLLeg.currentPart;
                                enemysNewLLegHP = playerLLeg.currentHP;
                                enemysNewLLegIsDestroyed = playerLLeg.destroyed;
                            }
                            if (playerRLeg.currentPart != null)
                            {
                                enemysNewRLeg = playerRLeg.currentPart;
                                enemysNewRLegHP = playerRLeg.currentHP;
                                enemysNewRLegIsDestroyed = playerRLeg.destroyed;
                            }
                            // finally swap
                            playerTail.Swap(playersNewTail, playersNewTailHP, playersNewTailIsDestroyed);
                            playerLLeg.Swap(null, 0, true);
                            playerRLeg.Swap(null, 0, true);
                            enemyTail.Swap(enemysNewTail, enemysNewTailHP, enemysNewTailIsDestroyed);
                            enemyLLeg.Swap(enemysNewLLeg, enemysNewLLegHP, enemysNewLLegIsDestroyed);
                            enemyRLeg.Swap(enemysNewRLeg, enemysNewRLegHP, enemysNewRLegIsDestroyed);
                            AddLogText("The Boss swaps lower bodies with you.");
                            break;
                        }
                    }
                    if (playerTest.currentPart != null)
                    {
                        if ((enemyTarget.type == PartData.Type.Tail ||
                        enemyTarget.type == PartData.Type.LeftLeg ||
                        enemyTarget.type == PartData.Type.RightLeg) 
                        && playerTest.currentPart.name == "The Serpent's Tail")
                        {
                            // if the target is the serpent's tail
                            PartData enemysNewTail = null;
                            int enemysNewTailHP = 0;
                            bool enemysNewTailIsDestroyed = true;
                            PartData playersNewTail = null;
                            int playersNewTailHP = 0;
                            bool playersNewTailIsDestroyed = true;
                            PartData playersNewLLeg = null;
                            int playersNewLLegHP = 0;
                            bool playersNewLLegIsDestroyed = true;
                            PartData playersNewRLeg = null;
                            int playersNewRLegHP = 0;
                            bool playersNewRLegIsDestroyed = true;

                            enemysNewTail = playerTest.currentPart;
                            enemysNewTailHP = playerTest.currentHP;
                            enemysNewTailIsDestroyed = playerTest.destroyed;

                            PartScript playerTail = playerCharacter.FindPart(PartData.Type.Tail);
                            PartScript playerLLeg = playerCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript playerRLeg = playerCharacter.FindPart(PartData.Type.RightLeg);
                            PartScript enemyTail = enemyCharacter.FindPart(PartData.Type.Tail);
                            PartScript enemyLLeg = enemyCharacter.FindPart(PartData.Type.LeftLeg);
                            PartScript enemyRLeg = enemyCharacter.FindPart(PartData.Type.RightLeg);
                            if (enemyTail.currentPart != null)
                            {
                                playersNewTail = enemyTail.currentPart;
                                playersNewTailHP = enemyTail.currentHP;
                                playersNewTailIsDestroyed = enemyTail.destroyed;
                            }
                            if (enemyLLeg.currentPart != null)
                            {
                                playersNewLLeg = enemyLLeg.currentPart;
                                playersNewLLegHP = enemyLLeg.currentHP;
                                playersNewLLegIsDestroyed = enemyLLeg.destroyed;
                            }
                            if (enemyRLeg.currentPart != null)
                            {
                                playersNewRLeg = enemyRLeg.currentPart;
                                playersNewRLegHP = enemyRLeg.currentHP;
                                playersNewRLegIsDestroyed = enemyRLeg.destroyed;
                            }
                            // finally swap
                            enemyTail.Swap(enemysNewTail, enemysNewTailHP, enemysNewTailIsDestroyed);
                            enemyLLeg.Swap(null, 0, true);
                            enemyRLeg.Swap(null, 0, true);
                            playerTail.Swap(playersNewTail, playersNewTailHP, playersNewTailIsDestroyed);
                            playerLLeg.Swap(playersNewLLeg, playersNewLLegHP, playersNewLLegIsDestroyed);
                            playerRLeg.Swap(playersNewRLeg, playersNewRLegHP, playersNewRLegIsDestroyed);
                            AddLogText("The Boss swaps lower bodies with you.");
                            break;
                        }
                    }
                    // bau
                    PartData enemyPart = null;
                    int enemyHP = 0;
                    bool enemyIsDestroyed = true;
                    PartData playerPart = null;
                    int playerHP = 0;
                    bool playerIsDestroyed = true;
                    if (enemyTarget.currentPart != null)
                    {
                        playerPart = enemyTarget.currentPart;
                        playerHP = enemyTarget.currentHP;
                        playerIsDestroyed = enemyTarget.destroyed;
                    }
                    PartScript enemyPartScript = enemyCharacter.FindPart(enemyTarget.type);
                    if (enemyPartScript.currentPart != null)
                    {
                        enemyPart = enemyPartScript.currentPart;
                        enemyHP = enemyPartScript.currentHP;
                        enemyIsDestroyed = enemyPartScript.destroyed;
                    }
                    enemySource.gameObject.GetComponent<ManaScript>().UseMana(manaCost);
                    // and finally swap
                    enemyTarget.Swap(enemyPart, enemyHP, enemyIsDestroyed);
                    enemyPartScript.Swap(playerPart, playerHP, playerIsDestroyed);
                    AddLogText("The Boss swaps " + enemyTarget.type.ToString() + "s with you.");
                    break;
                // resolve SEE action
                case PartData.Ability.See:
                    // eyes animation here
                    enemySource.gameObject.GetComponent<ManaScript>().ExposeEyes();
                    AddLogText("The Boss gazes into your eyes, attempting to reach your soul.");
                    bool success = GazeOfChaos();
                    if (success)
                    {
                        AddLogText("The Boss succeeds and your soul is lost.");
                        PartScript part = playerCharacter.FindPart(PartData.Type.Body);
                        part.Drain(part.currentPart.hp + 100);
                    }
                    else
                    {
                        AddLogText("The Boss fails and you are unaffected.");
                    }
                    break;
                // resolve NOTHING action
                case PartData.Ability.Nothing:
                    AddLogText("The Boss does nothing.");
                    break;
                default: break;
            }

            // end of turn
            ClearAll();
            playerCharacter.RegenerateMana();
            enemyCharacter.RegenerateMana();
        }
    }

    private void ClearAll()
    {
        // clear all options
        ClearPlayerAction();
        ClearPlayerSource();
        ClearPlayerTarget();

        // clear all text
        for (int i = 0; i < playerInfo.Count-1; i++)
        {
            playerInfo[i].text = string.Empty;
        }
        playerInfo[5].gameObject.SetActive(false);

        for (int i = 0; i < enemyInfo.Count - 1; i++)
        {
            enemyInfo[i].text = string.Empty;
        }
        enemyInfo[5].gameObject.SetActive(false);

        desc.text = string.Empty;

        // disable all action buttons
        foreach (ActionButtonScript button in buttons)
        {
            button.Disable();
        }
    }

    private void SetWarning(string warning)
    {
        warningText.text = warning;
        warningUI.SetActive(true);
    }

    private void ClearLogText()
    {
        logText.text = string.Empty;
    }

    public void AddLogText(string log)
    {
        logText.text += log + "\n";
    }

    private bool GazeOfChaos()
    {
        float r = Random.Range(0f, 100f);
        if (r < gazeThreshhold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
