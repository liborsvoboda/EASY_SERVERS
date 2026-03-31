using ProjectManagement.Gateway.UnitOfWork;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
using System.Collections.Generic;

namespace ProjectManagement.Manager {

    public class UserAuthenticationManager {
        private UnitOfWork unitOfWork;

        public UserAuthenticationManager() {
            unitOfWork = new UnitOfWork();
        }

        // check authentication
        public User CheckAuthentication(AuthenticationViewModel model) {
            User user = unitOfWork.User.Find(
                x => x.Email == model.Email && x.Password == model.Password && x.State == 1);

            if (user == null) {
                return null;
            }
            else {
                return user;
            }
        }

        public int NumberOfProjects() {
            return unitOfWork.Project.RowCount();
        }

        public int NumberOfTasks() {
            return unitOfWork.Tasks.RowCount();
        }

        public int NewTasks() {
            return unitOfWork.Tasks.GetNewTasks(null);
        }

        public int NumberOfNewComments() {
            return unitOfWork.Comment.RowCount();
        }

        public List<int> GetProjectNumberByMonth() {
            return unitOfWork.Project.GetProjectCountByMonth();
        }

        public List<int> ThisAndLastTwoYearProjectNumber() {
            return unitOfWork.Project.NumberOfThisAndPrevYearProject();
        }
    }
}