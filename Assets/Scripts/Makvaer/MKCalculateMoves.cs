using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class MKCalculateMoves : MonoBehaviour
{
    public List<List<GameObject>> calculatePossibleMoves(PosClass currPos, GameObject selectedPawn, List<List<List<GameObject>>> posList, string teamPawn, string oponentPawn)
    {
        int currX = currPos.getListX();
        int currY = currPos.getListY();
        List<List<GameObject>> calculatedMoves = new List<List<GameObject>>();

        List<List<int>> movesToCheck = new List<List<int>>()
        {
            new List<int> {currX-1,currY-1},//up left
            new List<int> {currX+1,currY-1},//down left
            new List<int> {currX-1,currY+1},//up right
            new List<int> {currX+1,currY+1},//down right
            new List<int> {currX,currY-2},//Left
            new List<int> {currX+2,currY},//Down
            new List<int> {currX,currY+2},//right
            new List<int> {currX-2,currY},//up
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
                        if (posList[currListX][currListY][1] == null && i < 4)
                        {

                            List<GameObject> data = new List<GameObject>()
                            {
                                currPosObj,
                                null
                            };

                            calculatedMoves.Add(data);
                            currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                        }
                        else if (posList[currListX][currListY][1] != null)
                        {
                            if (posList[currListX][currListY][1].CompareTag(oponentPawn))
                            {
                                List<GameObject> data = calculateAttackMoves(
                                posList[currListX][currListY][1],
                                currX,
                                currY,
                                currListX,
                                currListY,
                                i,
                                posList);
                                if (data != null) calculatedMoves.Add(data);
                            }
                            
                        }
                    }
                }
            }
            else continue; 
        }

        return calculatedMoves;
    }

    public List<GameObject> calculateAttackMoves(GameObject blackPawn, int currX, int currY, int currListX, int currListY, int listIteration, List<List<List<GameObject>>> posList)
    {
        List<List<int>> attackMovesToCheck = new List<List<int>>()
        {
            new List<int> {currX-2,currY-2},//up left
            new List<int> {currX+2,currY-2},//down left
            new List<int> {currX-2,currY+2},//up right
            new List<int> {currX+2,currY+2},//down right
            new List<int> {currX,currY-4},//Left
            new List<int> {currX+4,currY},//Down
            new List<int> {currX,currY+4},//right
            new List<int> {currX-4,currY},//up
        };

        List<GameObject> attackMove = new List<GameObject>();

        int currListXHere = attackMovesToCheck[listIteration][0];
        int currListYHere = attackMovesToCheck[listIteration][1];

        if (currListXHere >= 0 && currListYHere >= 0 && currListXHere < posList.Count && currListYHere < posList[currListXHere].Count && posList[currListXHere][currListYHere][1] == null && blackPawn != null && posList[currListXHere][currListYHere][0] != null)
        {
            attackMove.Add(posList[currListXHere][currListYHere][0]);
            attackMove.Add(blackPawn);
            posList[currListXHere][currListYHere][0].GetComponent<MeshRenderer>().material.color = Color.red;

            return attackMove;
        }
        else return null;
    }

    public List<List<GameObject>> calculatePossibleDamMoves(PosClass currPos, GameObject selectedPawn, List<List<List<GameObject>>> posList, string teamPawn, string oponentPawn)
    {
        int currX = currPos.getListX();
        int currY = currPos.getListY();
        List<List<GameObject>> calculatedMoves = new List<List<GameObject>>();
        int moveOffset = 0;

        for (int i = 0; i < 6; i++)
        {
            moveOffset += 1;

            List<List<int>> movesToCheck = new List<List<int>>()
            {
                new List<int> {currX-moveOffset,currY-moveOffset},//up left
                new List<int> {currX+moveOffset,currY-moveOffset},//down left
                new List<int> {currX-moveOffset,currY+moveOffset},//up right
                new List<int> {currX+moveOffset,currY+moveOffset},//down right
                new List<int> {currX,currY-moveOffset*2},//Left
                new List<int> {currX+moveOffset*2,currY},//Down
                new List<int> {currX,currY+moveOffset*2},//right
                new List<int> {currX-moveOffset*2,currY},//up
            };
            for (int j = 0; j < movesToCheck.Count; j++)
            {
                int currListX = movesToCheck[j][0];
                int currListY = movesToCheck[j][1];

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
                            else if (posList[currListX][currListY][1].CompareTag(oponentPawn))
                            {
                                List<GameObject> data = calculateDamAttackMoves(
                                    posList[currListX][currListY][1],
                                    currX,
                                    currY,
                                    currListX,
                                    currListY,
                                    j,
                                    posList);
                                if (data != null) calculatedMoves.Add(data);
                            }
                        }
                    }
                }
                else continue;
            }
        }

        return calculatedMoves;
    }
    public List<GameObject> calculateDamAttackMoves(GameObject blackPawn, int currX, int currY, int currListX, int currListY, int listIteration, List<List<List<GameObject>>> posList)
    {

    }

}