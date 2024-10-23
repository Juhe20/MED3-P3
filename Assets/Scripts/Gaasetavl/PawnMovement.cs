using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Search;
using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    [SerializeField] Material ogPosMatirial;
    public LocationController locationController;
    public enum state
    {
        WHITE_SELECT,
        WHITE_MOVE,
        BLACK_SELECT,
        BLACK_MOVE,
        WHITE_WON,
        BLACK_WON
    }
    public state currState = state.WHITE_SELECT;
    List<List<List<GameObject>>> posList;
    List<List<GameObject>> possibleMoves;

    GameObject selectedPawn;

    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(WaitForLocationController());
    }
    private IEnumerator WaitForLocationController()
    {
        while (locationController == null || locationController.posList.Count == 0)
        {
            // Wait for the next frame before checking again
            yield return null;
        }

        posList = locationController.posList;
    }

    // Update is called once per frame
    void Update()
    {
        //check if a pawn is clicked

        switch (currState)
        {
            //white states
            case (state.WHITE_SELECT):

                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();
                    selectedPawn = clicked;
                    if (selectedPawn != null)
                    {
                        if (selectedPawn.CompareTag("whitePawn"))
                        {
                            PawnClass pawnClass = selectedPawn.GetComponent<PawnClass>();
                            if (pawnClass != null)
                            {
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()][0].GetComponent<PosClass>();
                                //Debug.Log("X: "+pawnClass.getListX());
                                //Debug.Log("Y: " + pawnClass.getListY());
                                if (posClass != null)
                                {
                                    possibleMoves = calculatePossibleMoves(posClass);
                                    currState = state.WHITE_MOVE;
                                }
                            }
                        }
                    }
                }

                break;

            case (state.WHITE_MOVE):
                
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();

                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        if (clicked.Equals(possibleMoves[i][0])) // it says this line is out of range
                        {
                            if (possibleMoves[i][1]  != null)
                            {
                                Destroy(possibleMoves[i][1]);
                            }

                            selectedPawn.transform.position = possibleMoves[i][0].transform.position;

                            //update the data for the posobjects and the pawn object
                            PawnClass selectedPawnClass = selectedPawn.GetComponent<PawnClass>(); // get the class that hold the info
                            PosClass currPosClass = possibleMoves[i][0].GetComponent<PosClass>(); // get the class that hold the info
                            if (selectedPawnClass != null && currPosClass != null)
                            {
                                PosClass originPosClass = posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][0].GetComponent<PosClass>(); // get the class for the pos object it was standing on

                                posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][1] = null;

                                selectedPawnClass.setListX(currPosClass.getListX()); //set the selected pawns x pos to the new pos
                                selectedPawnClass.setListY((currPosClass.getListY()));//set the selected pawns y pos to the new pos

                                posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][1] = selectedPawn;

                            }
                            for (int j = 0; j < possibleMoves.Count; j++)
                            {
                                possibleMoves[j][0].GetComponent<MeshRenderer>().material = ogPosMatirial;
                                
                            }
                            possibleMoves.Clear();
                            currState = state.BLACK_SELECT; //set the state to the next
                            break;
                        }
                    }
                }

                break;

            case (state.WHITE_WON):
                
                break;

            //black states
            case (state.BLACK_SELECT):
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();
                    selectedPawn = clicked;
                    if (selectedPawn != null)
                    {
                        if (selectedPawn.CompareTag("blackPawn"))
                        {
                            PawnClass pawnClass = selectedPawn.GetComponent<PawnClass>();
                            if (pawnClass != null)
                            {
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()][0].GetComponent<PosClass>();
                                //Debug.Log("X: "+pawnClass.getListX());
                                //Debug.Log("Y: " + pawnClass.getListY());
                                if (posClass != null)
                                {
                                    possibleMoves = calculatePossibleMoves(posClass);
                                    currState = state.BLACK_MOVE;
                                }
                            }
                        }
                    }
                }
                break;

            case (state.BLACK_MOVE):
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();

                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        if (clicked.Equals(possibleMoves[i][0]))
                        {
                            selectedPawn.transform.position = possibleMoves[i][0].transform.position;

                            //update the data for the posobjects and the pawn object
                            PawnClass selectedPawnClass = selectedPawn.GetComponent<PawnClass>(); // get the class that hold the info
                            PosClass currPosClass = possibleMoves[i][0].GetComponent<PosClass>(); // get the class that hold the info
                            if (selectedPawnClass != null && currPosClass != null)
                            {
                                PosClass originPosClass = posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][0].GetComponent<PosClass>(); // get the class for the pos object it was standing on

                                posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][1] = null;

                                selectedPawnClass.setListX(currPosClass.getListX()); //set the selected pawns x pos to the new pos
                                selectedPawnClass.setListY((currPosClass.getListY()));//set the selected pawns y pos to the new pos

                                posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()][1] = selectedPawn;

                            }
                            for (int j = 0; j < possibleMoves.Count; j++)
                            {
                                possibleMoves[j][0].GetComponent<MeshRenderer>().material = ogPosMatirial;

                            }
                            possibleMoves.Clear();
                            currState = state.WHITE_SELECT; //set the state to the next
                            break;
                        }
                    }
                }
                break;

            case (state.BLACK_WON):

                break;
        }
    }

    GameObject clickObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            //Debug.Log("hit object:" + hitObject);

            return hitObject;
        }
        else
        {
            //Debug.Log("no object hit");
            return null;
        }
    }

    List<List<GameObject>> calculatePossibleMoves(PosClass currPos)
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
                            currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                            List<GameObject> data = new List<GameObject>()
                            {
                                currPosObj,
                                null
                            };
                            calculatedMoves.Add(data);
                            Debug.Log(calculatedMoves[calculatedMoves.Count - 1][0]);
                        }
                        else if (posList[currListX][currListY][1].CompareTag("blackPawn") && currState == state.WHITE_SELECT)
                        {
                            List<GameObject> data = calculateWhiteAttackMoves(
                                posList[currListX][currListY][1],
                                currX,
                                currY,
                                currListX,
                                currListY,
                                i);
                            if (data != null) calculatedMoves[i] = data;
                        }
                    }
                }
            }
            else continue;
                
        }

        return calculatedMoves;
    }

    List<GameObject> calculateWhiteAttackMoves(GameObject blackPawn,int currX, int currY,int currListX, int currListY,int listIteration)
    {
        List<List<int>> attackEvenMoves = new List<List<int>>()
        {
            //straights
            new List<int> {currListX-2,currListY},//up
            new List<int> {currListX+2,currListY},//down
            new List<int> {currListX,currListY-2},//left
            new List<int> {currListX,currListY+2},//right
            //diagonals
            new List<int> {currListX-2,currListY-1},//up left
            new List<int> {currListX+2,currListY-1},//down left
            new List<int> {currListX-2,currListY+1},//up right
            new List<int> {currListX+2,currListY+1}//down right
        };
        List<List<int>> attackOddMoves = new List<List<int>>()
        {
            //diagonals
            new List<int> {currListX-2,currListY-1},//up left
            new List<int> {currListX+2,currListY-1},//down left
            new List<int> {currListX-2,currListY+1},//up right
            new List<int> {currListX-2,currListY-1}//down right
        };

        List<GameObject> attackMoves;
        List<List<int>> attackMovesToCheck;

        if (currX % 2 == 0) attackMovesToCheck = attackEvenMoves; else attackMovesToCheck = attackOddMoves;
        int currListXHere = attackMovesToCheck[listIteration][0];
        int currListYHere = attackMovesToCheck[listIteration][1];

        if (currListXHere >= 0 && currListXHere < posList.Count && currListYHere >= 0 && currListYHere < posList[currListXHere].Count)
        {
            attackMoves = new List<GameObject>()
            {
                posList[currX][currY][0],
                posList[attackMovesToCheck[listIteration][0]][attackMovesToCheck[listIteration][1]][1]
            };

            if (posList[attackMovesToCheck[listIteration][0]][attackMovesToCheck[listIteration][1]][1] == null)
            {
                return null;
            }
            else
            {
                return attackMoves;
            }
        }
        else return null;
    }
    
    


    /*
    List<GameObject> calculatePossibleBlackMoves(PosClass currPos)
    {
        int currX = currPos.getListX();
        int currY = currPos.getListY();

        List<GameObject> calculatedMoves = new List<GameObject>();
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
                            currPosObj.GetComponent<MeshRenderer>().material.color = Color.red;
                            calculatedMoves.Add(currPosObj);
                        }
                    }
                    else Debug.Log("no posclass");
                }
            }
            else continue;

        }

        return calculatedMoves;
    }
*/
}
