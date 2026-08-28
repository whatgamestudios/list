// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    // Sits on a transparent overlay covering a list item row, on top of its
    // TMP_InputField, so horizontal swipes can be captured before the input
    // field's own drag-to-select-text handling claims them (it would, for any
    // gesture starting within its bounds, since it's the exact raycast hit and
    // Unity's event bubbling only kicks in when the hit object itself has no
    // handler). A tap/click that isn't a swipe is forwarded to activate the
    // input field underneath, so tap-to-edit keeps working.
    //
    // States: Default -> (swipe left) -> Red -> (swipe left) -> deleted.
    //         Red -> (swipe right) -> Default -> (swipe right) -> Green.
    //         Green -> (swipe left) -> Default, mirroring Red's return path.
    public class SwipeableListItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

        private enum SwipeState { Default, Red, Green }

        private const float SwipeThreshold = 60f;

        private static readonly Color DefaultColor = new Color(1f, 1f, 1f, 0.06f);
        private static readonly Color RedColor = new Color(0.85f, 0.3f, 0.3f, 0.5f);
        private static readonly Color GreenColor = new Color(0.3f, 0.75f, 0.4f, 0.5f);

        public Image RowBackground;
        public TMP_InputField ItemField;
        public Action OnDeleteRequested;

        private SwipeState state = SwipeState.Default;
        private Vector2 dragStartPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragStartPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float deltaX = eventData.position.x - dragStartPosition.x;

            if (Mathf.Abs(deltaX) < SwipeThreshold) {
                ActivateEditing();
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
            ActivateEditing();
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
                    OnDeleteRequested?.Invoke();
                    break;
                case SwipeState.Green:
                    SetState(SwipeState.Default);
                    break;
            }
        }

        private void HandleSwipeRight()
        {
            switch (state) {
                case SwipeState.Red:
                    SetState(SwipeState.Default);
                    break;
                case SwipeState.Default:
                    SetState(SwipeState.Green);
                    break;
                case SwipeState.Green:
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
                case SwipeState.Red:
                    RowBackground.color = RedColor;
                    break;
                case SwipeState.Green:
                    RowBackground.color = GreenColor;
                    break;
            }
        }
    }
}
