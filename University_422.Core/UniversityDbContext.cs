using Microsoft.EntityFrameworkCore;

namespace University_422.Core;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext()
    {
    }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> TableDepartments { get; set; }

    public virtual DbSet<DepartmentsSubject> TableDepartmentsSubjects { get; set; }

    public virtual DbSet<Faculty> TableFaculties { get; set; }

    public virtual DbSet<FirstName> TableFirstNames { get; set; }

    public virtual DbSet<Group> TableGroups { get; set; }

    public virtual DbSet<LastName> TableLastNames { get; set; }

    public virtual DbSet<Patronymic> TablePatronymics { get; set; }

    public virtual DbSet<Person> TablePersons { get; set; }

    public virtual DbSet<Student> TableStudents { get; set; }

    public virtual DbSet<Subject> TableSubjects { get; set; }

    public virtual DbSet<VDepartment> ViewDepartments { get; set; }

    public virtual DbSet<VDepartmentsSubject> ViewDepartmentsSubjects { get; set; }

    public virtual DbSet<VGroup> ViewGroups { get; set; }

    public virtual DbSet<VPerson> ViewPersons { get; set; }

    public virtual DbSet<VStudent> ViewStudents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql("Server=127.0.0.1;Port=5432;Database=university_db;User Id=postgres;Password=1234;Search Path=test;",
                o => o.MapEnum<TypeStatus>("type_status"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("test", "type_status", ["ЗАЧИСЛЕН", "АКАДЕМИЧЕСКИЙ ОТПУСК", "ОТЧИСЛЕН"]);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_departments_pkey");

            entity.ToTable("table_departments", "test");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_departments_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Faculty).WithMany(p => p.TableDepartments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_departments_faculty_id_fkey");
        });

        modelBuilder.Entity<DepartmentsSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_departments_subjects_pkey");

            entity.ToTable("table_departments_subjects", "test");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_departments_subjects_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Department).WithMany(p => p.TableDepartmentsSubjects)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_departments_subjects_department_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.TableDepartmentsSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_departments_subjects_subject_id_fkey");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_faculties_pkey");

            entity.ToTable("table_faculties", "test");

            entity.HasIndex(e => e.Title, "table_faculties_title_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_faculties_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<FirstName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_first_names_pkey");

            entity.ToTable("table_first_names", "test");

            entity.HasIndex(e => e.Name, "table_first_names_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_first_names_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_groups_pkey");

            entity.ToTable("table_groups", "test");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_groups_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.DateClosing).HasColumnName("date_closing");
            entity.Property(e => e.DateOpening).HasColumnName("date_opening");
            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Faculty).WithMany(p => p.TableGroups)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_groups_faculty_id_fkey");
        });

        modelBuilder.Entity<LastName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_last_names_pkey");

            entity.ToTable("table_last_names", "test");

            entity.HasIndex(e => e.Name, "table_last_names_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_last_names_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Patronymic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_patronymics_pkey");

            entity.ToTable("table_patronymics", "test");

            entity.HasIndex(e => e.Name, "table_patronymics_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_patronymics_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_persons_pkey");

            entity.ToTable("table_persons", "test");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_persons_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstNameId).HasColumnName("first_name_id");
            entity.Property(e => e.LastNameId).HasColumnName("last_name_id");
            entity.Property(e => e.PatronymicId).HasColumnName("patronymic_id");

            entity.HasOne(d => d.FirstName).WithMany(p => p.TablePeople)
                .HasForeignKey(d => d.FirstNameId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_persons_first_name_id_fkey");

            entity.HasOne(d => d.LastName).WithMany(p => p.TablePeople)
                .HasForeignKey(d => d.LastNameId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_persons_last_name_id_fkey");

            entity.HasOne(d => d.Patronymic).WithMany(p => p.TablePeople)
                .HasForeignKey(d => d.PatronymicId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_persons_patronymic_id_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_students_pkey");

            entity.ToTable("table_students", "test");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_students_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DegreeDate).HasColumnName("degree_date");
            entity.Property(e => e.EnrollmentDate).HasColumnName("enrollment_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Group).WithMany(p => p.TableStudents)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_students_group_id_fkey");

            entity.HasOne(d => d.Person).WithMany(p => p.TableStudents)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("table_students_person_id_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_subjects_pkey");

            entity.ToTable("table_subjects", "test");

            entity.HasIndex(e => e.Title, "table_subjects_title_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('table_subjects_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<VDepartment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_departments", "test");

            entity.Property(e => e.Faculty).HasColumnName("faculty");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<VDepartmentsSubject>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_departments_subjects", "test");

            entity.Property(e => e.Depaertment).HasColumnName("depaertment");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Subject).HasColumnName("subject");
        });

        modelBuilder.Entity<VGroup>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_groups", "test");

            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.DateClosing).HasColumnName("date_closing");
            entity.Property(e => e.DateOpening).HasColumnName("date_opening");
            entity.Property(e => e.Faculty).HasColumnName("faculty");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<VPerson>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_persons", "test");

            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
        });

        modelBuilder.Entity<VStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_students", "test");

            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FacultyName).HasColumnName("faculty_name");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.GroupName).HasColumnName("group_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
