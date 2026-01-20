#if POLYQA_USE_UGUI

using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyQA.Input
{
    public class UnityLegacyInputModifier : BaseInput
    {
        public BaseInput OriginalInput { get; private set; }
        private InputState _state;

        public void Init(BaseInput originalInput, InputState state)
        {
            OriginalInput = originalInput;
            _state = state;
        }

        public override string compositionString => OriginalInput.compositionString;

        public override IMECompositionMode imeCompositionMode
        {
            get => OriginalInput.imeCompositionMode;
            set => OriginalInput.imeCompositionMode = value;
        }

        public override Vector2 compositionCursorPos
        {
            get => OriginalInput.compositionCursorPos;
            set => OriginalInput.compositionCursorPos = value;
        }

        public override bool mousePresent => OriginalInput.mousePresent || _state.IsMouseEnabled;

        public override bool GetMouseButtonDown(int button) => OriginalInput.GetMouseButtonDown(button);

        public override bool GetMouseButtonUp(int button) => OriginalInput.GetMouseButtonUp(button);

        public override bool GetMouseButton(int button) => OriginalInput.GetMouseButton(button);

        public override Vector2 mousePosition => OriginalInput.mousePosition;

        public override Vector2 mouseScrollDelta => OriginalInput.mouseScrollDelta;

        public override bool touchSupported => OriginalInput.touchSupported || _state.IsTouchEnabled;

        public override int touchCount => _state.Touches.Count > 0 ? _state.Touches.Count : OriginalInput.touchCount;

        public override Touch GetTouch(int index) =>
            _state.Touches.Count > 0 ? _state.Touches[index] : OriginalInput.GetTouch(index);

        public override float GetAxisRaw(string axisName) => OriginalInput.GetAxisRaw(axisName);

        public override bool GetButtonDown(string buttonName) => OriginalInput.GetButtonDown(buttonName);
    }
}

#endif
