using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
        Button saveProgressButton;
        ProgressBar taskProgressBar;
        ToolbarSearchField searchBox;

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

            saveProgressButton = container.Q<Button>("saveProgressButton");
            saveProgressButton.clicked += SaveProgress;

            taskProgressBar = container.Q<ProgressBar>("taskProgressBar");

            searchBox = container.Q<ToolbarSearchField>("searchBox");
            searchBox.RegisterValueChangedCallback(OnSearchTextChanged);
        }

        private Toggle CreateTask(string taskText)
        {
            Toggle taskItem = new Toggle();
            taskItem.text = taskText;
            taskItem.RegisterValueChangedCallback(UpdateProgress);
            return taskItem;
        }

        private void AddTask(KeyDownEvent evt)
        {
            if(Event.current.Equals(Event.KeyboardEvent("Return")))
            {
                AddTask();
            }
        }
        
        private void AddTask()
        {
            if(currentTaskList == null)
            {
                return;
            }

            if(!string.IsNullOrEmpty(taskText.value))
            {
                taskListScrollView.Add(CreateTask(taskText.value));
                SaveTask(taskText.value);
                taskText.value = "";
                taskText.Focus();
                UpdateProgress();
            }
        }

        private void SaveTask(string task)
        {
            if(currentTaskList == null)
            {
                return;
            }

            currentTaskList.AddTask(task);
            EditorUtility.SetDirty(currentTaskList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadTasks()
        {
            currentTaskList = savedTasksObjectField.value as TaskListSO;

            if(currentTaskList == null)
            {
                return;
            }

            taskListScrollView.Clear();
            List<string> tasks = currentTaskList.GetTasks();

            foreach(string task in tasks)
            {
                taskListScrollView.Add(CreateTask(task));
            }

            UpdateProgress();
        }

        private void SaveProgress()
        {
            if(currentTaskList == null)
            {
                return;
            }

            List<string> tasks = new List<string>();

            foreach(Toggle task in taskListScrollView.Children())
            {
                if(!task.value)
                {
                    tasks.Add(task.text);
                }
            }

            currentTaskList.AddTasks(tasks);
            EditorUtility.SetDirty(currentTaskList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LoadTasks();

        }

        private void UpdateProgress()
        {
            if(currentTaskList == null)
            {
                return;
            }

            int count = 0;
            int completed = 0;

            foreach(Toggle task in taskListScrollView.Children())
            {
                if(task.value)
                {
                    completed++;
                }

                count++;
            }

            if(count > 0)
            {
                float progress = completed / (float) count;
                taskProgressBar.value = progress;
                taskProgressBar.title = $"{Mathf.Round(progress * 1000) / 10f}%";
            }
            else
            {
                taskProgressBar.value = 1;
                taskProgressBar.title = $"{100}%";
            }
        }

        private void UpdateProgress(ChangeEvent<bool> evt)
        {
            UpdateProgress();
        }

        private void OnSearchTextChanged(ChangeEvent<string> evt)
        {
            if(currentTaskList == null)
            {
                return;
            }

            string searchText = evt.newValue.ToUpper();

            foreach(Toggle task in taskListScrollView.Children())
            {
                string taskText = task.text.ToUpper();

                if(!string.IsNullOrEmpty(searchText) && taskText.Contains(searchText))
                {
                    task.AddToClassList("highlight");
                }
                else
                {
                    task.RemoveFromClassList("highlight");
                }
            }
        }
    }   
}
