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
        public virtual DbSet<Day> days { get; set; }
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

            //modelBuilder.Entity<Otdel>()
            //    .Property(e => e.ot_name)
            //    .IsFixedLength();

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
                .HasMany(e => e.users)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.u_otdel_id);

            //modelBuilder.Entity<Personal>()
            //    .Property(e => e.p_tab_number)
            //    .IsFixedLength();

            //modelBuilder.Entity<Personal>()
            //    .Property(e => e.p_lastname)
            //    .IsFixedLength();

            //modelBuilder.Entity<Personal>()
            //    .Property(e => e.p_name)
            //    .IsFixedLength();

            //modelBuilder.Entity<Personal>()
            //    .Property(e => e.p_midname)
            //    .IsFixedLength();

            //modelBuilder.Entity<Personal>()
            //    .Property(e => e.p_profeesion)
            //    .IsFixedLength();

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.tabels)
                .WithOptional(e => e.personal)
                .HasForeignKey(e => e.t_person_id)
                .WillCascadeOnDelete();

            //modelBuilder.Entity<Tabel>()
            //    .Property(e => e.t_number)
            //    .IsFixedLength();

            //modelBuilder.Entity<Tabel>()
            //    .Property(e => e.t_author)
            //    .IsFixedLength();

            modelBuilder.Entity<WorkTabel>()
                .HasMany(e => e.days)
                .WithOptional(e => e.tabel)
                .HasForeignKey(e => e.d_tabel_id)
                .WillCascadeOnDelete();

            //modelBuilder.Entity<User>()
            //    .Property(e => e.u_login)
            //    .IsFixedLength();

            //modelBuilder.Entity<User>()
            //    .Property(e => e.u_pass)
            //    .IsFixedLength();

            //modelBuilder.Entity<User>()
            //    .Property(e => e.u_fio)
            //    .IsFixedLength();
        }
    }
}
