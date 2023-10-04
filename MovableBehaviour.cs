using System.Transactions;
using System;
using SML;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Utils;
public class DragnDrop : MonoBehaviour
{
    public Func<bool> isVisible = null;
    public RectTransform canvas;
    private static bool alrDragging = false;
    private bool isDragging = false;
    private Vector2 offset;
    private RectTransform uiRectTransform;
    public float xLength;
    public float yLength;
    public float marginY;
    public float yOffset;
    public int l;
    public bool oldControls;
    public bool estimate;
    public void Start()
    {
        estimate = ModSettings.GetBool("Use rough estimates", "JAN.movablewills");
        oldControls = ModSettings.GetBool("Use old controls", "JAN.movablewills");
        string p = ModSettings.GetString("Middle-click Functionality", "JAN.movablewills");
        l = p == "Centralize" ? 1 : ( p  == "Go here" ? 2 : 0);
        uiRectTransform = GetComponent<RectTransform>();
        uiRectTransform.pivot = new Vector2(1, 1);
        canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void Update()
    {
        if (l == 1 && Input.GetKey(KeyCode.Mouse2))
        {
            isDragging = false;
            alrDragging = false;
            Vector2 canvasPosition = new Vector2(
                canvas.rect.width / 2,
                canvas.rect.height / 2
            );
            uiRectTransform.localPosition = canvasPosition;
            return;
        }
        if (!isVisible()) return;
        Vector2 screenMousePosition = Input.mousePosition;
        Vector2 screenPercentage = new Vector2(screenMousePosition.x / Screen.width, screenMousePosition.y / Screen.height);
        if (oldControls && Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector2 objPos = new Vector2(uiRectTransform.localPosition.x / canvas.rect.width, uiRectTransform.localPosition.y / canvas.rect.height);
            if(estimate){
            RectTransform t = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            Vector2 margin = new Vector2(t.rect.width/2/canvas.rect.width, t.rect.height /2/canvas.rect.height);
            if (!alrDragging &&(screenPercentage.x > objPos.x - margin.x && screenPercentage.x < objPos.x + margin.x) && (screenPercentage.y > objPos.y - margin.y && screenPercentage.y < objPos.y + margin.y))
            {
                alrDragging = true;
                offset = objPos - screenPercentage;
                isDragging = true;
            }
            }else{
            if (!alrDragging &&(screenPercentage.x > objPos.x - xLength && screenPercentage.x < objPos.x + xLength) && (screenPercentage.y > objPos.y - marginY && screenPercentage.y < objPos.y + marginY))
            {
                alrDragging = true;
                offset = objPos - screenPercentage;
                isDragging = true;
            }
            }
        }
        else if (!oldControls && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 objPos = new Vector2(uiRectTransform.localPosition.x / canvas.rect.width, uiRectTransform.localPosition.y / canvas.rect.height);
            if (!alrDragging && (screenPercentage.x > objPos.x - xLength && screenPercentage.x < objPos.x + xLength) && (screenPercentage.y > objPos.y - yLength + yOffset && screenPercentage.y < objPos.y +yLength+ yOffset))
            {
                alrDragging = true;
                offset = objPos - screenPercentage;
                isDragging = true;
            }
        }
        else if (l == 2 && Input.GetKey(KeyCode.Mouse2))
        {
            isDragging = false;
            alrDragging = false;
            Vector2 canvasPosition = new Vector2(
                canvas.rect.width * screenPercentage.x,
                canvas.rect.height * screenPercentage.y
            );
            uiRectTransform.localPosition = canvasPosition;
        }
        else if (oldControls && Input.GetKeyUp(KeyCode.Mouse1))
        {
            isDragging = false;
            alrDragging = false;
        }
        else if (!oldControls && Input.GetKeyUp(KeyCode.Mouse0))
        {
            isDragging = false;
            alrDragging = false;
        }
        if (isDragging)
        {
            Vector2 canvasPosition = new Vector2(
                canvas.rect.width * (screenPercentage.x + offset.x),
                canvas.rect.height * (screenPercentage.y + offset.y)
            );
            uiRectTransform.localPosition = canvasPosition;
            if (MiscUtils.IsOffBounds(new Vector2(uiRectTransform.localPosition.x * 100 / canvas.rect.width, uiRectTransform.localPosition.y * 100 / canvas.rect.height), 10, 20))
            {
                Vector2 screenCenter = new Vector2(canvas.rect.width / 2f, canvas.rect.height / 2f);
                gameObject.transform.localPosition = screenCenter;
                isDragging = false;
                ChatUtils.AddFeedbackMsg("<color=#FF0000>Don't take your stuff outside the screen, bumfuzzle</color=#FF0000>", feedbackMessageType: "warning");
            }
        }


    }
}