using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    [Serializable]
    public struct ScenarioData
    {
        public string Name;
        public TextAsset scenarioFile;
    }

    [SerializeField]
    public List<ScenarioData> scenarioRecord;
    public GameObject mainUI;
    public TMP_Text dialogueUIText;
    public Button dialogueButton;
    public GameObject choiceUI;
    public List<GameObject> choiceButtons;
    public List<GameObject> speakers;

    private Stack<Vector3Int> diaPosStk;
    private Scenario currentScenario;


    private void Start()
    {
        OverlordScript.inst.onEnemyDeath += PlayEndDialogue;
        diaPosStk = new Stack<Vector3Int>();
    }

    public void PlayEndDialogue()
    {
        if (scenarioRecord.Count > 2)
        {
            PlayScenario(1);
        }
    }

    public void PlayRandomKickDialogue()
    {
        if (!dialogueButton.enabled && mainUI.gameObject.activeSelf)
        {
            return;
        }
        else
        {
            // input total num of sets in latter range
            PlayScenario(scenarioRecord.Count - 1, UnityEngine.Random.Range(0, 19));
            AudioScript.inst.PlayKickSound();
        }
    }

    public void PlayScenario(int scenarioIndex)
    {
        mainUI.gameObject.SetActive(true);
        diaPosStk.Push(new Vector3Int(scenarioIndex, 0, -1));
        currentScenario = Scenario.ParseJSON(scenarioRecord[scenarioIndex].scenarioFile.ToString());

        PlayNextDialogue();
    }

    public void PlayScenario(int scenarioIndex, int diaSetIndex)
    {
        mainUI.gameObject.SetActive(true);
        diaPosStk.Push(new Vector3Int(scenarioIndex, diaSetIndex, -1));
        currentScenario = Scenario.ParseJSON(scenarioRecord[scenarioIndex].scenarioFile.ToString());

        PlayNextDialogue();
    }

    public void PlayNextDialogue()
    {
        dialogueButton.enabled = false;


        Vector3Int currentDiaPos = diaPosStk.Pop();
        currentDiaPos.z++;
        var diaSet = currentScenario.DialogueSets[currentDiaPos.y];

        // if end of scenario reached
        if (currentDiaPos.z >= diaSet.DialoguePieces.Count && diaPosStk.Count == 0)
        {
            EndScenario();
            return;
        }
        else if (currentDiaPos.z >= diaSet.DialoguePieces.Count)
        {
            // if different scenarios
            if (currentDiaPos.x != diaPosStk.Peek().x)
            {
                currentScenario = Scenario.ParseJSON(scenarioRecord[diaPosStk.Peek().x].scenarioFile.ToString());
                PlayNextDialogue();
                return;
            }
            else
            {
                PlayNextDialogue();
                return;
            }
        }

        var diaPiece = diaSet.DialoguePieces[currentDiaPos.z];

        // action by type
        switch (diaPiece.Type)
        {
            case 0:
                {
                    diaPosStk.Push(currentDiaPos);

                    foreach (GameObject s in speakers)
                    {
                        s.SetActive(false);
                    }

                    switch (diaPiece.Title)
                    {
                        case "Sowilo":
                            {
                                speakers[0].SetActive(true);
                                break;
                            }
                        case "Boss":
                            {
                                speakers[1].SetActive(true);
                                break;
                            }
                        case "Serpent":
                            {
                                if (PlayerPrefs.GetInt("TrueName", 0) == 0)
                                {
                                    speakers[2].SetActive(true);
                                }
                                else
                                {
                                    speakers[3].SetActive(true);
                                }
                                break;
                            }
                        case "Void":
                            {
                                speakers[4].SetActive(true);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    StartCoroutine(DisplayDialogueText(diaPiece.Text));
                    break;
                }
            case 1:
                {
                    diaPosStk.Push(currentDiaPos);
                    DisplayChoices();
                    break;
                }
            case 2:
                {
                    // start battle
                    OverlordScript.inst.StartBattle();
                    EndScenario();
                    break;
                }
            case 3:
                {
                    diaPosStk.Push(currentDiaPos);
                    // commands without arguments
                    OverlordScript.inst.SendMessage(diaPiece.Title);
                    PlayNextDialogue();
                    break;
                }
            case 4:
                {
                    // load next level
                    PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
                    LevelLoader.Instance.LoadNextScene();
                    break;
                }
            case 5:
                {
                    diaPosStk.Push(currentDiaPos);
                    AddBranchToStk(int.Parse(diaPiece.Title));
                    PlayNextDialogue();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void AddBranchToStk(int branch)
    {
        diaPosStk.Push(new Vector3Int(diaPosStk.Peek().x, branch, -1));
    }

    private void DisplayChoices()
    {
        foreach (GameObject choice in choiceButtons)
        {
            choice.SetActive(false);
        }

        choiceUI.gameObject.SetActive(true);
        Vector3Int currentDiaPos = diaPosStk.Pop();
        var diaSet = currentScenario.DialogueSets[currentDiaPos.y];
        var diaPiece = diaSet.DialoguePieces[currentDiaPos.z];
        int index = 0;

        while (diaPiece.Type == 1)
        {
            choiceButtons[index].GetComponentInChildren<TMP_Text>().text = diaPiece.Text;
            choiceButtons[index].GetComponent<DiaChoiceButton>().choice = int.Parse(diaPiece.Title);
            choiceButtons[index].gameObject.SetActive(true);
            index++;
            currentDiaPos.z++;

            // when set ends with choices
            if (currentDiaPos.z >= diaSet.DialoguePieces.Count)
            {
                currentDiaPos.z--;
                diaPosStk.Push(currentDiaPos);
                return;
            }

            diaPiece = diaSet.DialoguePieces[currentDiaPos.z];
        }
        // when no longer choices
        currentDiaPos.z--;
        diaPosStk.Push(currentDiaPos);
    }

    private void EndScenario()
    {
        diaPosStk = new Stack<Vector3Int>();
        mainUI.gameObject.SetActive(false);
    }

    IEnumerator DisplayDialogueText(string line)
    {
        dialogueUIText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueUIText.text += c;
            yield return null;
        }
        dialogueButton.enabled = true;
    }

    public void AddDiaBranch(int set)
    {
        if (set != -1)
        {
            Vector3Int last = diaPosStk.Peek();
            diaPosStk.Push(new Vector3Int(last.x, set, -1));
        }
        foreach (GameObject b in choiceButtons)
        {
            b.SetActive(false);
        }
        choiceUI.gameObject.SetActive(false);
    }

}
