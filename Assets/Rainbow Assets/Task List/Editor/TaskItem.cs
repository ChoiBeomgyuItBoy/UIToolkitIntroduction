using UnityEditor;
using UnityEngine.UIElements;

namespace RainbowAssets.TaskList.Editor
{
    public class TaskItem : VisualElement
    {
        Toggle taskToggle;
        Label taskLabel;

        public TaskItem(string taskText)
        {
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TaskListEditor.path + "TaskItem.uxml");
            this.Add(original.Instantiate());

            taskToggle = this.Q<Toggle>();
            taskLabel = this.Q<Label>();

            taskLabel.text = taskText;
        }

        public Toggle GetTaskToggle()
        {
            return taskToggle;
        }

        public Label GetTaskLabel()
        {
            return taskLabel;
        }
    }
}