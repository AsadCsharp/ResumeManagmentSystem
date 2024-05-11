using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using ResumeManagement.DTOs;
using ResumeManagement.Models;

namespace ResumeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IWebHostEnvironment _environment;

        public CandidatesController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }


        [Route("GetSkills")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _db.Skills.ToListAsync();
        }

      
        [Route("GetCandidates")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            return await _db.Candidates.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateDTO>>> GetCandidatesSkill()
        {
            List<CandidateDTO> candidateSkills = new List<CandidateDTO>();
            var allCandidate = _db.Candidates.ToList();
            foreach (var candidate in allCandidate)
            {
                var skillList = _db.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId)
                    .Select(x => new Skill
                    {
                        SkillId = x.SkillId,
                    }).ToList();
                candidateSkills.Add(new CandidateDTO
                {
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.CandidateName,
                    DateOfBirth = candidate.DateOfBirth,
                    MobileNo = candidate.MobileNo,
                    Picture = candidate.Picture,
                    IsFresher = candidate.IsFresher,
                    SkillList = skillList.ToArray()
                });
            }
            return Ok(candidateSkills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateDTO>> GetCandidateById(int id)
        {
            Candidate candidate = await _db.Candidates.FindAsync(id);
            Skill[] skills = _db.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId)
                .Select(x => new Skill
                {
                    SkillId = x.SkillId
                }).ToArray();
            CandidateDTO candidateDTO = new CandidateDTO()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                DateOfBirth = candidate.DateOfBirth,
                MobileNo = candidate.MobileNo,
                Picture = candidate.Picture,
                IsFresher = candidate.IsFresher,
                SkillList = skills.ToArray()
            };
            return Ok(candidateDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> PostCandidateSkill([FromForm] CandidateDTO candidateDTO)
        {
            var skills = JsonConvert.DeserializeObject<Skill[]>(candidateDTO.SkillsStringify);
            Candidate candidate = new Candidate()
            {
                CandidateName = candidateDTO.CandidateName,
                DateOfBirth = candidateDTO.DateOfBirth,
                MobileNo = candidateDTO.MobileNo,
                IsFresher = candidateDTO.IsFresher,
            };
            if(candidateDTO.PictureFile != null)
            {
                var webRoot = _environment.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidateDTO.PictureFile.FileName);
                var filePath = Path.Combine(webRoot, "Images", fileName);
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await candidateDTO.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }
            foreach(var item in skills)
            {
                var candidateSkills = new CandidateSkill()
                {
                    Candidate = candidate,
                    CandidateId = candidate.CandidateId,
                    SkillId = item.SkillId,
                };
                _db.Add(candidateSkills);
            }
            await _db.SaveChangesAsync();
            return Ok(candidate);
        }

        //[HttpPut("{id}")]
      
        [Route("Update")]
        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> PutCandidateSkill([FromForm] CandidateDTO candidateDTO)
        {
            var skills = JsonConvert.DeserializeObject<Skill[]>(candidateDTO.SkillsStringify);
            Candidate candidate = _db.Candidates.Find(candidateDTO.CandidateId);
            candidate.CandidateName = candidateDTO.CandidateName;
            candidate.DateOfBirth = candidateDTO.DateOfBirth;
            candidate.MobileNo = candidateDTO.MobileNo;
            candidate.IsFresher = candidateDTO.IsFresher;

            if (candidateDTO.PictureFile != null)
            {
                var webRoot = _environment.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidateDTO.PictureFile.FileName);
                var filePath = Path.Combine(webRoot, "Images", fileName);
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await candidateDTO.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }
            var existingSkills = _db.CandidateSkills.Where(x => x.CandidateId == candidateDTO.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _db.CandidateSkills.Remove(item);
            }
            foreach (var item in skills)
            {
                var candidateSkills = new CandidateSkill()
                {
                    CandidateId = candidate.CandidateId,
                    SkillId = item.SkillId,
                };
                _db.Add(candidateSkills);
            }
            _db.Entry(candidate).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(candidate);
        }

      
       
        [Route("Delete/{id}")]
        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> DeleteCandidate(int id)
        {
            Candidate candidate = _db.Candidates.Find(id);
            var existingSkills = _db.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _db.CandidateSkills.Remove(item);
            }
            _db.Entry(candidate).State = EntityState.Deleted;
            await _db.SaveChangesAsync();
            return Ok(candidate);
        }
    }
}
