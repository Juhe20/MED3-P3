using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : MonoBehaviour
{
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
        Debug.Log(posList.Count);
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
                    GameObject clicked = clickPawn();
                    if (clicked != null)
                    {
                        if (clicked.CompareTag("whitePawn"))
                        {
                            PawnClass pawnClass = clicked.GetComponent<PawnClass>();
                            if (pawnClass != null)
                            {
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()].GetComponent<PosClass>();
                                if (posClass != null)
                                {
                                    possibleMoves = calculatePossibleWhiteMoves(posClass);
                                }
                            }
                            else Debug.Log("idk");
                        }
                        else
                        {
                            Debug.Log("something else clicked");
                        }
                    }

                    currState = state.WHITE_MOVE;
                }

                break;

            case (state.WHITE_MOVE):
                
                break;

            case (state.WHITE_WON):
                
                break;

            //black states
            case (state.BLACK_SELECT):

                break;

            case (state.BLACK_MOVE):

                break;

            case (state.BLACK_WON):

                break;
        }
    }

    GameObject clickPawn()
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
        List<List<int>> evenListToCheck = new List<List<int>>()
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
            new List<int> {currX+1,currY},//down right
        };

        if (currX % 2 == 0) //to check if it is the short or long rows
        {
            for (int i = 0; i < evenListToCheck.Count; i++)
            {
                int currListX = evenListToCheck[i][0];
                int currListY = evenListToCheck[i][1];

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
        }
        return calculatedMoves;
    }

    void calculatePossibleBlackMoves()
    {

    }
}
