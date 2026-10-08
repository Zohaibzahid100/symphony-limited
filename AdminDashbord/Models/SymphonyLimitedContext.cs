using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AdminDashbord.Models;

public partial class SymphonyLimitedContext : DbContext
{
    public SymphonyLimitedContext()
    {
    }

    public SymphonyLimitedContext(DbContextOptions<SymphonyLimitedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutU> AboutUs { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseTopic> CourseTopics { get; set; }

    public virtual DbSet<CourseTrack> CourseTracks { get; set; }

    public virtual DbSet<EntranceExam> EntranceExams { get; set; }

    public virtual DbSet<EntranceExamApplication> EntranceExamApplications { get; set; }

    public virtual DbSet<EntranceMcq> EntranceMcqs { get; set; }

    public virtual DbSet<EntranceResult> EntranceResults { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<FinalExam> FinalExams { get; set; }

    public virtual DbSet<FinalMcq> FinalMcqs { get; set; }

    public virtual DbSet<FinalMcqanswer> FinalMcqanswers { get; set; }

    public virtual DbSet<FinalResult> FinalResults { get; set; }

    public virtual DbSet<LabSession> LabSessions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentEnrollment> StudentEnrollments { get; set; }

    public virtual DbSet<StudentLabRegistration> StudentLabRegistrations { get; set; }

    public virtual DbSet<StudentMcqanswer> StudentMcqanswers { get; set; }

    public virtual DbSet<TrackAssignmentRule> TrackAssignmentRules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AboutU>(entity =>
        {
            entity.HasKey(e => e.AboutId).HasName("PK__AboutUs__717FC93C2CEBB56E");

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.SectionTitle).HasMaxLength(100);
            entity.Property(e => e.SectionsImg)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("sectionsImg");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branch__A1682FC5E8494E0C");

            entity.ToTable("Branch");

            entity.HasIndex(e => e.BranchEmail, "UQ__Branch__9F0C0F998F93BFA0").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BranchCity).HasMaxLength(100);
            entity.Property(e => e.BranchContact).HasMaxLength(15);
            entity.Property(e => e.BranchEmail).HasMaxLength(100);
            entity.Property(e => e.BranchImg)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchName).HasMaxLength(100);
            entity.Property(e => e.BranchStatus)
                .HasMaxLength(10)
                .HasDefaultValue("open");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A7069B2D6C");

            entity.Property(e => e.CourseImg)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.CourseStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<CourseTopic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__CourseTo__022E0F5D219FA77B");

            entity.Property(e => e.TopicName).HasMaxLength(100);

            entity.HasOne(d => d.Track).WithMany(p => p.CourseTopics)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseTop__Track__66603565");
        });

        modelBuilder.Entity<CourseTrack>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("PK__CourseTr__7A74F8E09AFA19E3");

