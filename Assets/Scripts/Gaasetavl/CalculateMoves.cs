using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalculateMoves : MonoBehaviour
{
    [SerializeField] PawnMovement pawnM;

    public List<List<GameObject>> calculatePossibleMoves(PosClass currPos, GameObject selectedPawn, List<List<List<GameObject>>> posList, PawnMovement.state currState)
    {
        int currX = currPos.getListX();
        int currY = currPos.getListY();

        List<List<GameObject>> calculatedMoves = new List<List<GameObject>>();
        List<List<int>> movesToCheck;
        List<List<int>> evenMoves = new List<List<int>>()
        {
            //straights
            new List<int> {currX-2,currY},//up
            new List<int> {currX+2,currY},//down
            new List<int> {currX,currY-1},//left
            new List<int> {currX,currY+1},//right
            //diagonals
            new List<int> {currX-1,currY-1},//up left
            new List<int> {currX+1,currY-1},//down left
            new List<int> {currX-1,currY},//up right
            new List<int> {currX+1,currY}//down right
        };
        List<List<int>> oddMoves = new List<List<int>>()
        {
            //diagonals
            new List<int> {currX-1,currY},//up left
            new List<int> {currX+1,currY},//down left
            new List<int> {currX-1,currY+1},//up right
            new List<int> {currX+1,currY+1}//down right
        };

        if (currX % 2 == 0) movesToCheck = evenMoves; else movesToCheck = oddMoves;

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

                            if (selectedPawn.CompareTag("whitePawn"))
                            {
                                calculatedMoves.Add(data);
                                currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                            }
                            else if (selectedPawn.CompareTag("blackPawn") && currListX <= currX)
                            {
                                calculatedMoves.Add(data);
                                currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                            }
                        }
                        else if (posList[currListX][currListY][1].CompareTag("blackPawn") && currState == PawnMovement.state.WHITE_SELECT)
                        {
                            List<GameObject> data = calculateWhiteAttackMoves(
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
            else continue;

        }

        return calculatedMoves;
    }

    public List<GameObject> calculateWhiteAttackMoves(GameObject blackPawn, int currX, int currY, int currListX, int currListY, int listIteration, List<List<List<GameObject>>> posList)
    {
        List<List<int>> attackEvenMoves = new List<List<int>>()
        {
            //straights
            new List<int> {currListX-2,currListY},//up
            new List<int> {currListX+2,currListY},//down
            new List<int> {currListX,currListY-1},//left
            new List<int> {currListX,currListY+1},//right
            //diagonals
            new List<int> {currListX-1,currListY},//up left
            new List<int> {currListX+1,currListY},//down left
            new List<int> {currListX-1,currListY+1},//up right
            new List<int> {currListX+1,currListY+1}//down right
        };
        List<List<int>> attackOddMoves = new List<List<int>>()
        {
            //diagonals
            new List<int> {currListX-1,currListY-1},//up left
            new List<int> {currListX+1,currListY-1},//down left
            new List<int> {currListX-1,currListY},//up right
            new List<int> {currListX+1,currListY}//down right
        };

        List<GameObject> attackMove = new List<GameObject>();
        List<List<int>> attackMovesToCheck;

        if (currX % 2 == 0) attackMovesToCheck = attackEvenMoves; else attackMovesToCheck = attackOddMoves;
        int currListXHere = attackMovesToCheck[listIteration][0];
        int currListYHere = attackMovesToCheck[listIteration][1];

        if (currListXHere >= 0 && currListYHere >= 0 && currListXHere < posList.Count && currListYHere < posList[currListXHere].Count && posList[currListXHere][currListYHere][1] == null && blackPawn != null)
        {
            attackMove.Add(posList[currListXHere][currListYHere][0]);
            attackMove.Add(blackPawn);
            posList[currListXHere][currListYHere][0].GetComponent<MeshRenderer>().material.color = Color.red;

            return attackMove;
        }
        else return null;
    }
}
