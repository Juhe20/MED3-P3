using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour
{
    private bool isSelected = false;        // Track if the object is currently selected
    private Coroutine selectionDelayCoroutine; // Store the coroutine reference

    void OnMouseDown()
    {
        // Only allow selection if not already selected
        if (!isSelected)
        {
            isSelected = true; // Select the object when clicked
            selectionDelayCoroutine = StartCoroutine(SelectionDelay()); // Start delay coroutine
        }
    }

    void Update()
    {
        // Check for left mouse button click only if the object is selected
        if (isSelected && Input.GetMouseButtonDown(0) && selectionDelayCoroutine == null)
        {
            MoveObjectToMousePosition(); // Move the object to the clicked position
            isSelected = false; // Reset selection
        }
    }

    private void MoveObjectToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast to find the point in the world where the mouse is pointing
        if (Physics.Raycast(ray, out hit))
        {
            // Move the object to the hit point, keeping its Y position
            Vector3 newPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.position = newPosition; // Set the new position
            Debug.Log("Object moved to: " + newPosition);
        }
    }

    private IEnumerator SelectionDelay()
    {
        // Wait for 0.3 seconds
        yield return new WaitForSeconds(0.1f);

        // After the delay, allow the object to be selected again
        selectionDelayCoroutine = null; // Reset the coroutine reference
        Debug.Log("Ready for the second click.");
    }
}
