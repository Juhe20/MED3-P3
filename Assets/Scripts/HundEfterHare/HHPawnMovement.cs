using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HHPawnMovement : MonoBehaviour
{
    [SerializeField] Material ogPosMatirial;
    [SerializeField] HHLocationController locationController;
    [SerializeField] HHCalculateMoves calculateMoves;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject blackPawnPrefab;
    [SerializeField] GameObject whitePawnPrefab;
    [SerializeField] TimerController timerController;
    [SerializeField] float timerTime;
    [SerializeField] PawnMoveController pawnMoveController;

    public enum state
    {
        WHITE_PLACE,
        BLACK_PLACE,
        SELECT,
        MOVE,
        WHITE_WON,
        BLACK_WON
    }
    public state currState = state.WHITE_PLACE;
    List<List<List<GameObject>>> posList;
    List<List<GameObject>> possibleMoves = new List<List<GameObject>>();
    GameObject selectedPawn;

    private string whiteTag;
    private string blackTag;
    private string whichTurn;
    private string opponent;
    int whiteLeftToPlace;
    int blackLeftToPlace;
    GameObject whitePawn;

    [SerializeField] float whiteYOfsset;
    [SerializeField] float blackYOfsset;
    float currYOfsset;

    void Start()
    {
        StartCoroutine(WaitForLocationController());

        whiteTag = "whitePawn";// to avoid typos
        blackTag = "blackPawn";// to avoid typos
        whichTurn = whiteTag; //the tag of which turn it is("whitePawn" or "blackPawn")

        timerController.timeLeft = timerTime;
        whiteLeftToPlace = 1; blackLeftToPlace = 3;
    }

    private IEnumerator WaitForLocationController() //for if the locationcontroller is loaded after this script.
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
        if (currState != state.WHITE_PLACE || currState != state.BLACK_PLACE)
        {
            if (timerController.timeLeft <= 0)
            {
                currState = state.WHITE_WON;
            }
        }

        switch (currState)
        {
            case state.WHITE_PLACE:
                text.text = "White place pawn";

                if (whiteLeftToPlace != 0)
                { 
                    if (possibleMoves.Count == 0)
                    {
                        if (posList != null)
                        {
                            for (int i = 0; i < posList.Count; i++)
                            {
                                for (int j = 0; j < posList[i].Count; j++)
                                {
                                    if (posList[i][j][0] != null)
                                    {
                                        possibleMoves.Add(new List<GameObject>() { posList[i][j][0], null });
                                        posList[i][j][0].GetComponent<MeshRenderer>().material.color = Color.red;
                                    }
                                }
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject clicked = clickObject();

                        for (int i = 0; i < possibleMoves.Count; i++)
                        {
                            if (clicked.Equals(possibleMoves[i][0]))
                            {
                                PosClass currPosClass = clicked.GetComponent<PosClass>();
                                whitePawn = locationController.createPawn(whitePawnPrefab, clicked, currPosClass.getListX(), currPosClass.getListY(),currPosClass, whiteYOfsset);
                                clicked.GetComponent<MeshRenderer>().material = ogPosMatirial;
                                possibleMoves.RemoveAt(i);
                                whiteLeftToPlace -= 1;
                            }
                        }
                    }
                }
                else
                {
                    currState = state.BLACK_PLACE;
                }
                break;

            case state.BLACK_PLACE:
                text.text = "Black place pawns";

                if (blackLeftToPlace != 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject clicked = clickObject();

                        for (int i = 0; i < possibleMoves.Count; i++)
                        {
                            if (clicked.Equals(possibleMoves[i][0]))
                            {
                                PosClass currPosClass = clicked.GetComponent<PosClass>();
                                locationController.createPawn(blackPawnPrefab, clicked, currPosClass.getListX(), currPosClass.getListY(), currPosClass,blackYOfsset);
                                blackLeftToPlace -= 1;
                                clicked.GetComponent<MeshRenderer>().material = ogPosMatirial;
                                possibleMoves.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < possibleMoves.Count; j++)
                    {
                        possibleMoves[j][0].GetComponent<MeshRenderer>().material = ogPosMatirial;

                    }
                    possibleMoves.Clear();
                    currState = state.SELECT;
                }
                text.text = "";
                break;

            case state.SELECT:

                if (whichTurn == blackTag) timerController.timerOn = true; else if (whichTurn == whiteTag) timerController.timerOn = false;
                if (whichTurn == whiteTag) 
                {
                    opponent = blackTag;
                    currYOfsset = whiteYOfsset;
                }
                else if (whichTurn == blackTag)
                {
                    opponent = whiteTag;
                    currYOfsset = blackYOfsset;
                }

                if (whichTurn == whiteTag)
                {
                    PawnClass pawnClass = whitePawn.GetComponent<PawnClass>();
                    if (pawnClass != null)
                    {
                        PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()][0].GetComponent<PosClass>();
                        if (posClass != null)
                        {
                            if (calculateMoves.calculatePossibleMoves(posClass, selectedPawn, posList, whichTurn, opponent, false).Count == 0) currState = state.BLACK_WON;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();
                    selectedPawn = clicked;
                    if (selectedPawn != null)
                    {
                        if (selectedPawn.CompareTag(whichTurn))
                        {
                            PawnClass pawnClass = selectedPawn.GetComponent<PawnClass>();
                            if (pawnClass != null)
                            {
                                PosClass posClass = posList[pawnClass.getListX()][pawnClass.getListY()][0].GetComponent<PosClass>();
                                if (posClass != null)
                                {
                                    possibleMoves = calculateMoves.calculatePossibleMoves(posClass, selectedPawn, posList, whichTurn, opponent, true);
                                    if (possibleMoves.Count == 0) currState = state.SELECT; else currState = state.MOVE;
                                    if (possibleMoves.Count == 0 && whichTurn == whiteTag) currState = state.BLACK_WON; 
                                }
                            }
                        }
                    }
                }
                break;

            case state.MOVE:
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject clicked = clickObject();

                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        if (clicked.Equals(possibleMoves[i][0])) // it says this line is out of range
                        {
                            if (possibleMoves[i][1] != null)
                            {
                                Destroy(possibleMoves[i][1]);
                            }

                            //selectedPawn.transform.position = new Vector3(possibleMoves[i][0].transform.position.x, possibleMoves[i][0].transform.position.y + 0.5f, possibleMoves[i][0].transform.position.z);
                            //Call moveScript here
                            Vector3 targetPos = new Vector3(possibleMoves[i][0].transform.position.x, currYOfsset, possibleMoves[i][0].transform.position.z);
                            pawnMoveController.moveAndAnimatePawn(targetPos, selectedPawn, currYOfsset);

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

                            if (whichTurn == whiteTag) whichTurn = blackTag; else if (whichTurn == blackTag) whichTurn = whiteTag;
                            currState = state.SELECT;

                            break;
                        }
                    }
                }
                break;

            case state.WHITE_WON:
                text.text = "White Slays!";
                if (possibleMoves != null)
                {
                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMatirial;

                    }
                    possibleMoves.Clear();
                }
                break;

            case state.BLACK_WON:
                text.text = "Black Slays!";
                if (possibleMoves != null)
                {
                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMatirial;

                    }
                    possibleMoves.Clear();
                }
                break;
        }

        GameObject clickObject()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;

                return hitObject;
            }
            else
            {
                return null;
            }
        }
    }
}
