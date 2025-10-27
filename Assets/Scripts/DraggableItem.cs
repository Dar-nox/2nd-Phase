using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemID;
    public Image image;
    public TextMeshProUGUI countText;
    [HideInInspector] public Transform parentAfterDrag;
    public int count = 1;

    private void Start()
    {
        UpdateCountDisplay();
    }

    private void OnValidate()
    {
        UpdateCountDisplay();
    }

    public void UpdateCountDisplay()
    {
        if (countText != null)
        {
            if (count <= 1)
            {
                countText.gameObject.SetActive(false);
            }
            else
            {
                countText.gameObject.SetActive(true);
                countText.text = count.ToString();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // If this item is a stacked item, decrement the stack and drag a cloned single item instead.
        if (count > 1)
        {
            // remember original parent so the clone can return to the same slot
            Transform originalParent = transform.parent;

            // remove one from the stack on the original
            count -= 1;
            UpdateCountDisplay();

            // create a visual clone to drag
            GameObject clone = Instantiate(gameObject, transform.root);
            clone.name = gameObject.name; // remove (Clone) for clarity

            // get the DraggableItem on the clone and set it up as a single item
            DraggableItem cloneItem = clone.GetComponent<DraggableItem>();
            if (cloneItem != null)
            {
                cloneItem.count = 1;
                cloneItem.parentAfterDrag = originalParent;
                cloneItem.UpdateCountDisplay();
                if (cloneItem.image != null)
                    cloneItem.image.raycastTarget = false;
            }

            // place the clone at the mouse and make it top-most
            clone.transform.SetParent(transform.root);
            clone.transform.SetAsLastSibling();
            clone.transform.position = Input.mousePosition;

            // transfer the pointer drag to the clone and send it a begin-drag event so it becomes the active drag target
            eventData.pointerDrag = clone;
            ExecuteEvents.Execute<IBeginDragHandler>(clone, eventData, ExecuteEvents.beginDragHandler);

            return;
        }

        Debug.Log("Started dragging item");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        if (image != null)
            image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // If this GameObject is not the active pointer drag target, ignore the drag events
        if (eventData.pointerDrag != gameObject)
            return;

        Debug.Log("Dragging item");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If this GameObject is not the active pointer drag target, ignore the end-drag events
        if (eventData.pointerDrag != gameObject)
            return;

        Debug.Log("Ended dragging item");
        transform.SetParent(parentAfterDrag);
        if (image != null)
            image.raycastTarget = true;
    }
}
