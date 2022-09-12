using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Tabel.Models2
{
    public partial class BaseModel : DbContext
    {
        public BaseModel()
            : base("name=BaseModel")
        {
        }

        public virtual DbSet<WorkCalendar> calendars { get; set; }
        public virtual DbSet<Category> categories { get; set; }
        public virtual DbSet<Otdel> otdels { get; set; }
        public virtual DbSet<Personal> personals { get; set; }
        public virtual DbSet<Smena> smenas { get; set; }
        public virtual DbSet<SmenaDay> SmenaDays { get; set; }
        public virtual DbSet<SmenaPerson> SmenaPersons { get; set; }
        public virtual DbSet<WorkTabel> tabels { get; set; }
        public virtual DbSet<TabelPerson> tabelPersons { get; set; }
        public virtual DbSet<typeDay> typeDays { get; set; }
        public virtual DbSet<User> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(e => e.cat_tarif)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.category)
                .HasForeignKey(e => e.p_cat_id);

            modelBuilder.Entity<Otdel>()
                .Property(e => e.ot_name)
                .IsUnicode(false);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.otdels1)
                .WithOptional(e => e.otdel1)
                .HasForeignKey(e => e.ot_parent);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.p_otdel_id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.smenas)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.sm_OtdelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.tabels)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.t_otdel_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.users)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.u_otdel_id);

            modelBuilder.Entity<Personal>()
                .Property(e => e.p_tab_number)
                .IsUnicode(false);

            modelBuilder.Entity<Personal>()
                .Property(e => e.p_lastname)
                .IsUnicode(false);

            modelBuilder.Entity<Personal>()
                .Property(e => e.p_name)
                .IsUnicode(false);

            modelBuilder.Entity<Personal>()
                .Property(e => e.p_midname)
                .IsUnicode(false);

            modelBuilder.Entity<Personal>()
                .Property(e => e.p_profession)
                .IsUnicode(false);

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.tabelPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.d_person_id);

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.SmenaPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.sp_PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Smena>()
                .Property(e => e.sm_Number)
                .IsUnicode(false);

            modelBuilder.Entity<Smena>()
                .HasMany(e => e.SmenaPersons)
                .WithRequired(e => e.smena)
                .HasForeignKey(e => e.sp_SmenaId);

            modelBuilder.Entity<SmenaPerson>()
                .HasMany(e => e.SmenaDays)
                .WithOptional(e => e.SmenaPerson)
                .HasForeignKey(e => e.sd_SmenaPersonId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<WorkTabel>()
                .Property(e => e.t_number)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTabel>()
                .Property(e => e.t_author)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTabel>()
                .HasMany(e => e.tabelPersons)
                .WithRequired(e => e.tabel)
                .HasForeignKey(e => e.d_tabel_id);

            modelBuilder.Entity<typeDay>()
                .Property(e => e.t_name)
                .IsUnicode(false);

            modelBuilder.Entity<typeDay>()
                .Property(e => e.t_desc)
                .IsUnicode(false);

            modelBuilder.Entity<typeDay>()
                .HasMany(e => e.tabelPersons)
                .WithOptional(e => e.typeDay)
                .HasForeignKey(e => e.d_type);

            modelBuilder.Entity<User>()
                .Property(e => e.u_login)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.u_pass)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.u_fio)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.smenas)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.sm_UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
