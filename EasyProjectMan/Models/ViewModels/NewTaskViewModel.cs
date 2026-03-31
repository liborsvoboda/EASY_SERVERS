namespace ProjectManagement.Models.ViewModels {

    public class NewTaskViewModel {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string By { get; set; }
        public string Priority { get; set; }
    }
}