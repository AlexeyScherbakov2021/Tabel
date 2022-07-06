using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Tabel.Models
{
    public partial class BaseModel : DbContext
    {
        public BaseModel()
            : base("name=BaseModel")
        {
            
        }

        public virtual DbSet<Category> categories { get; set; }
        public virtual DbSet<TabelPerson> days { get; set; }
        public virtual DbSet<Otdel> otdels { get; set; }
        public virtual DbSet<Personal> personals { get; set; }
        public virtual DbSet<WorkTabel> tabels { get; set; }
        public virtual DbSet<User> users { get; set; }
        public virtual DbSet<WorkCalendar> calendars { get; set; }

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
                .HasMany(e => e.subOtdels)
                .WithOptional(e => e.parent)
                .HasForeignKey(e => e.ot_parent);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.p_otdel_id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.tabels)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.t_otdel_id);

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
                .HasMany(e => e.TabelPersons)
                .WithRequired(e => e.person)
                .HasForeignKey(e => e.d_person_id);

            modelBuilder.Entity<typeDay>()
                .HasMany(e => e.TabelPersons)
                .WithRequired(e => e.type_day)
                .HasForeignKey(e => e.d_type);


            modelBuilder.Entity<WorkTabel>()
                .Property(e => e.t_number)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTabel>()
                .Property(e => e.t_author)
                .IsUnicode(false);

            modelBuilder.Entity<WorkTabel>()
                .HasMany(e => e.TabelPersons)
                .WithOptional(e => e.tabel)
                .HasForeignKey(e => e.d_tabel_id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<User>()
                .Property(e => e.u_login)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.u_pass)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.u_fio)
                .IsUnicode(false);
        }
    }
}
