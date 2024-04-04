using Microsoft.EntityFrameworkCore;

namespace Class.Models
{
    public partial class ClassContext : DbContext
    {
        public ClassContext(DbContextOptions<ClassContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=M\\SQLEXPRESS;Initial Catalog=Class;Persist Security Info=True;User ID=sa;Password=123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.FcmToken).HasMaxLength(255);

                entity.Property(e => e.Salt)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                entity.Property(e => e.CourseID).HasColumnName("CourseID");

                entity.Property(e => e.CourseName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.AdminID)
                    .HasConstraintName("FK_Courses_Users");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");

                entity.HasKey(e => e.EnrollmentID);

                entity.Property(e => e.EnrollmentID).HasColumnName("EnrollmentID");

                entity.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(d => d.CourseID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollments_Courses");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollments_Users");
            });


            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("Assignments");

                entity.HasKey(e => e.AssignmentID);

                entity.Property(e => e.AssignmentID).HasColumnName("AssignmentID");

                entity.Property(e => e.CourseID).HasColumnName("CourseID"); 

                entity.Property(e => e.AssignmentName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

                entity.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(d => d.CourseID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Assignments_Courses");

            });


            modelBuilder.Entity<Submission>(entity =>
            {
                entity.ToTable("Submissions");

                entity.Property(e => e.SubmissionID).HasColumnName("SubmissionID");

                entity.Property(e => e.AssignmentID).HasColumnName("AssignmentID");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.SubmissionDate).HasColumnType("datetime");

                entity.Property(e => e.FilePath)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasOne<Assignment>()
                    .WithMany()
                    .HasForeignKey(d => d.AssignmentID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Submissions_Assignments");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Submissions_Users");


            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.ToTable("Tests");

                entity.Property(e => e.TestID).HasColumnName("TestID");

                entity.Property(e => e.AssignmentID).HasColumnName("AssignmentID");

                entity.Property(e => e.Question).HasColumnType("nvarchar(max)").IsRequired();

                entity.Property(e => e.CorrectAnswer).HasColumnType("nvarchar(max)").IsRequired();

                entity.HasOne<Assignment>()
                    .WithMany()
                    .HasForeignKey(d => d.AssignmentID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tests_Assignments");


            });

            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.ToTable("TestResults");

                entity.Property(e => e.TestResultID).HasColumnName("TestResultID");

                entity.Property(e => e.TestID).HasColumnName("TestID");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.AssignmentID).HasColumnName("AssignmentID");

                entity.Property(e => e.Score).IsRequired();

                entity.HasOne<Test>()
                    .WithMany()
                    .HasForeignKey(d => d.TestID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestResults_Tests");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestResults_Users");

                entity.HasOne<Assignment>()
                    .WithMany()
                    .HasForeignKey(d => d.AssignmentID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestResults_Assignments");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(e => e.CommentID);

                entity.Property(e => e.CommentID).HasColumnName("CommentID");

                entity.Property(e => e.SenderID).HasColumnName("SenderID");

                entity.Property(e => e.RecipientID).HasColumnName("RecipientID");

                entity.Property(e => e.AssignmentID).HasColumnName("AssignmentID");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("TEXT");

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.SenderID)
                    .HasConstraintName("FK_Comments_Users_SenderID");

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.RecipientID)
                    .HasConstraintName("FK_Comments_Users_RecipientID");

                entity.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.AssignmentID)
                   .HasConstraintName("FK_Comments_Users_AssignmentID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
