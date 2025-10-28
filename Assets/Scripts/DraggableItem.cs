using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    public string itemID;
    public Image image;
    public TextMeshProUGUI countText;
    [HideInInspector] public Transform parentAfterDrag;
    public int count = 1;
    [HideInInspector] public DraggableItem originalItem;
    private bool isClone = false;
    private bool isDraggingClone = false;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Nothing needed here anymore
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if this item has more than 1 count (works for both clones and originals)
        if (count > 1)
        {
            // Clone behavior for count > 1
            Debug.Log("Creating clone for stacked item");
            CreateCloneAndStartDrag(eventData);
            isDraggingClone = true;
        }
        else
        {
            // Normal drag behavior for count <= 1
            Debug.Log("Started dragging item");
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            isDraggingClone = false;
        }
    }

    private void CreateCloneAndStartDrag(PointerEventData eventData)
    {
        // Store the parent before creating clone
        parentAfterDrag = transform.parent;
        
        // Create a clone of this game object
        GameObject cloneObj = Instantiate(gameObject, transform.parent);
        
        // Set the clone's RectTransform size to 150x150
        RectTransform cloneRect = cloneObj.GetComponent<RectTransform>();
        if (cloneRect != null)
        {
            cloneRect.sizeDelta = new Vector2(150, 150);
        }
        
        // Get the clone's DraggableItem component
        DraggableItem cloneItem = cloneObj.GetComponent<DraggableItem>();
        if (cloneItem != null)
        {
            // Set clone properties
            cloneItem.count = 1;
            cloneItem.isClone = true;
            cloneItem.originalItem = this;
            cloneItem.UpdateCountDisplay();
            cloneItem.parentAfterDrag = transform.parent;
            
            // Set up the clone for dragging
            cloneItem.transform.SetParent(transform.root);
            cloneItem.transform.SetAsLastSibling();
            cloneItem.image.raycastTarget = false;
            
            // Manually set eventData.pointerDrag to the clone so the drag system works
            eventData.pointerDrag = cloneObj;
        }
        
        // Decrement the original item's count
        count--;
        UpdateCountDisplay();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Only drag if we're not creating a clone
        if (!isDraggingClone)
        {
            Debug.Log("Dragging item");
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Ended dragging item");
        
        // Only handle end drag if we're not creating a clone
        if (!isDraggingClone)
        {
            // Check if clone was dropped successfully
            if (isClone && transform.parent == parentAfterDrag)
            {
                // Clone was not placed in a new slot, destroy it and restore original count
                if (originalItem != null)
                {
                    originalItem.count++;
                    originalItem.UpdateCountDisplay();
                }
                Destroy(gameObject);
            }
            else
            {
                // Normal end drag behavior
                transform.SetParent(parentAfterDrag);
                image.raycastTarget = true;
            }
        }
        
        // Reset the flag
        isDraggingClone = false;
    }
}
