using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public BattleScript battleScript;

    private PartScript defenseSource;
    private bool shouldSwapBody;

    public bool DefendOrNot()
    {
        bool eval = false;
        shouldSwapBody = false;
        if (EvaluateDefense() == 1f)
        {
            eval = true;
            battleScript.SetEnemySource(defenseSource);
            battleScript.SetEnemyAction(PartData.Ability.Defend);
        }
        return eval;
    }

    public void TakeAction()
    {
        // should do necessary checks
        // mana checks
        // null swap check
        // bitten once check
        // made all selections
        // target not destroyed on attack and bite actions

        EvaluateOffense();

    }

    private float EvaluateDefense()
    {
        float eval = float.NegativeInfinity;
        int highestAtk = 0;
        float highestDefVal = float.NegativeInfinity;
        // determine highest atk
        foreach (PartScript part in battleScript.playerCharacter.parts)
        {
            if (!part.destroyed)
            {
                if (part.currentPart.atk > highestAtk)
                {
                    highestAtk = part.currentPart.atk;
                }
            }
        }
        // determine if defense is worth it
        PartScript body = battleScript.enemyCharacter.FindPart(PartData.Type.Body);
        if (body.currentHP < (1.5 * (highestAtk-body.currentPart.def)))
        {
            // if swapping is possible
            PartScript head = battleScript.enemyCharacter.FindPart(PartData.Type.Head);
            if (!head.destroyed)
            {
                if (head.currentPart.name == "Sowilo's Head" && head.gameObject.GetComponent<ManaScript>().currentMana >= 5)
                {
                    shouldSwapBody = true;
                    return eval;
                }
            }
            // else
            foreach (PartScript part in battleScript.enemyCharacter.parts)
            {
                // determines if part is capable of defense
                if (!part.destroyed)
                {
                    bool canDefend = false;
                    foreach (PartData.Ability action in part.currentPart.abilities)
                    {
                        if (action == PartData.Ability.Defend)
                        {
                            canDefend = true;
                        }
                    }
                    if (canDefend)
                    {
                        // chooses which to defend with
                        float defval = part.currentHP + part.currentPart.def;
                        if (defval > highestDefVal)
                        {
                            highestDefVal = defval;
                            defenseSource = part;
                            eval = 1f;
                        }
                    }
                }
            }
        }

        return eval;
    }

    private void EvaluateOffense()
    {
        List<PartScript> actionParts = new List<PartScript>();
        List<PartData.Ability> actions = new List<PartData.Ability>();
        PartScript selectedPart = null;
        PartData.Ability selectedAction = PartData.Ability.Nothing;
        PartScript selectedTarget = null;
        int highestAtk = int.MinValue;
        PartScript enemyHighestATKPart = null;
        PartData.Ability enemyHighestATKAction = PartData.Ability.Nothing;
        int choice = -1;
        bool needTarget = false;
        List<PartScript> potentialSwaps = new List<PartScript>();

        foreach (PartScript part in battleScript.enemyCharacter.parts)
        {
            if (!part.destroyed)
            {
                bool canSwap = false;
                bool canBite = false;
                bool canAttack = false;
                bool canSee = false;
                bool canAreaAttack = false;

                foreach (PartData.Ability action in part.currentPart.abilities)
                {
                    if (action == PartData.Ability.Attack)
                    {
                        canAttack = true;
                    }
                    else if (action == PartData.Ability.AreaAttack)
                    {
                        canAreaAttack = true;
                    }
                    // mana check
                    else if (action == PartData.Ability.Swap &&
                        battleScript.enemyCharacter.FindPart(PartData.Type.Head).gameObject.GetComponent<ManaScript>().currentMana >= 5)
                    {
                        canSwap = true;
                    }
                    // bite check
                    else if (action == PartData.Ability.Bite && !battleScript.hasEnemyBitten)
                    {
                        canBite = true;
                    }
                    // eye check
                    else if (action == PartData.Ability.See &&
                        !battleScript.playerCharacter.FindPart(PartData.Type.Head).destroyed)
                    {
                        canSee = true;
                    }
                }
                // eval choices
                if (canSwap && shouldSwapBody)
                {
                    selectedPart = part;
                    selectedAction = PartData.Ability.Swap;
                    selectedTarget = battleScript.playerCharacter.FindPart(PartData.Type.Body);
                }
                else if (canBite && part.currentHP < part.currentPart.hp / 5)
                {
                    selectedPart = part;
                    selectedAction = PartData.Ability.Bite;
                    needTarget = true;
                }
                else if (canSwap)
                {
                    actionParts.Add(part);
                    actions.Add(PartData.Ability.Swap);
                }
                else if (canSee)
                {
                    actionParts.Add(part);
                    actions.Add(PartData.Ability.See);
                }
                else if (canAreaAttack)
                {
                    actionParts.Add(part);
                    actions.Add(PartData.Ability.AreaAttack);
                    if (part.currentPart.atk >= highestAtk)
                    {
                        highestAtk = part.currentPart.atk;
                        enemyHighestATKAction = PartData.Ability.AreaAttack;
                        enemyHighestATKPart = part;
                    }
                }
                else if (canAttack)
                {
                    if (part.currentPart.atk >= highestAtk)
                    {
                        highestAtk = part.currentPart.atk;
                        enemyHighestATKAction = PartData.Ability.Attack;
                        enemyHighestATKPart = part;
                    }
                }

            }
            else if (part.currentPart != null && part.currentPart.name == "Sowilo's Head")
            {
                selectedPart = part;
                selectedAction = PartData.Ability.Bite;
                needTarget = true;
            }
            else
            {
                potentialSwaps.Add(part);
            }
        }

        // add attack part
        if (enemyHighestATKAction != PartData.Ability.AreaAttack || enemyHighestATKAction != PartData.Ability.Nothing)
        {
            actions.Add(enemyHighestATKAction);
            actionParts.Add(enemyHighestATKPart);
        }

        choose:
        // choose action
        if (selectedPart == null)
        {
            needTarget = false;
            if (actions.Count > 0)
            {
                choice = Random.Range(0, actionParts.Count);
                selectedAction = actions[choice];
                selectedPart = actionParts[choice];

                if (selectedAction == PartData.Ability.Attack ||
                    selectedAction == PartData.Ability.Swap)
                {
                    needTarget = true;
                }
            }
            else
            {
                selectedAction = PartData.Ability.Nothing;
                selectedPart = battleScript.enemyCharacter.FindPart(PartData.Type.Body);
            }
        }

        // evaluate target
        if (needTarget)
        {
            PartScript highestPlayerATKPart = null;
            PartScript LowestPlayerHPPart = null;
            PartScript playerBody = null;
            List<PartScript> pps = new List<PartScript>();
            int highestPlayerATK = int.MinValue;
            int lowestPlayerHP = int.MaxValue;

            // gather info
            foreach (PartScript part in battleScript.playerCharacter.parts)
            {
                if (part.type == PartData.Type.Body)
                {
                    playerBody = part;
                }

                if (!part.destroyed)
                {
                    pps.Add(part);
                    if (part.currentPart.atk > highestPlayerATK)
                    {
                        highestPlayerATK = part.currentPart.atk;
                        highestPlayerATKPart = part;
                    }
                    if (part.currentHP < lowestPlayerHP)
                    {
                        lowestPlayerHP = part.currentHP;
                        LowestPlayerHPPart = part;
                    }
                }
            }

            // target for swap
            if (selectedAction == PartData.Ability.Swap)
            {
                List<PartScript> swaps = new List<PartScript>();
                if (potentialSwaps.Count > 0)
                {
                    foreach (PartScript part in potentialSwaps)
                    {
                        // null swap check
                        PartScript pp = battleScript.playerCharacter.FindPart(part.type);
                        if (!pp.destroyed)
                        {
                            swaps.Add(pp);
                        }
                    }

                    if (swaps.Count > 0)
                    {
                        int ha = int.MinValue;

                        // exception for the tail
                        PartScript et = battleScript.enemyCharacter.FindPart(PartData.Type.Tail);
                        bool st = false;
                        if (et.currentPart != null && et.currentPart.name == "The Serpent's Tail" && 
                            !et.destroyed)
                        {
                            st = true;
                        }

                        foreach (PartScript part in swaps)
                        {
                            if (st && (part.type == PartData.Type.RightLeg ||
                                part.type == PartData.Type.LeftLeg ||
                                part.type == PartData.Type.Tail))
                            {
                                continue;
                            }
                            else if (part.currentPart.atk > ha)
                            {
                                ha = part.currentPart.atk;
                                selectedTarget = part;
                            }
                        }
                    }
                    else
                    {
                        PartScript ep = battleScript.enemyCharacter.FindPart(highestPlayerATKPart.type);
                        if (highestPlayerATK > ep.currentPart.atk)
                        {
                            selectedTarget = highestPlayerATKPart;
                        }
                        else
                        {
                            selectedPart = null; 
                            selectedTarget = null;
                            selectedAction = PartData.Ability.Nothing;
                            goto choose;
                        }
                    }
                }
                else
                {
                    PartScript ep = battleScript.enemyCharacter.FindPart(highestPlayerATKPart.type);
                    if (highestPlayerATK > ep.currentPart.atk)
                    {
                        selectedTarget = highestPlayerATKPart;
                    }
                    else
                    {
                        selectedPart = null;
                        selectedTarget = null;
                        selectedAction = PartData.Ability.Nothing;
                        goto choose;
                    }
                }
            }
            // target for bite
            else if (selectedAction == PartData.Ability.Bite)
            {
                if (playerBody.currentHP <= selectedPart.currentPart.hp)
                {
                    selectedTarget = playerBody;
                }
                else
                {
                    selectedTarget = LowestPlayerHPPart;
                }
            }
            // target for attack
            else
            {
                int bodyDMG = selectedPart.currentPart.atk - playerBody.currentPart.def;
                int HAPDMG = selectedPart.currentPart.atk - highestPlayerATKPart.currentPart.def;
                int LHPDMG = selectedPart.currentPart.atk - LowestPlayerHPPart.currentPart.def;
                if (LHPDMG >= lowestPlayerHP)
                {
                    selectedTarget = LowestPlayerHPPart;
                }
                else if (HAPDMG > highestPlayerATKPart.currentPart.hp/2)
                {
                    selectedTarget = highestPlayerATKPart;
                }
                else
                {
                    selectedTarget = pps[Random.Range(0,pps.Count)];
                }
            }
        }

        // set choices
        battleScript.SetEnemyAction(selectedAction);
        battleScript.SetEnemySource(selectedPart);
        battleScript.SetEnemyTarget(selectedTarget);
    }
}
