using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MKPawnMovement : MonoBehaviour
{
    [SerializeField] Material ogPosMatirial;
    [SerializeField] MKLocationController locationController;
    [SerializeField] MKCalculateMoves calculateMoves;
    [SerializeField] TextMeshProUGUI text;

    public enum state
    {
        SELECT,
        MOVE,
        WHITE_WON,
        BLACK_WON
    }
    public state currState = state.SELECT;
    List<List<List<GameObject>>> posList;
    List<List<GameObject>> possibleMoves;
    GameObject selectedPawn;

    private string whiteTag;
    private string blackTag;
    private string whichTurn;
    private string opponent;

    void Start()
    {
        StartCoroutine(WaitForLocationController());

        whiteTag = "whitePawn";// to avoid typos
        blackTag = "blackPawn";// to avoid typos
        whichTurn = whiteTag; //the tag of which turn it is("whitePawn" or "blackPawn")
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
        //check how many white are left
        GameObject[] whitePawnsLeft = GameObject.FindGameObjectsWithTag(whiteTag);
        if (whitePawnsLeft.Length == 0) currState = state.BLACK_WON;

        //check how many black are left
        GameObject[] blackPawnsLeft = GameObject.FindGameObjectsWithTag(blackTag);
        if (blackPawnsLeft.Length == 0) currState = state.WHITE_WON;

        switch (currState)
        {
            case state.SELECT:
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
                                    if (whichTurn == whiteTag) opponent = blackTag; else if (whichTurn == blackTag) opponent = whiteTag;

                                    possibleMoves = calculateMoves.calculatePossibleMoves(posClass, selectedPawn, posList, whichTurn,opponent);
                                    if (possibleMoves.Count == 0) currState = state.SELECT; else currState = state.MOVE;
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

                            selectedPawn.transform.position = new Vector3(possibleMoves[i][0].transform.position.x, possibleMoves[i][0].transform.position.y + 0.5f, possibleMoves[i][0].transform.position.z);

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