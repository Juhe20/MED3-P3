using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Search;
using UnityEngine;
using TMPro;

public class PawnMovement : MonoBehaviour
{
    [SerializeField] Material ogPosMaterial;
    public LocationController locationController;
    [SerializeField] CalculateMoves cm;
    [SerializeField] TimerController timerController;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timerTime;
    [SerializeField] PawnMoveController pawnMoveController;
    [SerializeField] float whiteYOffset = 0.5f;
    [SerializeField] float blackYOffset = 0.5f;

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
        timerController.timeLeft = timerTime;
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
        //check if the time is up
        if (timerController.timeLeft == 0) currState = state.BLACK_WON;
        
        //check how many black are left
        GameObject[] blackPawnsLeft = GameObject.FindGameObjectsWithTag("blackPawn");
        if (blackPawnsLeft.Length == 0) currState = state.WHITE_WON;


        switch (currState)
        {
            //white states
            case (state.WHITE_SELECT):
                if (timerController.timerOn == true) timerController.timerOn = false;
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
                                if (posClass != null)
                                {
                                    possibleMoves = cm.calculatePossibleMoves(posClass,selectedPawn,posList,currState);
                                    if (possibleMoves.Count == 0) currState = state.WHITE_SELECT; else currState = state.WHITE_MOVE;
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

                            //selectedPawn.transform.position = new Vector3 (possibleMoves[i][0].transform.position.x, possibleMoves[i][0].transform.position.y+0.5f, possibleMoves[i][0].transform.position.z);
                            Vector3 targetPos = new Vector3(possibleMoves[i][0].transform.position.x, whiteYOffset, possibleMoves[i][0].transform.position.z);
                            pawnMoveController.moveAndAnimatePawn(targetPos, selectedPawn, whiteYOffset);

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
                                possibleMoves[j][0].GetComponent<MeshRenderer>().material = ogPosMaterial;
                                
                            }
                            possibleMoves.Clear();
                            currState = state.BLACK_SELECT; //set the state to the next
                            break;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMaterial;

                    }
                    possibleMoves.Clear();
                    currState = state.WHITE_SELECT; //set the state to the next
                }

                break;

            case (state.WHITE_WON):
                timerText.text = "The Foxes Slays!";
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMaterial;

                }
                possibleMoves.Clear();
                break;

            //black states
            case (state.BLACK_SELECT):
                //Start the timer if it hasn't been started
                if (timerController.timerOn == false) timerController.timerOn = true;
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
                                if (posClass != null)
                                {
                                    possibleMoves = cm.calculatePossibleMoves(posClass,selectedPawn,posList,currState);
                                    if (possibleMoves.Count == 0) currState = state.BLACK_SELECT; else currState = state.BLACK_MOVE;
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
                        if (clicked != null)
                        {
                            if (clicked.Equals(possibleMoves[i][0]))
                            {
                                //selectedPawn.transform.position = new Vector3(possibleMoves[i][0].transform.position.x, possibleMoves[i][0].transform.position.y + 0.5f, possibleMoves[i][0].transform.position.z);
                                Vector3 targetPos = new Vector3(possibleMoves[i][0].transform.position.x, blackYOffset, possibleMoves[i][0].transform.position.z);
                                pawnMoveController.moveAndAnimatePawn(targetPos, selectedPawn, blackYOffset);

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
                                    possibleMoves[j][0].GetComponent<MeshRenderer>().material = ogPosMaterial;

                                }
                                possibleMoves.Clear();
                                currState = state.WHITE_SELECT; //set the state to the next
                                break;
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    for (int i = 0; i < possibleMoves.Count; i++)
                    {
                        possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMaterial;

                    }
                    possibleMoves.Clear();
                    currState = state.BLACK_SELECT; //set the state to the next
                }
                break;

            case (state.BLACK_WON):
                timerText.text = "The Geese Slays!";
                for (int i = 0; i < possibleMoves.Count; i++)
                {
                    possibleMoves[i][0].GetComponent<MeshRenderer>().material = ogPosMaterial;

                }
                possibleMoves.Clear();
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
