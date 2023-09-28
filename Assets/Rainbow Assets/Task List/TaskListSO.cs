using System.Collections.Generic;
using UnityEngine;

namespace RainbowAssets.TaskList
{
    [CreateAssetMenu(menuName = "Task List/New Task List")]
    public class TaskListSO : ScriptableObject
    {
        [SerializeField] List<string> tasks = new List<string>();

        public List<string> GetTasks()
        {
            return tasks;
        }

        public void AddTask(string savedTask)
        {
            tasks.Add(savedTask);
        }

        public void AddTasks(List<string> savedTasks)
        {
            tasks.Clear();
            tasks = savedTasks;
        }
    }
}