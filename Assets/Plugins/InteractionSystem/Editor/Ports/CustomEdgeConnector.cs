using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using NodeEngine.Edges;

namespace NodeEngine.Ports
{
    public class CustomEdgeConnector<TEdge> : EdgeConnector where TEdge : DSEdge, new()
    {
        private readonly EdgeDragHelper m_EdgeDragHelper;

        private DSEdge m_EdgeCandidate;

        private bool m_Active;

        private Vector2 m_MouseDownPosition;

        internal const float k_ConnectionDistanceTreshold = 10f;

        public override EdgeDragHelper edgeDragHelper => m_EdgeDragHelper;

        public CustomEdgeConnector(IEdgeConnectorListener listener)
        {
            m_EdgeDragHelper = new EdgeDragHelper<TEdge>(listener);
            m_Active = false;
            base.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            base.target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            base.target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            base.target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            base.target.RegisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            base.target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            base.target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            base.target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            base.target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
            }
            else
            {
                if (!CanStartManipulation(e))
                {
                    return;
                }

                BasePort port = base.target as BasePort;
                if (port != null)
                {
                    m_MouseDownPosition = e.localMousePosition;
                    m_EdgeCandidate = new TEdge();
                    m_EdgeDragHelper.draggedPort = port;
                    m_EdgeDragHelper.edgeCandidate = m_EdgeCandidate;
                    if (m_EdgeDragHelper.HandleMouseDown(e))
                    {
                        m_Active = true;
                        base.target.CaptureMouse();
                        e.StopPropagation();
                    }
                    else
                    {
                        m_EdgeDragHelper.Reset();
                        m_EdgeCandidate = null;
                    }
                }
            }
        }

        private void OnCaptureOut(MouseCaptureOutEvent e)
        {
            m_Active = false;
            if (m_EdgeCandidate != null)
            {
                Abort();
            }
        }

        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            if (m_Active)
            {
                m_EdgeDragHelper.HandleMouseMove(e);
                m_EdgeCandidate.candidatePosition = e.mousePosition;
                m_EdgeCandidate.UpdateEdgeControl();
                e.StopPropagation();
            }
        }

        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (m_Active && CanStopManipulation(e))
            {
                if (CanPerformConnection(e.localMousePosition))
                {
                    m_EdgeDragHelper.HandleMouseUp(e);
                }
                else
                {
                    Abort();
                }

                m_Active = false;
                m_EdgeCandidate = null;
                base.target.ReleaseMouse();
                e.StopPropagation();
            }
        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode == KeyCode.Escape && m_Active)
            {
                Abort();
                m_Active = false;
                base.target.ReleaseMouse();
                e.StopPropagation();
            }
        }

        private void Abort()
        {
            (base.target?.GetFirstAncestorOfType<GraphView>())?.RemoveElement(m_EdgeCandidate);
            m_EdgeCandidate.input = null;
            m_EdgeCandidate.output = null;
            m_EdgeCandidate = null;
            m_EdgeDragHelper.Reset();
        }

        private bool CanPerformConnection(Vector2 mousePosition)
        {
            return Vector2.Distance(m_MouseDownPosition, mousePosition) > 10f;
        }
    }
}
