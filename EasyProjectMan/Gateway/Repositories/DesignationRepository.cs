using ProjectManagement.Gateway.IRepositories;
using ProjectManagement.Models;
using ProjectManagement.Models.Context;
using System.Linq;

namespace ProjectManagement.Gateway.Repositories {

    public class DesignationRepository : Repository<Designation>, IDesignationRepository {
        private ApplicationDbContext context;

        public DesignationRepository(ApplicationDbContext context) : base(context) {
            this.context = context;
        }

        public int RowCount() {
            return context.Designations.ToList().Count;
        }
    }
}