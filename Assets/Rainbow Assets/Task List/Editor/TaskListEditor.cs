using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

namespace RainbowAssets.TaskList.Editor
{
    public class TaskListEditor : EditorWindow
    {   
        VisualElement container;
        ObjectField savedTasksObjectField;
        Button loadTasksButton;
        TextField taskText;
        Button addTaskButton;
        ScrollView taskListScrollView;

        TaskListSO currentTaskList;

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

            savedTasksObjectField = container.Q<ObjectField>("savedTasksObjectField");
            savedTasksObjectField.objectType = typeof(TaskListSO);

            loadTasksButton = container.Q<Button>("loadTasksButton");
            loadTasksButton.clicked += LoadTasks;

            taskText = container.Q<TextField>("taskText");
            taskText.RegisterCallback<KeyDownEvent>(AddTask);

            addTaskButton = container.Q<Button>("addTaskButton");
            addTaskButton.clicked += AddTask;

            taskListScrollView = container.Q<ScrollView>("taskListScrollView");
        }

        private Toggle CreateTask(string taskText)
        {
            Toggle taskItem = new Toggle();
            taskItem.text = taskText;
            return taskItem;
        }

        private void AddTask(KeyDownEvent e)
        {
            if(Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                AddTask();
            }
        }
        
        private void AddTask()
        {
            if(!string.IsNullOrEmpty(taskText.value))
            {
                taskListScrollView.Add(CreateTask(taskText.value));
                SaveTask(taskText.value);
                taskText.value = "";
                taskText.Focus();
            }
        }

        private void SaveTask(string task)
        {
            currentTaskList.AddTask(task);
            EditorUtility.SetDirty(currentTaskList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadTasks()
        {
            currentTaskList = savedTasksObjectField.value as TaskListSO;

            if(currentTaskList != null)
            {
                taskListScrollView.Clear();
                List<string> tasks = currentTaskList.GetTasks();

                foreach(string task in tasks)
                {
                   taskListScrollView.Add(CreateTask(task));
                }
            }
        }
    }   
}
