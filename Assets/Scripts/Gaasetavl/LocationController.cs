using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    public List<List<GameObject>> posList = new List<List<GameObject>>();
    [SerializeField] GameObject posPrefab;
    //[SerializeField] GameObject posVisualPrefab;
    [SerializeField] GameObject board;
    [SerializeField] GameObject bPawnPrefab;
    [SerializeField] GameObject wPawnPrefab;

    // Start is called before the first frame update
    void Start()
    {
        float pawnDis = 28f / 13f;
        float oddOffset = pawnDis;
        float widthFactor = 2f;

        for (int i = 0; i < 13; i++)
        {
            posList.Add(new List<GameObject>());
            if (i % 2 == 0) //For even rows(row 0, 2, 4 etc.)
            {
                for (int j = 0; j < 7; j++)
                {
                    posList[i].Add(null);
                }

                if (i >= 4 && i <= 8) //For the middle long part
                {
                    for (int j = 0; j < 7; j++)
                    {
                        //Calculate position
                        float _currX = i * pawnDis;
                        float _currZ = j * pawnDis * widthFactor;

                        createBoardPos(_currX, _currZ,i,j);
                    }
                } else //for the outer short parts
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (j >= 2 && j <= 4)
                        {
                            //Calculate position
                            float _currX = i * pawnDis;
                            float _currZ = j * pawnDis * widthFactor;

                            createBoardPos(_currX, _currZ, i, j);
                        } else
                        {
                            continue;
                        }
                    }
                }
            } else //For odd rows(1, 3, 5 etc.)
            {
                for (int j = 0; j < 6; j++)
                {
                    posList[i].Add(null);
                }

                if (i >= 4 && i <= 8) //For the middle long part
                {
                    for (int j = 0; j < 6; j++)
                    {
                        //Calculate position
                        float _currX = i * pawnDis;
                        float _currZ = j * pawnDis * widthFactor+oddOffset;

                        createBoardPos(_currX, _currZ, i, j);
                    }
                }
                else //for the outer short parts
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (j >= 2 && j <= 3)
                        {
                            //Calculate position
                            float _currX = i * pawnDis;
                            float _currZ = j * pawnDis*widthFactor+oddOffset;

                            createBoardPos(_currX, _currZ, i, j);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
        string stringToPrint = "";
        for (int i = 0; i < posList.Count; i++)
        {
            stringToPrint += "\n";
            for (int j = 0; j < posList[i].Count; j++)
            {
                if (posList[i][j] != null)
                {
                    stringToPrint += posList[i][j].name;
                } else
                {
                    stringToPrint += "null";
                }
            }
        }
        Debug.Log(stringToPrint);
    }


    void createBoardPos(float x, float z, int i, int j)
    {
        (int, int) pos = (i, j);
        //instansiate object
        GameObject currObj = Instantiate(posPrefab, new Vector3(x,0.5f,z),Quaternion.Euler(0,0,0));
        currObj.transform.SetParent(board.transform);

        PosClass pawnClass = currObj.GetComponent<PosClass>();
        if (pawnClass != null)
        {
            pawnClass.setListX(i);
            pawnClass.setListY(j);
        }

        if (pos == (0,2) || pos == (0, 4))
        {
            createPawn(wPawnPrefab,currObj,i,j,pawnClass);
        }
        else if (i > 3 && i % 2 == 0)
        {
            createPawn(bPawnPrefab, currObj,i,j,pawnClass);
        }

        posList[i][j] = currObj;
    }

    void createPawn(GameObject prefab,GameObject posObj,int i, int j, PosClass currPosObj)
    {
        if (posObj != null) //just a safe null check
        {
            GameObject currObj = Instantiate(prefab, posObj.transform.position, Quaternion.Euler(0,0,0));
            currObj.transform.SetParent(board.transform);
            PawnClass pawnClass = currObj.GetComponent<PawnClass>();
            pawnClass.setListX(i);
            pawnClass.setListY(j);
            
            currPosObj.setOccupied(true);
        }
    }
}
