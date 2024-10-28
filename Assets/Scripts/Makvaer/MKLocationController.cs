using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKLocationController : MonoBehaviour
{
    public List<List<List<GameObject>>> posList = new List<List<List<GameObject>>>();
    [SerializeField] GameObject posPrefab;
    //[SerializeField] GameObject posVisualPrefab;
    [SerializeField] GameObject board;
    [SerializeField] GameObject bPawnPrefab;
    [SerializeField] GameObject wPawnPrefab;

    // Start is called before the first frame update
    void Start()
    {
        float pawnDis = 30f / 8f;
        float oddOffset = pawnDis;
        float widthFactor = 1f;

        for (int i = 0; i < 8; i++)
        {
            posList.Add(new List<List<GameObject>>());

            for (int j = 0; j < 8; j++)
            {
                posList[i].Add(new List<GameObject> { null, null });
            }

            for (int j = 0; j < 8; j++)
            {
                float _currX = i * pawnDis;
                float _currZ = j * pawnDis * widthFactor;

                if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                {
                    createBoardPos(_currX, _currZ, i, j);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createBoardPos(float x, float z, int i, int j)
    {
        //instansiate object
        GameObject currObj = Instantiate(posPrefab, new Vector3(x, 0f, z), Quaternion.Euler(0, 0, 0));
        currObj.transform.SetParent(board.transform);

        PosClass posClass = currObj.GetComponent<PosClass>();
        if (posClass != null)
        {
            posClass.setListX(i);
            posClass.setListY(j);
        }

        if (i < 3) // dette burde relativt nemt kunne laves om til at tage imod en liste med exempelvis string "white" og "black" og så tjekke hvis den liste pos == white lav en vid og vice versa med sort
        {
            createPawn(bPawnPrefab, currObj, i, j, posClass);
        }
        else if (i > 4)
        {
            createPawn(wPawnPrefab, currObj, i, j, posClass);
        }

        posList[i][j][0] = currObj;
    }

    void createPawn(GameObject prefab, GameObject posObj, int i, int j, PosClass currPosObj)
    {
        if (posObj != null) //just a safe null check
        {
            GameObject currObj = Instantiate(prefab, new Vector3(posObj.transform.position.x, posObj.transform.position.y + 0.5f, posObj.transform.position.z), Quaternion.Euler(0, 0, 0));
            currObj.transform.SetParent(board.transform);
            PawnClass pawnClass = currObj.GetComponent<PawnClass>();
            pawnClass.setListX(i);
            pawnClass.setListY(j);

            posList[i][j][1] = currObj;
        }
    }
}
