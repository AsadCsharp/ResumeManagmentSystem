using Microsoft.EntityFrameworkCore;

namespace ResumeManagement.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill { SkillId = 1, SkillName = "SQL"},
                new Skill { SkillId = 2, SkillName = "C#" },
                new Skill { SkillId = 3, SkillName = "HTML" },
                new Skill { SkillId = 4, SkillName = ".NET" },
                new Skill { SkillId = 5, SkillName = "API" }
                );
        }
        internal object Find(int CandidateId)
        {
            throw new NotImplementedException();
        }
    }
}
