using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Command = Scribble.Command;

sealed class ScribbleManipulator : PointerManipulator
{
    #region Command queue

    public Queue<Command> Commands { get; private set; } = new Queue<Command>();

    #endregion

    #region Private variables

    int _pointerID;

    bool IsActive => _pointerID >= 0;

    #endregion

    #region PointerManipulator implementation

    public ScribbleManipulator(Scribble scribble)
    {
        _pointerID = -1;
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    #endregion

    #region Pointer callbacks

    void OnPointerDown(PointerDownEvent e)
    {
        if (IsActive)
        {
            e.StopImmediatePropagation();
        }
        else if (CanStartManipulation(e))
        {
            Commands.Enqueue(Command.NewDown(e.localPosition));
            target.CapturePointer(_pointerID = e.pointerId);
            e.StopPropagation();
        }
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_pointerID)) return;
        Commands.Enqueue(Command.NewMove(e.localPosition));
        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        if (!IsActive || !target.HasPointerCapture(_pointerID)) return;

        if (CanStopManipulation(e))
        {
            Commands.Enqueue(Command.NewUp(e.localPosition));
            _pointerID = -1;
            target.ReleaseMouse();
            e.StopPropagation();
        }
    }

    #endregion
}
