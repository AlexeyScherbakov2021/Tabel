using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Tabel.Models2
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<AddWork> AddWorks { get; set; }
        public virtual DbSet<calendar> calendars { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<Mod> Mods { get; set; }
        public virtual DbSet<ModPerson> ModPersons { get; set; }
        public virtual DbSet<otdel> otdels { get; set; }
        public virtual DbSet<personal> personals { get; set; }
        public virtual DbSet<smena> smenas { get; set; }
        public virtual DbSet<SmenaDay> SmenaDays { get; set; }
        public virtual DbSet<SmenaPerson> SmenaPersons { get; set; }
        public virtual DbSet<tabel> tabels { get; set; }
        public virtual DbSet<TabelDay> TabelDays { get; set; }
        public virtual DbSet<tabelPerson> tabelPersons { get; set; }
        public virtual DbSet<TransDay> TransDays { get; set; }
        public virtual DbSet<TransPerson> TransPersons { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<typeDay> typeDays { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<ModOtdelSumFP> ModOtdelSumFPs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddWork>()
                .Property(e => e.aw_Name)
                .IsUnicode(false);

            modelBuilder.Entity<AddWork>()
                .HasMany(e => e.ModPersons)
                .WithMany(e => e.AddWorks)
                .Map(m => m.ToTable("ModPersonAddWorks").MapLeftKey("aw_id").MapRightKey("mp_Id"));

            modelBuilder.Entity<category>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.category)
                .HasForeignKey(e => e.p_cat_id);

            modelBuilder.Entity<Mod>()
                .HasMany(e => e.ModOtdelSumFPs)
                .WithRequired(e => e.Mod)
                .HasForeignKey(e => e.mo_mod_id);

            modelBuilder.Entity<Mod>()
                .HasMany(e => e.ModPersons)
                .WithOptional(e => e.Mod)
                .HasForeignKey(e => e.md_modId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<otdel>()
                .Property(e => e.ot_name)
                .IsUnicode(false);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.Mods)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.m_otdelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.ModOtdelSumFPs)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.mo_otdel_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.otdels1)
                .WithOptional(e => e.otdel1)
                .HasForeignKey(e => e.ot_parent);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.p_otdel_id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.smenas)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.sm_OtdelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.tabels)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.t_otdel_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.Transports)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.tr_OtdelId);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.users)
                .WithOptional(e => e.otdel)
                .HasForeignKey(e => e.u_otdel_id);

            modelBuilder.Entity<otdel>()
                .HasMany(e => e.users1)
                .WithMany(e => e.otdels)
                .Map(m => m.ToTable("UserOtdels").MapLeftKey("Otdel_ID").MapRightKey("User_ID"));

            modelBuilder.Entity<personal>()
                .Property(e => e.p_tab_number)
                .IsUnicode(false);

            modelBuilder.Entity<personal>()
                .Property(e => e.p_lastname)
                .IsUnicode(false);

            modelBuilder.Entity<personal>()
                .Property(e => e.p_name)
                .IsUnicode(false);

            modelBuilder.Entity<personal>()
                .Property(e => e.p_midname)
                .IsUnicode(false);

            modelBuilder.Entity<personal>()
                .Property(e => e.p_profession)
                .IsUnicode(false);

            modelBuilder.Entity<personal>()
                .HasMany(e => e.ModPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.md_personalId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<personal>()
                .HasMany(e => e.tabelPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.tp_person_id);

            modelBuilder.Entity<personal>()
                .HasMany(e => e.SmenaPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.sp_PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<personal>()
                .HasMany(e => e.TransPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.tp_PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<smena>()
                .Property(e => e.sm_Number)
                .IsUnicode(false);

            modelBuilder.Entity<smena>()
                .HasMany(e => e.SmenaPersons)
                .WithRequired(e => e.smena)
                .HasForeignKey(e => e.sp_SmenaId);

            modelBuilder.Entity<SmenaPerson>()
                .HasMany(e => e.SmenaDays)
                .WithOptional(e => e.SmenaPerson)
                .HasForeignKey(e => e.sd_SmenaPersonId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tabel>()
                .Property(e => e.t_number)
                .IsUnicode(false);

            modelBuilder.Entity<tabel>()
                .Property(e => e.t_author)
                .IsUnicode(false);

            modelBuilder.Entity<tabel>()
                .HasMany(e => e.tabelPersons)
                .WithRequired(e => e.tabel)
                .HasForeignKey(e => e.tp_tabel_id);

            modelBuilder.Entity<TabelDay>()
                .Property(e => e.td_Hours)
                .HasPrecision(18, 1);

            modelBuilder.Entity<tabelPerson>()
                .HasMany(e => e.TabelDays)
                .WithRequired(e => e.tabelPerson)
                .HasForeignKey(e => e.td_TabelPersonId);

            modelBuilder.Entity<TransPerson>()
                .HasMany(e => e.TransDays)
                .WithRequired(e => e.TransPerson)
                .HasForeignKey(e => e.td_TransPersonId);

            modelBuilder.Entity<Transport>()
                .Property(e => e.tr_Number)
                .IsUnicode(false);

            modelBuilder.Entity<Transport>()
                .HasMany(e => e.TransPersons)
                .WithRequired(e => e.Transport)
                .HasForeignKey(e => e.tp_TranspId);

            modelBuilder.Entity<typeDay>()
                .Property(e => e.t_name)
                .IsUnicode(false);

            modelBuilder.Entity<typeDay>()
                .Property(e => e.t_desc)
                .IsUnicode(false);

            modelBuilder.Entity<typeDay>()
                .HasMany(e => e.TabelDays)
                .WithRequired(e => e.typeDay)
                .HasForeignKey(e => e.td_KindId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .Property(e => e.u_login)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.u_pass)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.u_fio)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.Mods)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.m_author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.smenas)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.sm_UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.tabels)
                .WithOptional(e => e.user)
                .HasForeignKey(e => e.t_author_id);

            modelBuilder.Entity<user>()
                .HasMany(e => e.Transports)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.tr_UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
