using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
using System.Collections.Generic;

namespace ProjectManagement.Gateway.IRepositories {

    public interface IAssignUserRepository : IRepository<AssignProject> {

        List<AssignedUserViewModel> GetAssignedUsers();

        List<User> GetAssignedUserByProjectId(int projectId);
    }
}