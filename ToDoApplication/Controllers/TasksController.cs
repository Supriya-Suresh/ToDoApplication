using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDoApplication.Data;
using ToDoApplication.Models;

namespace ToDoApplication.Controllers
{
    public class TasksController : Controller
    {
        

        public IActionResult Index()
        {
            var CompletedTasks = InMemoryTaskCollection.GetTasks().Where(t => t.Status == "Completed").ToList();
            if (CompletedTasks.Any())
            {
                InMemoryTaskCollection.GetTasks().RemoveAll(t => t.Status == "Completed");
            }
            var tasks = InMemoryTaskCollection.GetTasks();
            return View(tasks);
        }

        [HttpGet("GetTasks")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await Task.Run(() => InMemoryTaskCollection.GetTasks());
            return View(tasks);
        }

        public async Task<IActionResult> AddTask()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddTask(Tasks task)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            // Validation : Check if the task name already exists
            var existingTask = InMemoryTaskCollection.GetTasks()
                .Find(t => t.Name.Equals(task.Name, StringComparison.OrdinalIgnoreCase));
            if (existingTask != null)
            {
                ModelState.AddModelError("task.Name", "Task name already exists.");
                return View();
            }

            //If the status is Completed then no need to add to the list
            if (task.Status != "Completed")
            {
                var newTask = task;
                newTask.Id = Guid.NewGuid();
                InMemoryTaskCollection.GetTasks().Add(newTask);
            }
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> UpdateTask(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }
            var task = InMemoryTaskCollection.GetTaskById(id);
            if (task == null)
            {
                return RedirectToAction("Index");
            }
            return View(task);
        }


        [HttpPost("PostUpdateTask")]
        public async Task<IActionResult> PostUpdateTask(Tasks task)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UpdateTask",task);
            }

            var oldTask = InMemoryTaskCollection.GetTaskById(task.Id);
            if (oldTask == null)
            {
                return NotFound();
            }

            // Validation : Check if the task name already exists
            var existingTask = InMemoryTaskCollection.GetTasks()
                .Any(t => t.Id != oldTask.Id && t.Name.Equals(task.Name, StringComparison.OrdinalIgnoreCase));
            if (existingTask)
            {
                ModelState.AddModelError("task.Name", "Task name already exists.");
                return View("UpdateTask", task);
            }

            InMemoryTaskCollection.UpdateTask(task);

            //InMemoryTaskCollection.GetTasks().Remove(oldTask);
            //InMemoryTaskCollection.GetTasks().Add(task);
            return RedirectToAction("Index");            
            
        }
    }
}
