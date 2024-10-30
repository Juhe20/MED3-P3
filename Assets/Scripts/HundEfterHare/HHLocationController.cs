using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class HHLocationController : MonoBehaviour
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
        float pawnDis = 30f / 5f;
        float oddOffset = pawnDis;
        float widthFactor = 1f;

        for (int i = 0; i < 5; i++)
        {
            posList.Add(new List<List<GameObject>>());

            for (int j = 0; j < 5; j++)
            {
                posList[i].Add(new List<GameObject> { null, null });
            }

            for (int j = 0; j < 5; j++)
            {
                float _currX = i * pawnDis;
                float _currZ = j * pawnDis * widthFactor;

                if (!CheckOutsideBorder(i,j))
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

        posList[i][j][0] = currObj;
    }

    public GameObject createPawn(GameObject prefab, GameObject posObj, int i, int j, PosClass currPosObj)
    {
        if (posObj != null) //just a safe null check
        {
            GameObject currObj = Instantiate(prefab, new Vector3(posObj.transform.position.x, posObj.transform.position.y + 0.5f, posObj.transform.position.z), Quaternion.Euler(0, 0, 0));
            currObj.transform.SetParent(board.transform);
            PawnClass pawnClass = currObj.GetComponent<PawnClass>();
            pawnClass.setListX(i);
            pawnClass.setListY(j);

            posList[i][j][1] = currObj;
            return currObj;
        }
        else return null;
    }

    bool CheckOutsideBorder(int x, int y)
    {
        List<List<int>> outsideboard = new List<List<int>>()
        {
            new List<int> {0,0},
            new List<int> {0,1},
            new List<int> {0,3},
            new List<int> {0,4},
            new List<int> {1,0},
            new List<int> {1,4},
            new List<int> {3,0},
            new List<int> {3,4},
            new List<int> {4,0},
            new List<int> {4,1},
            new List<int> {4,3},
            new List<int> {4,4},
        };

        List<int> currentPos = new List<int>() { x, y };
        bool isOutside = false;

        for (int i = 0; i < outsideboard.Count; i++)
        {
            if (outsideboard[i][0] == x && outsideboard[i][1] == y)
            {
                isOutside = true; break;
            }
            else isOutside = false;
        }
        return isOutside;
    }
}
