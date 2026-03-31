using System.Collections.Generic;

namespace ProjectManagement.Models.ViewModels {

    public class UserAccessViewModel {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<int> PageId { get; set; }
        public int State { get; set; }
    }
}