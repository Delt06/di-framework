using DELTation.DIFramework.Containers;
using UnityEditor;
using UnityEngine.UIElements;

namespace DELTation.DIFramework.Editor.DependencyGraph
{
    public class DependencyGraphWindow : EditorWindow
    {
        private DependencyGraphView _graphView;

        private void OnEnable()
        {
            if (_graphView == null) return;
            if (!rootVisualElement.Contains(_graphView))
                rootVisualElement.Add(_graphView);
        }

        private void OnDisable()
        {
            if (_graphView != null)
                rootVisualElement.Remove(_graphView);
        }

        public void Initialize(DependencyContainerBase container)
        {
            _graphView = new DependencyGraphView(container);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
    }
}