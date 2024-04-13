using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SigilHandler : MonoBehaviour
{
    bool active = false;
    bool draw = false;
    int lastPoint = -1;
    bool empty = true;
    List<int> activeLines = new List<int>();
    public SigilMaster master;
    public GameObject points;
    public GameObject[] sigilLines;
    LineDataClass lineData;
    public TextAsset jsonLineData;

    void Start()
    {
        lineData = LineDataClass.ParseDataFromJSON(jsonLineData.ToString());
    }

    private void Update()
    {
        if (active)
        {
            if (Input.GetMouseButtonUp(1) && !empty)
            {
                finishSigil();
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2))
            {
                startOver();
            }
        }
    }

    public void activate()
    {
        active = true;
        empty = true;
        endDraw();
        activeLines.Clear();
        showPoints();
        hideAllLines();
        gameObject.SetActive(true);
    }

    public void deactivate()
    {
        active = false;
        endDraw();
        hidePoints();
    }

    public void clearLines()
    {
        hideAllLines();
    }

    public void hide()
    {
        deactivate();
        gameObject.SetActive(false);
    }

    public void beginDraw(int point)
    {
        if (active)
        {
            draw = true;
            lastPoint = point;
        }
    }

    public void addPoint(int point)
    {
        if (draw == true && point != lastPoint)
        {
            bool fin;
            if (lastPoint < point)
            {
                fin = findLine(lastPoint, point);
            }
            else
            {
                fin = findLine(point, lastPoint);
            }
            if (fin) {lastPoint = point;}
        }
    }

    private bool findLine(int p1, int p2)
    {
        FirstPointData lineList = lineData.firstPoints[p1];
        int lineNum = 0;
        GameObject line = null;
        for(int i = 0; i < lineList.secondPoints.Count; i++)
        {
            if (lineList.secondPoints[i].secondPoint == p2)
            {
                lineNum = lineList.secondPoints[i].lineNumber;
                line = sigilLines[lineNum];
                break;
            }
        }
        if (line != null && !line.activeSelf)
        {
            if (master.useMana(1))
            {
                line.SetActive(true);
                activeLines.Add(lineNum);
                empty = false;
                return true;
            }
            else
            {
                // when there is no mana
                return false;
            }
        }
        else if (line != null && line.activeSelf)
        {
            return true;
        }
        else return false;
    }

    public void endDraw()
    {
        draw = false;
        lastPoint = -1;
    }

    private void finishSigil()
    {
        deactivate();
        activeLines.Sort();
        master.sendLines(activeLines);
        master.changePhase();
    }

    private void startOver()
    {
        endDraw();
        hideAllLines();
    }

    private void hideAllLines()
    {
        for (int i = 0; i < sigilLines.Length; i++)
        {
            sigilLines[i].SetActive(false);
        }
        activeLines.Clear();
        empty = true;
    }

    public void hidePoints()
    {
        points.SetActive(false);
    }

    public void showPoints()
    {
        points.SetActive(true);
    }
}