            entity.Property(e => e.CourseFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrackName).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseTracks)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseTra__Cours__6754599E");
        });

        modelBuilder.Entity<EntranceExam>(entity =>
        {
            entity.HasKey(e => e.EntranceExamId).HasName("PK__Entrance__D7E2593E42867253");

            entity.Property(e => e.ExamFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExamStatus)
                .HasMaxLength(15)
                .HasDefaultValue("Upcoming");
            entity.Property(e => e.ExamTitle).HasMaxLength(200);
            entity.Property(e => e.TotalMarks).HasDefaultValue(20);

            entity.HasOne(d => d.Branch).WithMany(p => p.EntranceExams)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntranceExams_Branches");

            entity.HasOne(d => d.Course).WithMany(p => p.EntranceExams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceE__Cours__6C190EBB");
        });

        modelBuilder.Entity<EntranceExamApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Entrance__C93A4C99615DAA62");

            entity.HasIndex(e => new { e.StudentId, e.EntranceExamId }, "UQ_Application_Student_Exam").IsUnique();

            entity.Property(e => e.ApplyDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Applied");

            entity.HasOne(d => d.Branch).WithMany(p => p.EntranceExamApplications)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceE__Branc__68487DD7");

            entity.HasOne(d => d.EntranceExam).WithMany(p => p.EntranceExamApplications)
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceE__Entra__693CA210");

            entity.HasOne(d => d.Payment).WithMany(p => p.EntranceExamApplications)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__EntranceE__Payme__6A30C649");

            entity.HasOne(d => d.Student).WithMany(p => p.EntranceExamApplications)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceE__Stude__6B24EA82");
        });

        modelBuilder.Entity<EntranceMcq>(entity =>
        {
            entity.HasKey(e => e.McqId).HasName("PK__Entrance__E757760025161801");

            entity.ToTable("EntranceMCQs");

            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Mcqmarks)
                .HasDefaultValue(1)
                .HasColumnName("MCQMarks");
            entity.Property(e => e.OptionA).HasMaxLength(200);
            entity.Property(e => e.OptionB).HasMaxLength(200);
            entity.Property(e => e.OptionC).HasMaxLength(200);
            entity.Property(e => e.OptionD).HasMaxLength(200);
            entity.Property(e => e.QuestionText).HasMaxLength(500);

            entity.HasOne(d => d.EntranceExam).WithMany(p => p.EntranceMcqs)
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceM__Entra__6E01572D");

            entity.HasOne(d => d.Topic).WithMany(p => p.EntranceMcqs)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__EntranceM__Topic__6EF57B66");
        });

        modelBuilder.Entity<EntranceResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Entrance__97690208BF984D18");

            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ResultDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ResultStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedTrack).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.AssignedTrackId)
                .HasConstraintName("FK__EntranceR__Assig__6FE99F9F");

            entity.HasOne(d => d.EntranceExam).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceR__Entra__70DDC3D8");

            entity.HasOne(d => d.Student).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceR__Stude__71D1E811");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.FaqId).HasName("PK__FAQs__9C741C4337121C3E");

            entity.ToTable("FAQs");

            entity.Property(e => e.Answer).HasMaxLength(1000);
            entity.Property(e => e.Question).HasMaxLength(300);
        });

        modelBuilder.Entity<FinalExam>(entity =>
        {
            entity.HasKey(e => e.FinalExamId).HasName("PK__FinalExa__B590494A31D56C72");

            entity.Property(e => e.ExamTitle).HasMaxLength(150);
            entity.Property(e => e.TotalMarks).HasDefaultValue(100);

            entity.HasOne(d => d.Course).WithMany(p => p.FinalExams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalExam__Cours__72C60C4A");
        });

        modelBuilder.Entity<FinalMcq>(entity =>
        {
            entity.HasKey(e => e.McqId).HasName("PK__FinalMCQ__E7577600BBD7D27A");

            entity.ToTable("FinalMCQs");

            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.McqMarks).HasDefaultValue(1);
            entity.Property(e => e.OptionA).HasMaxLength(200);
            entity.Property(e => e.OptionB).HasMaxLength(200);
            entity.Property(e => e.OptionC).HasMaxLength(200);
            entity.Property(e => e.OptionD).HasMaxLength(200);
            entity.Property(e => e.QuestionText).HasMaxLength(500);

            entity.HasOne(d => d.FinalExam).WithMany(p => p.FinalMcqs)
                .HasForeignKey(d => d.FinalExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQs__Final__76969D2E");

            entity.HasOne(d => d.Topic).WithMany(p => p.FinalMcqs)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__FinalMCQs__Topic__778AC167");
        });

        modelBuilder.Entity<FinalMcqanswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__FinalMCQ__D4825004E9274669");

            entity.ToTable("FinalMCQAnswers");

            entity.Property(e => e.SelectedOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FinalExam).WithMany(p => p.FinalMcqanswers)
                .HasForeignKey(d => d.FinalExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQA__Final__73BA3083");

            entity.HasOne(d => d.Mcq).WithMany(p => p.FinalMcqanswers)
                .HasForeignKey(d => d.McqId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQA__McqId__74AE54BC");

            entity.HasOne(d => d.Student).WithMany(p => p.FinalMcqanswers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQA__Stude__75A278F5");
        });

        modelBuilder.Entity<FinalResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__FinalRes__97690208C20F3DEE");

            entity.Property(e => e.Grade).HasMaxLength(5);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Remarks).HasMaxLength(300);
            entity.Property(e => e.ResultDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ResultStatus).HasMaxLength(10);

            entity.HasOne(d => d.FinalExam).WithMany(p => p.FinalResults)
                .HasForeignKey(d => d.FinalExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalResu__Final__787EE5A0");

            entity.HasOne(d => d.Student).WithMany(p => p.FinalResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalResu__Stude__797309D9");
        });

        modelBuilder.Entity<LabSession>(entity =>
        {
            entity.HasKey(e => e.LabSessionId).HasName("PK__LabSessi__9F54F659D8BE0D57");

            entity.Property(e => e.SessionFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SessionName).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.LabSessions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabSessio__Cours__7A672E12");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A389341F9E9");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasDefaultValue("paid");
            entity.Property(e => e.PaymentType).HasMaxLength(30);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Payments_Branches");

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Studen__7B5B524B");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52B99E8738E14");

            entity.HasIndex(e => e.RollNumber, "UQ_Students_RollNumber").IsUnique();

            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RollNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Students)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Students__Branch__06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__UserId__07C12930");
        });

        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__StudentE__7F68771B96AD0EAE");

            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Cours__7D439ABD");

            entity.HasOne(d => d.Payment).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__StudentEn__Payme__7E37BEF6");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Stude__7F2BE32F");

            entity.HasOne(d => d.Track).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Track__00200768");
        });

        modelBuilder.Entity<StudentLabRegistration>(entity =>
        {
            entity.HasKey(e => e.LabRegistrationId).HasName("PK__StudentL__B1255A7CC9AC295F");

            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LabSession).WithMany(p => p.StudentLabRegistrations)
                .HasForeignKey(d => d.LabSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentLa__LabSe__01142BA1");

            entity.HasOne(d => d.Payment).WithMany(p => p.StudentLabRegistrations)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__StudentLa__Payme__02084FDA");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentLabRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentLa__Stude__02FC7413");
        });

        modelBuilder.Entity<StudentMcqanswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__StudentM__D482500493CB2448");

            entity.ToTable("StudentMCQAnswers");

            entity.Property(e => e.SelectedOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.EntranceExam).WithMany(p => p.StudentMcqanswers)
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentMC__Entra__03F0984C");

            entity.HasOne(d => d.Mcq).WithMany(p => p.StudentMcqanswers)
                .HasForeignKey(d => d.McqId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentMC__McqId__04E4BC85");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentMcqanswers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentMC__Stude__05D8E0BE");
        });

        modelBuilder.Entity<TrackAssignmentRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__TrackAss__110458E2FDA1FB42");

            entity.Property(e => e.MaxPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MinPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedTrack).WithMany(p => p.TrackAssignmentRules)
                .HasForeignKey(d => d.AssignedTrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackAssi__Assig__08B54D69");

            entity.HasOne(d => d.Course).WithMany(p => p.TrackAssignmentRules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackAssi__Cours__09A971A2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9841F16F");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053448B857FA").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasDefaultValue("student");
            entity.Property(e => e.UserImg)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
