using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
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
    List<List<GameObject>> posList;
    List<GameObject> possibleMoves;

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
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()].GetComponent<PosClass>();
                                //Debug.Log("X: "+pawnClass.getListX());
                                //Debug.Log("Y: " + pawnClass.getListY());
                                if (posClass != null)
                                {
                                    possibleMoves = calculatePossibleWhiteMoves(posClass);
                                    currState = state.WHITE_MOVE;
                                }
                            }
                            else Debug.Log("idk");
                        }
                        else
                        {
                            Debug.Log("not white pawn clicked");
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
                        if (clicked.Equals(possibleMoves[i]))
                        {
                            selectedPawn.transform.position = possibleMoves[i].transform.position;

                            //update the data for the posobjects and the pawn object
                            PawnClass selectedPawnClass = selectedPawn.GetComponent<PawnClass>(); // get the class that hold the info
                            PosClass currPosClass = possibleMoves[i].GetComponent<PosClass>(); // get the class that hold the info
                            Debug.Log(selectedPawnClass);
                            Debug.Log(currPosClass);
                            if (selectedPawnClass != null && currPosClass != null)
                            {
                                PosClass originPosClass = posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()].GetComponent<PosClass>(); // get the class for the pos object it was standing on
                                originPosClass.setOccupied(false); //update its occupied variable

                                selectedPawnClass.setListX(currPosClass.getListX()); //set the selected pawns x pos to the new pos
                                selectedPawnClass.setListY((currPosClass.getListY()));//set the selected pawns y pos to the new pos

                                currPosClass.setOccupied(true); //set the new pos objects occupied to true.

                            }
                            else Debug.Log("fuck");
                            for (int j = 0; j < possibleMoves.Count; j++)
                            {
                                possibleMoves[j].GetComponent<MeshRenderer>().material = ogPosMatirial;
                                
                            }
                            possibleMoves.Clear();
                            currState = state.BLACK_SELECT; //set the state to the next
                            break;
                        }
                        else
                        {
                            Debug.Log("invalid move/one of the others in the list");
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
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()].GetComponent<PosClass>();
                                //Debug.Log("X: "+pawnClass.getListX());
                                //Debug.Log("Y: " + pawnClass.getListY());
                                if (posClass != null)
                                {
                                    possibleMoves = calculatePossibleBlackMoves(posClass);
                                    currState = state.BLACK_MOVE;
                                }
                            }
                            else Debug.Log("idk");
                        }
                        else
                        {
                            Debug.Log("not white pawn clicked");
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
                        if (clicked.Equals(possibleMoves[i]))
                        {
                            selectedPawn.transform.position = possibleMoves[i].transform.position;

                            //update the data for the posobjects and the pawn object
                            PawnClass selectedPawnClass = selectedPawn.GetComponent<PawnClass>(); // get the class that hold the info
                            PosClass currPosClass = possibleMoves[i].GetComponent<PosClass>(); // get the class that hold the info
                            Debug.Log(selectedPawnClass);
                            Debug.Log(currPosClass);
                            if (selectedPawnClass != null && currPosClass != null)
                            {
                                PosClass originPosClass = posList[selectedPawnClass.getListX()][selectedPawnClass.getListY()].GetComponent<PosClass>(); // get the class for the pos object it was standing on
                                originPosClass.setOccupied(false); //update its occupied variable

                                selectedPawnClass.setListX(currPosClass.getListX()); //set the selected pawns x pos to the new pos
                                selectedPawnClass.setListY((currPosClass.getListY()));//set the selected pawns y pos to the new pos

                                currPosClass.setOccupied(true); //set the new pos objects occupied to true.

                            }
                            else Debug.Log("fuck");
                            for (int j = 0; j < possibleMoves.Count; j++)
                            {
                                possibleMoves[j].GetComponent<MeshRenderer>().material = ogPosMatirial;

                            }
                            possibleMoves.Clear();
                            currState = state.WHITE_SELECT; //set the state to the next
                            break;
                        }
                        else
                        {
                            Debug.Log("invalid move/one of the others in the list");
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

    List<GameObject> calculatePossibleWhiteMoves(PosClass currPos)
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
                GameObject currPosObj = posList[currListX][currListY];
                if (currPosObj != null)
                {
                    PosClass posClass = currPosObj.GetComponent<PosClass>();
                    if (posClass != null)
                    {
                        if (!posClass.getOccupied())
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
                GameObject currPosObj = posList[currListX][currListY];
                if (currPosObj != null)
                {
                    PosClass posClass = currPosObj.GetComponent<PosClass>();
                    if (posClass != null)
                    {
                        if (!posClass.getOccupied())
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
}
