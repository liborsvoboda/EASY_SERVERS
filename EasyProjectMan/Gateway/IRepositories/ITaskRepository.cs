using ProjectManagement.Models.ViewModels;
using System.Collections.Generic;
using Task = ProjectManagement.Models.Task;

namespace ProjectManagement.Gateway.IRepositories {

    public interface ITaskRepository : IRepository<Task> {

        int RowCount();

        List<TaskViewModel> GetTasksByProjectId(int projectId);

        int GetNewTasks(int? userId);

        List<NewTaskViewModel> GetNewTasksList(int userId);

        int RowCountByProjectId(int projectId);
    }
}