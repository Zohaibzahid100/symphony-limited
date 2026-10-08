using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

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

    public virtual DbSet<Branch> Branch { get; set; }

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
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Connection string will be provided by dependency injection in Program.cs
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AboutU>(entity =>
        {
            entity.HasKey(e => e.AboutId).HasName("PK__AboutUs__717FC93C5B188D52");

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.SectionTitle).HasMaxLength(100);
            entity.Property(e => e.SectionsImg)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("sectionsImg");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branch__A1682FC5FE7053BF");

            entity.ToTable("Branch");

            entity.HasIndex(e => e.BranchEmail, "UQ__Branch__9F0C0F993FD6EE6E").IsUnique();

            entity.HasIndex(e => e.BranchEmail, "UQ__Branch__9F0C0F99847A5368").IsUnique();

            entity.HasIndex(e => e.BranchEmail, "UQ__Branch__9F0C0F99B99F5FB2").IsUnique();

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
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A73CAADB7C");

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
            entity.HasKey(e => e.TopicId).HasName("PK__CourseTo__022E0F5DD105945F");

            entity.Property(e => e.TopicName).HasMaxLength(100);

            entity.HasOne(d => d.Track).WithMany(p => p.CourseTopics)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseTop__Track__0A688BB1");
        });

        modelBuilder.Entity<CourseTrack>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("PK__CourseTr__7A74F8E09B95B3F4");

            entity.Property(e => e.CourseFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrackName).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseTracks)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseTra__Cours__0B5CAFEA");
        });

        modelBuilder.Entity<EntranceExam>(entity =>
        {
            entity.HasKey(e => e.EntranceExamId).HasName("PK__Entrance__D7E2593EA2139DD8");

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
                .HasConstraintName("FK__EntranceE__Cours__0C50D423");
        });

        modelBuilder.Entity<EntranceExamApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId);

            entity.HasIndex(e => new { e.StudentId, e.EntranceExamId }).IsUnique();

            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Applied");

            entity.HasOne(d => d.Student).WithMany()
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EntranceExam).WithMany()
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Branch).WithMany()
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Payment).WithMany()
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EntranceMcq>(entity =>
        {
            entity.HasKey(e => e.McqId).HasName("PK__Entrance__E757760059854B1C");

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
                .HasConstraintName("FK__EntranceM__Entra__0D44F85C");

            entity.HasOne(d => d.Topic).WithMany(p => p.EntranceMcqs)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__EntranceM__Topic__0E391C95");
        });

        modelBuilder.Entity<EntranceResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Entrance__976902089841F300");

            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ResultDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ResultStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedTrack).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.AssignedTrackId)
                .HasConstraintName("FK__EntranceR__Assig__0F2D40CE");

            entity.HasOne(d => d.EntranceExam).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.EntranceExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceR__Entra__10216507");

            entity.HasOne(d => d.Student).WithMany(p => p.EntranceResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EntranceR__Stude__11158940");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.FaqId).HasName("PK__FAQs__9C741C4381F7D120");

            entity.ToTable("FAQs");

            entity.Property(e => e.Answer).HasMaxLength(1000);
            entity.Property(e => e.Question).HasMaxLength(300);
        });

        modelBuilder.Entity<FinalExam>(entity =>
        {
            entity.HasKey(e => e.FinalExamId).HasName("PK__FinalExa__B590494A745C75AC");

            entity.Property(e => e.ExamTitle).HasMaxLength(150);
            entity.Property(e => e.TotalMarks).HasDefaultValue(100);

            entity.HasOne(d => d.Course).WithMany(p => p.FinalExams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalExam__Cours__1209AD79");
        });

        modelBuilder.Entity<FinalMcq>(entity =>
        {
            entity.HasKey(e => e.McqId).HasName("PK__FinalMCQ__E757760042D05299");

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
                .HasConstraintName("FK__FinalMCQs__Final__15DA3E5D");

            entity.HasOne(d => d.Topic).WithMany(p => p.FinalMcqs)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__FinalMCQs__Topic__16CE6296");
        });

        modelBuilder.Entity<FinalMcqanswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__FinalMCQ__D4825004B083EF4B");

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
                .HasConstraintName("FK__FinalMCQA__Final__12FDD1B2");

            entity.HasOne(d => d.Mcq).WithMany(p => p.FinalMcqanswers)
                .HasForeignKey(d => d.McqId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQA__McqId__13F1F5EB");

            entity.HasOne(d => d.Student).WithMany(p => p.FinalMcqanswers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalMCQA__Stude__14E61A24");
        });

        modelBuilder.Entity<FinalResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__FinalRes__976902087F17413C");

            entity.Property(e => e.Grade).HasMaxLength(5);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Remarks).HasMaxLength(300);
            entity.Property(e => e.ResultDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ResultStatus).HasMaxLength(10);

            entity.HasOne(d => d.FinalExam).WithMany(p => p.FinalResults)
                .HasForeignKey(d => d.FinalExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalResu__Final__02FC7413");

            entity.HasOne(d => d.Student).WithMany(p => p.FinalResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FinalResu__Stude__02084FDA");
        });

        modelBuilder.Entity<LabSession>(entity =>
        {
            entity.HasKey(e => e.LabSessionId).HasName("PK__LabSessi__9F54F659962DAB7D");

            entity.Property(e => e.SessionFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SessionName).HasMaxLength(100);

            entity.HasOne(d => d.Course).WithMany(p => p.LabSessions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabSessio__Cours__07C12930");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38805DE99A");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentType).HasMaxLength(30);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20).HasDefaultValue("Paid");

            entity.HasOne(d => d.Branch).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Payments_Branches");

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Studen__10566F31");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52B9900C6A163");

            entity.HasIndex(e => e.RollNumber, "UQ__Students__E9F06F169FF2F04F").IsUnique();

            entity.HasIndex(e => e.RollNumber, "UQ__Students__E9F06F16EF84BC46").IsUnique();

            entity.HasIndex(e => e.RollNumber, "UQ__Students__E9F06F16F6B851C3").IsUnique();

            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RollNumber).HasMaxLength(20);

            entity.HasOne(d => d.Branch).WithMany(p => p.Students)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Students__Branch__2334397B");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__UserId__24285DB4");
        });

        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__StudentE__7F68771BBECF59AE");

            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Cours__1B9317B3");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Stude__1C873BEC");

            entity.HasOne(d => d.Track).WithMany(p => p.StudentEnrollments)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentEn__Track__1D7B6025");

            entity.HasOne(d => d.Payment).WithMany()
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StudentLabRegistration>(entity =>
        {
            entity.HasKey(e => e.LabRegistrationId).HasName("PK__StudentL__B1255A7CA2ED95B2");

            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.LabSession).WithMany(p => p.StudentLabRegistrations)
                .HasForeignKey(d => d.LabSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentLa__LabSe__0C85DE4D");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentLabRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentLa__Stude__0B91BA14");

            entity.HasOne(d => d.Payment).WithMany()
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StudentMcqanswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__StudentM__D4825004E9B67B97");

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
                .HasConstraintName("FK__StudentMC__Entra__2057CCD0");

            entity.HasOne(d => d.Mcq).WithMany(p => p.StudentMcqanswers)
                .HasForeignKey(d => d.McqId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentMC__McqId__214BF109");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentMcqanswers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentMC__Stude__22401542");
        });

        modelBuilder.Entity<TrackAssignmentRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__TrackAss__110458E264979F8A");

            entity.Property(e => e.MaxPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MinPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedTrack).WithMany(p => p.TrackAssignmentRules)
                .HasForeignKey(d => d.AssignedTrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackAssi__Assig__251C81ED");

            entity.HasOne(d => d.Course).WithMany(p => p.TrackAssignmentRules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrackAssi__Cours__2610A626");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C08C72AED");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105346F57FAD7").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534BBC1ADAC").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DCF4C387").IsUnique();

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
