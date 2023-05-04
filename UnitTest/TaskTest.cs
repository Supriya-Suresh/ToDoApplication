using NUnit.Framework.Internal;
using ToDoApplication.Data;
using ToDoApplication.Models;
using ToDoApplication.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest
{
    [TestClass]
    public class TaskTest
    {
        
        [TestMethod]
        public void GetTaskById_ReturnsTaskById()
        {
            // Arrange
            var task = InMemoryTaskCollection.GetTasks().First();

            // Act
            var Gettask = InMemoryTaskCollection.GetTaskById(task.Id);

            // Assert
            Assert.IsNotNull(task); // Ensure a task is returned
            Assert.AreEqual(task.Id, Gettask.Id); // Ensure the returned task has the correct ID
            Assert.AreEqual(task.Name, Gettask.Name); // Ensure the returned task has the correct Name
            Assert.AreEqual(task.Priority, Gettask.Priority); // Ensure the returned task has the correct Priority
        }


        [TestMethod]
        public async Task AddTask_AddsTask()
        {
            // Arrange
            var newTask = new Tasks { Id = new Guid(), Name = "Task 10", Priority = 3, Status = "In Progress" };
            var controller = new TasksController();

            // Act
            var result = await controller.AddTask(newTask);
            var task = InMemoryTaskCollection.GetTasks().Last();


            // Assert
            Assert.AreEqual("Task 10", task.Name); // Ensure the task name was updated to the correct value
            Assert.AreEqual(3, task.Priority); // Ensure the task priority was updated to the correct value
            Assert.AreEqual("In Progress", task.Status); // Ensure the task status was updated to the correct value
        }

        [TestMethod]
        public async Task AddCompletedTask_DoesNotAddTask()
        {
            // Arrange
            var newTask = new Tasks { Id = new Guid(), Name = "Task 11", Priority = 3, Status = "Completed" };
            var controller = new TasksController();

            // Act
            var result = await controller.AddTask(newTask);
            var task = InMemoryTaskCollection.GetTaskById(newTask.Id);

            // Assert
            Assert.IsNull(task);
        }

        [TestMethod]
        public async Task AddTaskWithSameName_DoesNotAddTask()
        {
            // Arrange
            var newTask = new Tasks { Id = new Guid(), Name = "Task 1", Priority = 3, Status = "In Progress" };
            var controller = new TasksController();

            // Act
            var result = await controller.AddTask(newTask);
            var tasks = InMemoryTaskCollection.GetTasks().Where(t => t.Name.Equals("Task 1", StringComparison.OrdinalIgnoreCase));
            int counts = tasks.Count();
            // Assert
            Assert.AreEqual(1, counts); 
        }

        [TestMethod]
        public async Task AddTask_WithDuplicateTask_ReturnsViewResultWithErrorMessage()
        {
            // Arrange
            var controller = new TasksController();
            var newTask = new Tasks { Id = new Guid(), Name = "Task 4", Priority = 3, Status = "In Progress" };
            controller.AddTask(newTask);
            var newDuplicateTask = new Tasks { Id = new Guid(), Name = "Task 4", Priority = 1, Status = "In Progress" };

            // Act
            var result = await controller.AddTask(newDuplicateTask) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Task name already exists.", result.ViewData.ModelState["task.Name"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public async Task Update_task_Completed_RemovesTask()
        {
            // Arrange
            var newTask = new Tasks { Id = new Guid(), Name = "Task 12", Priority = 3, Status = "Not Started" };
            Guid newTaskId = newTask.Id;
            var controller = new TasksController();
            var result = await controller.AddTask(newTask);

            // Act
            var task = InMemoryTaskCollection.GetTaskById(newTaskId);
            if (task != null)
            {
                newTask.Status = "Completed";
                var result2 = await controller.PostUpdateTask(newTask);
                var Tasks = InMemoryTaskCollection.GetTaskById(newTaskId);

                // Assert
                Assert.IsNull(Tasks);
            }
             
        }

        [TestMethod]
        public async Task UpdateTaskById_UpdatePriority_UpdateStatus_UpdatesTask()
        {
            // Arrange
            var newTask = new Tasks { Id = new Guid(), Name = "Task 5", Priority = 3, Status = "In Progress" };
            var controller = new TasksController();
            var result = await controller.AddTask(newTask);

            // Act

            var AddedTask = InMemoryTaskCollection.GetTasks().Last();
            AddedTask.Priority = 1;
            AddedTask.Status = "Not Started";
            var VerifyTask = InMemoryTaskCollection.GetTaskById(AddedTask.Id);

            // Assert
            Assert.AreEqual(1, VerifyTask.Priority); // Ensure the task priority was updated to the correct value
            Assert.AreEqual("Not Started", VerifyTask.Status); // Ensure the task status was updated to the correct value
        }

        [TestMethod]
        public async Task UpdateTask_WithDuplicateTaskName_ReturnsViewResultWithErrorMessage()
        {
            // Arrange
            var controller = new TasksController();
            var newTask1 = new Tasks { Id = new Guid(), Name = "Task 6", Priority = 3, Status = "In Progress" };
            var newTask2 = new Tasks { Id = new Guid(), Name = "Task 7", Priority = 1, Status = "In Progress" };
            var result1 = await controller.AddTask(newTask1);
            var result2 = await controller.AddTask(newTask2);


            // Act
            var getTask = InMemoryTaskCollection.GetTaskById(newTask1.Id);
            getTask.Name = "Task 7";
            var result = await controller.PostUpdateTask(getTask) as ViewResult;

            // Assert
            Assert.AreEqual("Task name already exists.",result.ViewData.ModelState["task.Name"].Errors[0].ErrorMessage);
        }
    }
}