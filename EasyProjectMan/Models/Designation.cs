using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models {

    public class Designation {
        public int Id { get; set; }

        [Required(ErrorMessage = "Designation Required")]
        public string DesignationName { get; set; }

        public int State { get; set; }
        public List<User> Users { get; set; }
    }
}