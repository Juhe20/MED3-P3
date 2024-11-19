using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PawnMoveController : MonoBehaviour
{
    public void moveAndAnimatePawn(Vector3 targetPos, GameObject objectToMove, float yOffset)
    {
        Animator animator = objectToMove.GetComponent<Animator>();
        animator.SetFloat("aSpeed", 1f);

        Vector3 lookDir = (targetPos - objectToMove.transform.position).normalized;
        lookDir.y = 0f;
        objectToMove.transform.rotation = Quaternion.LookRotation(lookDir);


        StartCoroutine(MoveTarget(objectToMove, targetPos, animator, yOffset));
    }

    private IEnumerator MoveTarget(GameObject objectToMove, Vector3 targetPos,Animator animator, float yOffset)
    {
        float speed = 5f;
        float threshold = 0.01f;

        while (Vector3.Distance(objectToMove.transform.position, targetPos) > threshold)
        {
            objectToMove.transform.position = Vector3.MoveTowards(
                    objectToMove.transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );

            yield return null;
        }

        objectToMove.transform.position = targetPos;
        animator.SetFloat("aSpeed", 0f);
        animator.Rebind();
        animator.Update(0);
    }
}
