using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HHCalculateMoves : MonoBehaviour
{
    public List<List<GameObject>> calculatePossibleMoves(PosClass currPos, GameObject selectedPawn, List<List<List<GameObject>>> posList, string teamPawn, string oponentPawn)
    {
        int currX = currPos.getListX();
        int currY = currPos.getListY();
        List<List<GameObject>> calculatedMoves = new List<List<GameObject>>();

        List<List<int>> movesToCheck = new List<List<int>>()
        {
            new List<int> {currX-1,currY-1},//up left
            new List<int> {currX,currY-1}, //left
            new List<int> {currX+1,currY-1},//down left
            new List<int> {currX+1,currY}, //down
            new List<int> {currX-1,currY+1},//up right
            new List<int> {currX,currY+1}, //right
            new List<int> {currX+1,currY+1}, //down right
            new List<int> {currX-1,currY} //up
        };

        for (int i = 0; i < movesToCheck.Count; i++)
        {
            int currListX = movesToCheck[i][0];
            int currListY = movesToCheck[i][1];

            if (currListX >= 0 && currListX < posList.Count && currListY >= 0 && currListY < posList[currListX].Count)
            {
                GameObject currPosObj = posList[currListX][currListY][0];
                if (currPosObj != null)
                {
                    PosClass posClass = currPosObj.GetComponent<PosClass>();
                    if (posClass != null)
                    {
                        if (posList[currListX][currListY][1] == null)
                        {

                            List<GameObject> data = new List<GameObject>()
                            {
                                currPosObj,
                                null
                            };

                            calculatedMoves.Add(data);
                            currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                        }
                    }
                }
            }
            else continue;
        }

        return calculatedMoves;
    }
}
