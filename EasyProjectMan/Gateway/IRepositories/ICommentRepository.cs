using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
using System.Collections.Generic;

namespace ProjectManagement.Gateway.IRepositories {

    public interface ICommentRepository : IRepository<Comment> {

        List<CommentViewModel> GetCommentsByProjectAndTask(int projectId, int taskId);

        int RowCount(List<int> projectIds, string mention, int userId);

        int RowCount();

        List<NewCommentViewModel> GetNewCommentList(List<int> projectIds, string mention, int userId);
    }
}