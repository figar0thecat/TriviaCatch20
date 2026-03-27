using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransFrom;

    [SerializeField, Range(10, 150)]
    private float leverRange;

    private Vector2 inputDirection;
    private bool isInput;

    public Animator anim;

    private void Awake()
    {
        rectTransFrom = GetComponent<RectTransform>();
    }

    void Start () {
		
	}
	
	
	void Update () {
        if (isInput)
        {
            InputControlVector();
        }
        else
        {
            anim.SetFloat("PoleVertical", 0);
            anim.SetFloat("PoleHorizontal", 0);
        }
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero;
        isInput = false;
    }

    private void ControlJoystickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransFrom.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector;
        inputDirection = inputVector / leverRange;
    }

    private void InputControlVector()
    {
        //Debug.Log(inputDirection.x + " / " + inputDirection.y);
        anim.SetFloat("PoleVertical", inputDirection.y);
        anim.SetFloat("PoleHorizontal", -inputDirection.x);
    }
}
