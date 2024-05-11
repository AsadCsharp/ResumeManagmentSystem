using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagement.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        [Column(TypeName ="date")]
        public DateTime DateOfBirth { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public bool IsFresher { get; set; }
    }
}
