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

        private const float SwipeThreshold = 60f;
        private const float HoldThresholdSeconds = 0.4f;
        private const float ReorderThreshold = 40f;
        private const float TapMaxDistance = 30f;
        private const float DoubleTapMaxInterval = 0.35f;
        private const float DoubleTapMaxDistance = 40f;

        public Image RowBackground;
        public TMP_InputField ItemField;
        public Action OnDeleteRequested;

        // direction: -1 to move up (earlier in the list), +1 to move down.
        public Action<int> OnMoveRequested;

        // Fires whenever the swipe state changes, so the caller can persist it
        // (ListsStore) - see also ApplyInitialState, for restoring it on creation.
        public Action<ItemSwipeState> OnStateChanged;

        // The ScrollRect's GameObject. A quick drag that turns out to be
        // predominantly vertical is handed off to this instead of being treated
        // as a swipe, so scrolling still works - see OnBeginDrag.
        public GameObject ScrollTarget;

        private ItemSwipeState state = ItemSwipeState.Default;
        private Vector2 dragStartPosition;
        private bool isHorizontalSwipe;
        private bool isHoldDrag;
        private float pointerDownTime;
        private float lastTapTime = -999f;
        private Vector2 lastTapPosition;

        // Sets the starting state (e.g. loaded from ListsStore) without treating
        // it as a new change - RowBackground must already be assigned. Call once,
        // right after creation.
        public void ApplyInitialState(ItemSwipeState initial)
        {
            state = initial;
            if (RowBackground != null) {
                RowBackground.color = ItemSwipeColors.Get(state);
            }
        }

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

            Vector2 totalDelta = eventData.position - dragStartPosition;

            if (!isHorizontalSwipe) {
                if (ScrollTarget != null) {
                    ExecuteEvents.Execute<IEndDragHandler>(ScrollTarget, eventData, ExecuteEvents.endDragHandler);
                }
                // A "vertical" classification on a near-zero movement is just
                // jitter, not an actual scroll attempt - still counts as a tap.
                if (totalDelta.magnitude < TapMaxDistance) {
                    RegisterTap(eventData.position);
                }
                return;
            }

            if (Mathf.Abs(totalDelta.x) < SwipeThreshold) {
                if (totalDelta.magnitude < TapMaxDistance) {
                    RegisterTap(eventData.position);
                }
                return;
            }

            if (totalDelta.x < 0) {
                HandleSwipeLeft();
            } else {
                HandleSwipeRight();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Covers the case OnBeginDrag/OnEndDrag never fire at all - a
            // genuinely zero-movement mouse click (Editor testing). Real touch
            // taps almost always move a few pixels and get classified as a
            // micro-drag instead, which is why OnEndDrag also calls RegisterTap.
            RegisterTap(eventData.position);
        }

        // Independent of Unity's own click-count tracking (unreliable here,
        // since this component also implements IBeginDragHandler - once a drag
        // starts, Unity won't fire OnPointerClick on release at all, which is
        // true for nearly every real touch tap due to ordinary finger jitter).
        private void RegisterTap(Vector2 position)
        {
            float now = Time.unscaledTime;
            bool isDoubleTap = now - lastTapTime <= DoubleTapMaxInterval
                && Vector2.Distance(position, lastTapPosition) <= DoubleTapMaxDistance;

            if (isDoubleTap) {
                ActivateEditing();
                lastTapTime = -999f; // consumed, so a 3rd quick tap doesn't chain into another double-tap
            } else {
                lastTapTime = now;
                lastTapPosition = position;
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
                case ItemSwipeState.Default:
                    SetState(ItemSwipeState.Red);
                    break;
                case ItemSwipeState.Red:
                    SetState(ItemSwipeState.DarkRed);
                    break;
                case ItemSwipeState.DarkRed:
                    OnDeleteRequested?.Invoke();
                    break;
                case ItemSwipeState.Green:
                    SetState(ItemSwipeState.Default);
                    break;
                case ItemSwipeState.DarkGreen:
                    SetState(ItemSwipeState.Green);
                    break;
            }
        }

        private void HandleSwipeRight()
        {
            switch (state) {
                case ItemSwipeState.DarkRed:
                    SetState(ItemSwipeState.Red);
                    break;
                case ItemSwipeState.Red:
                    SetState(ItemSwipeState.Default);
                    break;
                case ItemSwipeState.Default:
                    SetState(ItemSwipeState.Green);
                    break;
                case ItemSwipeState.Green:
                    SetState(ItemSwipeState.DarkGreen);
                    break;
                case ItemSwipeState.DarkGreen:
                    break;
            }
        }

        private void SetState(ItemSwipeState newState)
        {
            state = newState;
            if (RowBackground != null) {
                RowBackground.color = ItemSwipeColors.Get(state);
            }
            OnStateChanged?.Invoke(state);
        }
    }
}
