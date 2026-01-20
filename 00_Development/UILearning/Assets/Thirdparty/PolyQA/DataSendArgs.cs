using System;
using System.Numerics;
using MemoryPack;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace PolyQA
{
    [MemoryPackable]
    public readonly partial struct Void
    {
        public static readonly Void Default = new();
    }

    [MemoryPackable]
    public readonly partial struct DataSendArgs<T>
    {
        public readonly string Key;
        public readonly int Frame;
        public readonly T Value;
        public DataSendArgs(string key, int frame, T value)
        {
            Key = key;
            Frame = frame;
            Value = value;
        }
    }

    public static class OS
    {
        public const string Windows = "Windows";
        public const string MacOS = "MacOS";
        public const string Linux = "Linux";
        public const string Android = "Android";
        public const string IOS = "iOS";
        public const string Unknown = "Unknown";
    }

    [MemoryPackable]
    public readonly partial struct StartSessionRequest
    {
        public readonly string SessionId;
        public readonly string SDKVersion;

        /// <summary>
        /// Versionの変更履歴は下記参照
        /// https://www.notion.so/Protocol-Version-10de1c3a22ef805395cbef8b096e3afa
        /// </summary>
        public readonly int ProtocolVersion;

        public readonly string OS;

        public readonly string AppId;

        public StartSessionRequest(
            string sessionId, string sdkVersion, int protocolVersion, string os, string appId)
        {
            SessionId = sessionId;
            SDKVersion = sdkVersion;
            ProtocolVersion = protocolVersion;
            OS = os;
            AppId = appId;
        }
    }

    public static class EventSystemAction
    {
        public const string Click = "EventSystems.Click";
        public const string PointerDown = "EventSystems.PointerDown";
        public const string PointerUp = "EventSystems.PointerUp";
        public const string PointerMove = "EventSystems.PointerMove";
        public const string Drag = "EventSystems.Drag";
        public const string EndDrag = "EventSystems.EndDrag";
        public const string Drop = "EventSystems.Drop";
    }

    public static class UIAction
    {
        public const string InputText = "UI.InputText";
    }

    [MemoryPackable]
    public readonly partial struct GameObjectUpdate
    {
        public readonly string Path;
        public readonly bool IsInteractable;
        public readonly string[] AvailableActions;
        public readonly int Frame;
        public readonly string Label;

        public GameObjectUpdate(string path, string label, bool isInteractable, string[] availableActions, int frame)
        {
            Path = path;
            Label = label;
            IsInteractable = isInteractable;
            AvailableActions = availableActions;
            Frame = frame;
        }
    }

    [MemoryPackable]
    public readonly partial struct InteractableGameObject
    {
        public readonly string Path;
        public readonly string[] ClassNames;
        public readonly Rect Rect;
        public readonly string Label;

        public InteractableGameObject(string path, string[] classNames, Rect rect, string label)
        {
            Path = path;
            ClassNames = classNames;
            Rect = rect;
            Label = label;
        }
    }

    [MemoryPackable]
    public readonly partial struct GetInteractableGameObjectsResponse
    {
        public readonly int Frame;
        public readonly Vector2 ScreenSize;
        public readonly InteractableGameObject[] GameObjects;

        public GetInteractableGameObjectsResponse(int frame, Vector2 screenSize, InteractableGameObject[] gameObjects)
        {
            Frame = frame;
            ScreenSize = screenSize;
            GameObjects = gameObjects;
        }
    }

    [MemoryPackable]
    public readonly partial struct InputActionRequest
    {
        public readonly IInputActionSequence[] Sequences;

        public InputActionRequest(IInputActionSequence[] sequences)
        {
            Sequences = sequences;
        }
    }

    [MemoryPackable]
    public readonly partial struct InputRecordUpdate
    {
        public readonly IInputActionSequence[] Sequences;
        public readonly TimeSpan Delay;

        public InputRecordUpdate(IInputActionSequence[] sequences, TimeSpan delay)
        {
            Sequences = sequences;
            Delay = delay;
        }
    }

    [MemoryPackable]
    [MemoryPackUnion(0, typeof(InputMouseActionSequence))]
    [MemoryPackUnion(1, typeof(InputTouchActionSequence))]
    [MemoryPackUnion(2, typeof(InputTouchRecordSequence))]
    [MemoryPackUnion(3, typeof(InputTextSequence))]
    public partial interface IInputActionSequence
    {
    }

    public static class MouseButton
    {
        public const int Left = 0;
        public const int Right = 1;
        public const int Middle = 2;
    }

    [MemoryPackable]
    public partial class InputMouseActionSequence : IInputActionSequence
    {
        public readonly int Mouse;
        public readonly IInputPointerAction[] Actions;

        [MemoryPackConstructor]
        public InputMouseActionSequence(int mouse, IInputPointerAction[] actions)
        {
            Mouse = mouse;
            Actions = actions;
        }

        public InputMouseActionSequence(IInputPointerAction[] actions) : this(MouseButton.Left, actions)
        {
        }
    }

    [MemoryPackable]
    public partial class InputTouchActionSequence : IInputActionSequence
    {
        public readonly int FingerId;
        public readonly IInputPointerAction[] Actions;

        [MemoryPackConstructor]
        public InputTouchActionSequence(int fingerId,  IInputPointerAction[] actions)
        {
            FingerId = fingerId;
            Actions = actions;
        }

        public InputTouchActionSequence(IInputPointerAction[] actions) : this(0, actions)
        {
        }
    }

    [MemoryPackable]
    [MemoryPackUnion(0, typeof(PointerAction))]
    [MemoryPackUnion(1, typeof(GameObjectPointerAction))]
    [MemoryPackUnion(2, typeof(PixelPointerAction))]
    [MemoryPackUnion(3, typeof(ScreenPointerAction))]
    public partial interface IInputPointerAction
    {
    }

    [MemoryPackable]
    public partial class PointerAction : IInputPointerAction
    {
        public const string Down = "PointerDown";
        public const string Up = "PointerUp";
        public const string Move = "PointerMove";
        public const string Stationary = "PointerStationary";
        public const string Cancel = "PointerCancel";

        public readonly string Action;
        public readonly TimeSpan Delay;

        [MemoryPackConstructor]
        public PointerAction(string action, TimeSpan delay)
        {
            Action = action;
            Delay = delay;
        }

        public PointerAction(string action) : this(action, TimeSpan.Zero)
        {
        }
    }

    [MemoryPackable]
    public partial class GameObjectPointerAction : PointerAction
    {
        public readonly string Path;

        [MemoryPackConstructor]
        public GameObjectPointerAction(string path, string action, TimeSpan delay) : base(action, delay)
        {
            Path = path;
        }

        public GameObjectPointerAction(string path, string action) : base(action)
        {
            Path = path;
        }
    }

    [MemoryPackable]
    public partial class PixelPointerAction : PointerAction
    {
        public readonly Vector2 Position;

        [MemoryPackConstructor]
        public PixelPointerAction(Vector2 position, string action, TimeSpan delay) : base(action, delay)
        {
            Position = position;
        }

        public PixelPointerAction(Vector2 position, string action) : base(action)
        {
            Position = position;
        }
    }

    [MemoryPackable]
    public partial class ScreenPointerAction : PointerAction
    {
        public readonly Vector2 Position;

        [MemoryPackConstructor]
        public ScreenPointerAction(Vector2 position, string action, TimeSpan delay) : base(action, delay)
        {
            Position = position;
        }

        public ScreenPointerAction(Vector2 position, string action) : base(action)
        {
            Position = position;
        }
    }

    [MemoryPackable]
    public partial class InputTouchRecordSequence : IInputActionSequence
    {
        public readonly InputTouchRecordAction[] Actions;

        public InputTouchRecordSequence(InputTouchRecordAction[] actions)
        {
            Actions = actions;
        }
    }

    [MemoryPackable]
    public partial class InputTouchRecordAction
    {
        public readonly int TouchId;
        public readonly string Action;
        public readonly TimeSpan Delay;
        public readonly Vector2 Position;

        public InputTouchRecordAction(int touchId, string action, TimeSpan delay, Vector2 position)
        {
            TouchId = touchId;
            Action = action;
            Delay = delay;
            Position = position;
        }
    }

    [MemoryPackable]
    public partial class InputTextSequence : IInputActionSequence
    {
        public readonly string Path;
        public readonly string Text;

        public InputTextSequence(string path, string text)
        {
            Path = path;
            Text = text;
        }
    }

    [MemoryPackable]
    public partial struct FindGameObjectRequest_V1
    {
        public readonly string Name;
        public readonly int GameObjectInstanceId;
        public readonly bool IsRegex;

        public FindGameObjectRequest_V1(string name, int gameObjectInstanceId, bool isRegex)
        {
            Name = name;
            GameObjectInstanceId = gameObjectInstanceId;
            IsRegex = isRegex;
        }
    }

    [MemoryPackable]
    public partial struct FindGameObjectRequest
    {
        public readonly string Name;
        public readonly int GameObjectInstanceId;
        public readonly bool IsRegex;
        public readonly bool IsChildrenOnly;

        public FindGameObjectRequest(string name, int gameObjectInstanceId, bool isRegex, bool isChildrenOnly)
        {
            Name = name;
            GameObjectInstanceId = gameObjectInstanceId;
            IsRegex = isRegex;
            IsChildrenOnly = isChildrenOnly;
        }
    }

    [MemoryPackable]
    public partial struct GetComponentRequest
    {
        public readonly int GameObjectInstanceId;
        public readonly string ComponentClass;

        public GetComponentRequest(int gameObjectInstanceId, string componentClass)
        {
            GameObjectInstanceId = gameObjectInstanceId;
            ComponentClass = componentClass;
        }
    }

    [MemoryPackable]
    public partial struct GetComponentValueRequest
    {
        public readonly int ComponentInstanceId;
        public readonly string PropertyName;

        public GetComponentValueRequest(int componentInstanceId, string propertyName)
        {
            ComponentInstanceId = componentInstanceId;
            PropertyName = propertyName;
        }
    }
}