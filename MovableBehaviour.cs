using System;
using SML;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Utils;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Bindings;
using BMG.UI;
using Game.Interface;
using System.IO.Compression;
public class DragnDrop : MonoBehaviour
{
    //Movable wills stuff
    public bool allowSaving = false;
    public float timeForSaving = 2;
    public LastWillPanel a = null;
    public bool movablewills = false;
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
    public string snapshot = null;
    public bool b = false;

    // Undo/redo stuff
    public DropOutStack<string> undoStack = new(50);
    public DropOutStack<string> redoStack = new(50);
    public static GameObject lastTouched = null;
    public BMG_InputField field;
    private bool modifiedText = false;

    public void Start()
    {
        undoStack.Push("");
        field.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(OnChanged));
        field.onSelect.AddListener(new UnityEngine.Events.UnityAction<string>(OnSelect));
        estimate = ModSettings.GetBool("Use rough estimates", "JAN.movablewills");
        oldControls = ModSettings.GetBool("Use old controls", "JAN.movablewills");
        b = ModSettings.GetBool("Auto-save will", "JAN.movablewills");
        movablewills = ModSettings.GetBool("Movable Wills", "JAN.movablewills");
        string p = ModSettings.GetString("Middle-click Functionality", "JAN.movablewills");
        l = p == "Centralize" ? 1 : ( p  == "Go here" ? 2 : ( p  == "Save" ? 3 : 0));
        uiRectTransform = GetComponent<RectTransform>();
        uiRectTransform.pivot = new Vector2(1, 1);
        canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void Update()
    {
        modifiedText = false;
        if(movablewills) MovingBehaviour();
        Undo();
        Redo();
        if(a != null && allowSaving && b) Save();
    }
    public void Save(){
        if(l == 3 && Input.GetKeyDown(KeyCode.Mouse2)) {
            a.SaveWill();
            allowSaving = false;
            return;
        }
        if(timeForSaving<=0) {a.SaveWill(); allowSaving = false; return;}
        timeForSaving -= Time.deltaTime;
        
    }
    public void MovingBehaviour(){
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

    public void Redo(){
        if (!isVisible()) return;
        if(redoStack.Count() == 0) return;
        if(!(lastTouched == gameObject)) return;
        if(!(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))) return;
        string redo = redoStack.Pop();
        modifiedText = true;
        undoStack.Push(field.text);
        field.text = redo;
        SetSelectionPosition(redo.Length);
        
    }
    public void Undo(){
        if (!isVisible()) return;
        if(!(lastTouched == gameObject)) return;
        if(undoStack.Count() == 0) return;
        if(!(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)) || Input.GetKey(KeyCode.LeftShift)) return;
        string undo = undoStack.Pop();
        modifiedText = true;
        redoStack.Push(field.text);
        field.text = undo;
        SetSelectionPosition(undo.Length);
        
    }
    private void SetSelectionPosition(int pos){
        field.selectionAnchorPosition = pos;
        field.stringPosition = pos;
        field.stringPositionInternal = pos;
        field.stringSelectPositionInternal = pos;
    }
    public void OnChanged(string text){
        allowSaving = true;
        timeForSaving = 2;
        if(snapshot != null){
            undoStack.Push(snapshot);
            snapshot = null;
        }
        if(modifiedText) return;
        redoStack.Clear();
        if(MiscUtils.CalcLevenshteinDistance(undoStack.Peek(), text) < 4) return;
        snapshot = text;
    }
    public void OnSelect(string text){
        lastTouched = gameObject;
    }
}