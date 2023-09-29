using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

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
        Label notificationLabel;

        public const string path = "Assets/Rainbow Assets/Task List/Editor/";

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
            
            notificationLabel = container.Q<Label>("notificationLabel");

            UpdateNotifications("Please load a task list to continue.");
        }

        private TaskItem CreateTask(string taskText)
        {
            TaskItem taskItem = new TaskItem(taskText);
            taskItem.GetTaskToggle().RegisterValueChangedCallback(UpdateProgress);
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
                UpdateNotifications("Task added successfully.");
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
            UpdateNotifications("Task added successfully.");
        }

        private void LoadTasks()
        {
            currentTaskList = savedTasksObjectField.value as TaskListSO;

            if(currentTaskList == null)
            {
                UpdateNotifications("Failed to load task list.");
                return;
            }

            taskListScrollView.Clear();
            List<string> tasks = currentTaskList.GetTasks();

            foreach(string task in tasks)
            {
                taskListScrollView.Add(CreateTask(task));
            }

            UpdateProgress();
            UpdateNotifications($"{currentTaskList.name} sucessfully loaded.");
        }

        private void SaveProgress()
        {
            if(currentTaskList == null)
            {
                return;
            }

            List<string> tasks = new List<string>();

            foreach(TaskItem task in taskListScrollView.Children())
            {
                if(!task.GetTaskToggle().value)
                {
                    tasks.Add(task.GetTaskLabel().text);
                }
            }

            currentTaskList.AddTasks(tasks);
            EditorUtility.SetDirty(currentTaskList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LoadTasks();
            UpdateNotifications("Progress saved succesfully.");
        }

        private void UpdateProgress()
        {
            if(currentTaskList == null)
            {
                return;
            }

            int count = 0;
            int completed = 0;

            foreach(TaskItem task in taskListScrollView.Children())
            {
                if(task.GetTaskToggle().value)
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
                UpdateNotifications("Progress updated. Don't forget to save!");
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

            foreach(TaskItem task in taskListScrollView.Children())
            {
                string taskText = task.GetTaskLabel().text.ToUpper();

                if(!string.IsNullOrEmpty(searchText) && taskText.Contains(searchText))
                {
                    task.GetTaskLabel().AddToClassList("highlight");
                }
                else
                {
                    task.GetTaskLabel().RemoveFromClassList("highlight");
                }
            }
        }

        private void UpdateNotifications(string text)
        {
            if(!string.IsNullOrEmpty(text))
            {
                notificationLabel.text = text;
            }
        }
    }   
}
