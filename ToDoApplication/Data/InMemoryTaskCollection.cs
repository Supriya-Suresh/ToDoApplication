using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;
using ToDoApplication.Models;

namespace ToDoApplication.Data
{
    public class InMemoryTaskCollection
    {
        private static List<Tasks> tasks = new List<Tasks>
        {
            new Tasks { Id=Guid.NewGuid(),Name = "Task 1", Priority = 2, Status = "Not Started" },
            new Tasks { Id=Guid.NewGuid(),Name = "Task 2", Priority = 1, Status = "In Progress" },
            new Tasks { Id=Guid.NewGuid(), Name = "Task 3", Priority = 3, Status = "Completed" }
        };

        public static List<Tasks> GetTasks()
        {
            return tasks;
        }

        public static Tasks GetTaskById(Guid id)
        {
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public static void UpdateTask(Tasks task)
        {
            var existingTask = tasks.FirstOrDefault(t => t.Id == task.Id);
            if (task.Status == "Completed")
            {
                tasks.Remove(existingTask);
                
            }
            else
            {
                if (existingTask != null)
                {
                    existingTask.Name = task.Name;
                    existingTask.Priority = task.Priority;
                    existingTask.Status = task.Status;
                }
            }            
        }
    }
}
