using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeCard : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Settings")]
    private RectTransform rectMovement;
    public RectTransform dropRect;
    public GameObject dropPos;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public PuzzleManager puzzleManagerScript;
    Vector2 startLocation;
    public bool drop;
    private bool dragging;
    public float timer;
    public float maxOffsets;
    public float minOffsets;
    private float distance;
    private Vector2 startPosition;

    [Header("Audio Settings")]
    [SerializeField] AudioSource cardSwipeSound;
    [SerializeField] AudioSource cardAcceptedSound;
    [SerializeField] AudioSource cardRejectedSound;

    private bool hasPlayedSwipeSound = false;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectMovement = GetComponent<RectTransform>();
        startLocation = rectMovement.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        startPosition = new Vector2(dropPos.transform.position.x, dropPos.transform.position.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //begin drag 
        canvasGroup.blocksRaycasts = false;
        hasPlayedSwipeSound = false;
    }

    private void Update()
    {
        if (dragging)
        {
            timer = timer + Time.deltaTime;
            //  Debug.Log(timer);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas)
        {
            dragging = true;
            float newx = rectMovement.anchoredPosition.x + eventData.delta.x / canvas.scaleFactor;
            if (newx > rectMovement.anchoredPosition.x)
            {
                // Clamp X movement
                float clampedX = Mathf.Clamp(newx, minOffsets, maxOffsets);
                rectMovement.anchoredPosition = new Vector2(clampedX, rectMovement.anchoredPosition.y);

                // Play swipe sound once when card reaches a certain threshold
                if (!hasPlayedSwipeSound && clampedX > (maxOffsets * 0.5f))
                {
                    if (cardSwipeSound != null)
                    {
                        cardSwipeSound.Play();
                    }
                    hasPlayedSwipeSound = true;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (IsOverlapping(rectMovement, dropRect))
        {
            Debug.Log("collided");
            if (Check())
            {
                // Play accepted sound (card was successfully swiped)
                if (cardAcceptedSound != null)
                {
                    cardAcceptedSound.Play();
                }
            }
            else
            {
                NotCollide();
            }
        }
        else
        {
            NotCollide();
        }
        timer = 0;
        canvasGroup.blocksRaycasts = true;
        dragging = false;
    }
    void NotCollide()
    {
        Debug.Log("Did not collide");
        // Play rejected sound (card returns to start)
        if (cardRejectedSound != null)
        {
            cardRejectedSound.Play();
        }

        rectMovement.anchoredPosition = startLocation;
    }
    void Restart()
    {
        Debug.Log("restart");
        rectMovement.anchoredPosition = startLocation;
        canvasGroup.blocksRaycasts = true;
        dragging = false;
        timer = 0;
    }

     bool Check()
     {
        distance = maxOffsets - minOffsets;
        float swipeSpeed = distance / timer;
        Debug.Log(swipeSpeed);
        //see how long it took to drag at the end 
        if (swipeSpeed is >= 200 and <= 1000f)
        {
            
             drop = true;

            Restart();
                //need to tell  puzzle master 
             puzzleManagerScript.FinishActivePuzzle();
            return true;
        }
        else
        {
            Debug.Log("To slow or to fast");
            return false;
        }
     }

    bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect rectA = GetScreenRect(a);
        Rect rectB = GetScreenRect(b);
        return rectA.Overlaps(rectB);
    }
    public Rect GetScreenRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}