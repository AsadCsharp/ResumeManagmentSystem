using ResumeManagement.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManagement.DTOs
{
    public class CandidateDTO
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime DateOfBirth { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public IFormFile? PictureFile { get; set; } 
        public bool IsFresher { get; set; }
        public string SkillsStringify { get; set; } = string.Empty;
        public Skill[] SkillList { get; set; } = new Skill[0];
    }
}
