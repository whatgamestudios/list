// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    // Sits on a transparent overlay covering a list item row, on top of its
    // TMP_InputField, so gestures can be captured before the input field's own
    // drag-to-select-text handling claims them (it would, for any gesture
    // starting within its bounds, since it's the exact raycast hit and Unity's
    // event bubbling only kicks in when the hit object itself has no handler).
    //
    // Gestures:
    //  - Double tap: enters edit mode. A single tap does nothing.
    //  - Quick horizontal drag: swipe state machine, see below.
    //  - Quick vertical drag: forwarded to the enclosing ScrollRect so the list
    //    still scrolls normally.
    //  - Touch and hold (without moving) for HoldThresholdSeconds, then drag up
    //    or down: reorders the item by one position per gesture, in the
    //    direction dragged.
    //
    // Swipe states: Default -> (swipe left) -> Red -> (swipe left) -> deleted.
    //         Red -> (swipe right) -> Default -> (swipe right) -> Green.
    //         Green -> (swipe left) -> Default, mirroring Red's return path.
    public class SwipeableListItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

        private enum SwipeState { Default, DarkRed, Red, DarkGreen, Green }

        private const float SwipeThreshold = 60f;
        private const float HoldThresholdSeconds = 0.4f;
        private const float ReorderThreshold = 40f;

        private static readonly Color DefaultColor = new Color(1f, 1f, 1f, 0.06f);
        private static readonly Color DarkRedColor = new Color(0.5f, 0.0f, 0.0f, 1.0f);
        private static readonly Color RedColor = new Color(0.85f, 0.3f, 0.3f, 0.5f);
        private static readonly Color DarkGreenColor = new Color(0.0f, 0.39f, 0.15f, 1.0f);
        private static readonly Color GreenColor = new Color(0.3f, 0.75f, 0.4f, 0.5f);

        public Image RowBackground;
        public TMP_InputField ItemField;
        public Action OnDeleteRequested;

        // direction: -1 to move up (earlier in the list), +1 to move down.
        public Action<int> OnMoveRequested;

        // The ScrollRect's GameObject. A quick drag that turns out to be
        // predominantly vertical is handed off to this instead of being treated
        // as a swipe, so scrolling still works - see OnBeginDrag.
        public GameObject ScrollTarget;

        private SwipeState state = SwipeState.Default;
        private Vector2 dragStartPosition;
        private bool isHorizontalSwipe;
        private bool isHoldDrag;
        private float pointerDownTime;

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownTime = Time.unscaledTime;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragStartPosition = eventData.position;

            // Held still (no movement past Unity's own drag threshold) for at
            // least HoldThresholdSeconds before this drag started at all -> treat
            // the whole gesture as a reorder, regardless of its direction.
            isHoldDrag = Time.unscaledTime - pointerDownTime >= HoldThresholdSeconds;
            if (isHoldDrag) {
                return;
            }

            // Otherwise classify by total displacement since the initial press
            // (more reliable than a single frame's delta), and stick with that
            // classification for the rest of the drag.
            Vector2 totalDelta = eventData.position - eventData.pressPosition;
            isHorizontalSwipe = Mathf.Abs(totalDelta.x) > Mathf.Abs(totalDelta.y);

            if (!isHorizontalSwipe && ScrollTarget != null) {
                ExecuteEvents.Execute<IBeginDragHandler>(ScrollTarget, eventData, ExecuteEvents.beginDragHandler);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isHoldDrag) {
                return;
            }
            if (!isHorizontalSwipe && ScrollTarget != null) {
                ExecuteEvents.Execute<IDragHandler>(ScrollTarget, eventData, ExecuteEvents.dragHandler);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isHoldDrag) {
                float deltaY = eventData.position.y - dragStartPosition.y;
                if (Mathf.Abs(deltaY) >= ReorderThreshold) {
                    OnMoveRequested?.Invoke(deltaY > 0 ? -1 : 1);
                }
                return;
            }

            if (!isHorizontalSwipe) {
                if (ScrollTarget != null) {
                    ExecuteEvents.Execute<IEndDragHandler>(ScrollTarget, eventData, ExecuteEvents.endDragHandler);
                }
                return;
            }

            float deltaX = eventData.position.x - dragStartPosition.x;

            if (Mathf.Abs(deltaX) < SwipeThreshold) {
                return;
            }

            if (deltaX < 0) {
                HandleSwipeLeft();
            } else {
                HandleSwipeRight();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount >= 2) {
                ActivateEditing();
            }
        }

        private void ActivateEditing()
        {
            if (ItemField != null) {
                ItemField.Select();
                ItemField.ActivateInputField();
            }
        }

        private void HandleSwipeLeft()
        {
            switch (state) {
                case SwipeState.Default:
                    SetState(SwipeState.Red);
                    break;
                case SwipeState.Red:
                    SetState(SwipeState.DarkRed);
                    break;
                case SwipeState.DarkRed:
                    OnDeleteRequested?.Invoke();
                    break;
                case SwipeState.Green:
                    SetState(SwipeState.Default);
                    break;
                case SwipeState.DarkGreen:
                    SetState(SwipeState.Green);
                    break;
            }
        }

        private void HandleSwipeRight()
        {
            switch (state) {
                case SwipeState.DarkRed:
                    SetState(SwipeState.Red);
                    break;
                case SwipeState.Red:
                    SetState(SwipeState.Default);
                    break;
                case SwipeState.Default:
                    SetState(SwipeState.Green);
                    break;
                case SwipeState.Green:
                    SetState(SwipeState.DarkGreen);
                    break;
                case SwipeState.DarkGreen:
                    break;
            }
        }

        private void SetState(SwipeState newState)
        {
            state = newState;
            if (RowBackground == null) {
                return;
            }

            switch (state) {
                case SwipeState.Default:
                    RowBackground.color = DefaultColor;
                    break;
                case SwipeState.DarkRed:
                    RowBackground.color = DarkRedColor;
                    break;
                case SwipeState.Red:
                    RowBackground.color = RedColor;
                    break;
                case SwipeState.DarkGreen:
                    RowBackground.color = DarkGreenColor;
                    break;
                case SwipeState.Green:
                    RowBackground.color = GreenColor;
                    break;
            }
        }
    }
}
