using ProjectManagement.Models;

namespace ProjectManagement.Gateway.IRepositories {

    public interface IDesignationRepository : IRepository<Designation> {

        int RowCount();
    }
}