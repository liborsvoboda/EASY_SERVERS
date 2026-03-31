using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
using System.Collections.Generic;

namespace ProjectManagement.Gateway.IRepositories {

    public interface IUserRepository : IRepository<User> {

        int RowCount();

        List<UserViewModel> GetAllUsers();
    }
}