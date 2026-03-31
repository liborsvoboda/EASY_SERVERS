using ProjectManagement.Models;
using System.Collections.Generic;

namespace ProjectManagement.Gateway.IRepositories {

    public interface IProjectRepository : IRepository<Project> {

        int RowCount();

        Project GetProjectByProjectId(int projectId);

        List<int> GetProjectCountByMonth();

        List<int> NumberOfThisAndPrevYearProject();
    }
}