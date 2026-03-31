using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models {

    public class UserAccess {
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PageId { get; set; }

        public int State { get; set; }
        public User User { get; set; }
    }
}