using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace RainbowAssets.TaskList.Editor
{
    public class TaskListEditor : EditorWindow
    {   
        VisualElement container;
        TextField taskText;
        Button addTaskButton;
        ScrollView taskListScrollView;

        const string path = "Assets/Rainbow Assets/Task List/Editor/";

        [MenuItem("Rainbow Assets/Task List Editor")]
        public static void OpenWindow()
        {
            TaskListEditor window = GetWindow<TaskListEditor>();
            window.titleContent = new GUIContent("Task List");
        }

        public void CreateGUI()
        {
            container = rootVisualElement;

            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "TaskListEditor.uxml");
            container.Add(original.Instantiate());

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "TaskListEditor.uss");
            container.styleSheets.Add(styleSheet);

            taskText = container.Q<TextField>("taskText");
            taskText.RegisterCallback<KeyDownEvent>(AddTask);

            addTaskButton = container.Q<Button>("addTaskButton");
            addTaskButton.clicked += AddTask;

            taskListScrollView = container.Q<ScrollView>("taskListScrollView");
        }

        private void AddTask()
        {
            if(!string.IsNullOrEmpty(taskText.value))
            {
                Toggle taskItem = new Toggle();
                taskListScrollView.Add(taskItem);
                taskItem.text = taskText.value;
                taskText.value = "";
                taskText.Focus();
            }
        }

        private void AddTask(KeyDownEvent e)
        {
            if(Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                AddTask();
            }
        }
    }   
}
