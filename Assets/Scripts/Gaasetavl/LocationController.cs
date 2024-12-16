using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    public PythonReceiver receivePos;
    public List<List<List<GameObject>>> posList = new List<List<List<GameObject>>>();
    [SerializeField] GameObject posPrefab;
    //[SerializeField] GameObject posVisualPrefab;
    [SerializeField] GameObject board;
    [SerializeField] GameObject bPawnPrefab;
    [SerializeField] GameObject wPawnPrefab;

    // Start is called before the first frame update
    void Start()
    {
        float pawnDis = 28f / 13f; //size of the board Divided with the number of pawns in a row
        float oddOffset = pawnDis; //in gaasetavl every other row is indended, this is the offset
        float widthFactor = 2f; // to strecth it to the full width of the board

        for (int i = 0; i < 13; i++)
        {
            posList.Add(new List<List<GameObject>>());
            if (i % 2 == 0) //For even rows(row 0, 2, 4 etc.)
            {
                for (int j = 0; j < 7; j++)
                {
                    posList[i].Add(new List<GameObject> { null, null });
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
                    posList[i].Add(new List<GameObject> { null, null });
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
                if (posList[i][j][0] != null)
                {
                    stringToPrint += posList[i][j][0].name; //This line says nullreference expection
                } else
                {
                    stringToPrint += "null";
                }
            }
        }
        //Debug.Log(stringToPrint);
    }


    void createBoardPos(float x, float z, int i, int j)
    {
        (int, int) pos = (i, j);
        //instansiate object
        GameObject currObj = Instantiate(posPrefab, new Vector3(x,0f,z),Quaternion.Euler(0,0,0));
        currObj.transform.SetParent(board.transform);

        PosClass posClass = currObj.GetComponent<PosClass>();
        if (posClass != null)
        {
            posClass.setListX(i);
            posClass.setListY(j);
        }

        foreach (Vector3 pawnPos in receivePos.whitePosition)
        {
            if(pawnPos.x == i && pawnPos.z == j)
            {
                createPawn(wPawnPrefab, currObj, i, j, posClass, 0.1f, 90f);
            }
        }
        foreach (Vector3 pawnPos in receivePos.blackPositions)
        {
            if (pawnPos.x == i && pawnPos.z == j)
            {
                createPawn(bPawnPrefab, currObj, i, j, posClass, 0.1f, -90f);
            }
        }
        //if (pos == (0,2) || pos == (0, 4)) // dette burde relativt nemt kunne laves om til at tage imod en liste med exempelvis string "white" og "black" og så tjekke hvis den liste pos == white lav en vid og vice versa med sort
        //{
        //    createPawn(wPawnPrefab,currObj,i,j,posClass,0.1f,90f);
        //}
        //else if (i > 4 && i % 2 == 0)
        //{
        //    createPawn(bPawnPrefab, currObj,i,j,posClass,0.6f,-90f);
        //}

        posList[i][j][0] = currObj;
    }

    void createPawn(GameObject prefab,GameObject posObj,int i, int j, PosClass currPosObj, float yOffset, float yROffset)
    {
        if (posObj != null) //just a safe null check
        {
            GameObject currObj = Instantiate(prefab,new Vector3(posObj.transform.position.x,posObj.transform.position.y+yOffset,posObj.transform.position.z), Quaternion.Euler(0,yROffset,0));
            currObj.transform.SetParent(board.transform);
            PawnClass pawnClass = currObj.GetComponent<PawnClass>();
            pawnClass.setListX(i);
            pawnClass.setListY(j);

            posList[i][j][1] = currObj;
        }
    }
}
