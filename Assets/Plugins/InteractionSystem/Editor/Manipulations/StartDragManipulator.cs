using NodeEngine.Ports;
using UnityEngine.UIElements;

namespace NodeEngine.Manipulations
{
    public class StartDragManipulator : MouseManipulator
    {
        public StartDragManipulator(BasePort basePort)
        {
            BasePortManager.Register(basePort);
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }


        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            var result = CanStartManipulation(e);
            if (result)
            {
                e.StopPropagation();
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            var result = CanStartManipulation(e);
            if (result)
            {
                e.StopPropagation();
            }
        }
    }
}
