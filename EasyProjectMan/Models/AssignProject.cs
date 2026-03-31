using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models {

    public class AssignProject {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project is Required")]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [Required(ErrorMessage = "User is Required")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
        public int State { get; set; }
    }
}