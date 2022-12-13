using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Tabel.Models;

namespace Tabel.Models
{
    public partial class BaseModel : DbContext
    {
        //private BaseModel BaseDB = null;

        //public BaseModel GetDB()
        //{
        //    if (BaseDB is null)
        //        CreateDB();

        //    return BaseDB;
        //}


        public static BaseModel CreateDB()
        {
            string ConnectString;

#if DEBUG
            ConnectString = ConfigurationManager.ConnectionStrings["BaseModelLocal"].ConnectionString;
#endif

#if RELEASE
            ConnectString = ConfigurationManager.ConnectionStrings["BaseModel"].ConnectionString;
            ConnectString += ";user id=fpLoginName;password=ctcnhjt,s";
#endif

#if DEMO
            ConnectString = ConfigurationManager.ConnectionStrings["BaseModelDemo"].ConnectionString;
            ConnectString += ";user id=fpLoginName;password=ctcnhjt,s";
#endif



            BaseModel db = new BaseModel(ConnectString);

            return db;

        }

        public BaseModel(string cs) : base(cs)   //: base("name=BaseModel")
        {
        }

        public virtual DbSet<WorkCalendar> calendars { get; set; }
        public virtual DbSet<Category> categories { get; set; }
        public virtual DbSet<Mod> Mods { get; set; }
        public virtual DbSet<ModPerson> ModPersons { get; set; }
        public virtual DbSet<Otdel> otdels { get; set; }
        public virtual DbSet<Personal> personals { get; set; }
        public virtual DbSet<Smena> smenas { get; set; }
        public virtual DbSet<SmenaDay> SmenaDays { get; set; }
        public virtual DbSet<SmenaPerson> SmenaPersons { get; set; }
        public virtual DbSet<WorkTabel> tabels { get; set; }
        public virtual DbSet<TabelDay> TabelDays { get; set; }
        public virtual DbSet<TabelPerson> tabelPersons { get; set; }
        public virtual DbSet<TransDay> TransDays { get; set; }
        public virtual DbSet<TransPerson> TransPersons { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<typeDay> typeDays { get; set; }
        public virtual DbSet<User> users { get; set; }
        public virtual DbSet<AddWorks> addWorks { get; set; }
        public virtual DbSet<GeneralCharges> GeneralCharges { get; set; }
        public virtual DbSet<GenChargMonth> GenChargMonth { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddWorks>()
                .Property(e => e.aw_Name)
                .IsUnicode(false);


            modelBuilder.Entity<GeneralCharges>()
                .HasMany(e => e.ListGenMonth)
                .WithRequired(e => e.GenCarhge)
                .HasForeignKey(e => e.gm_GenId);

            //modelBuilder.Entity<Otdel>()
            //    .HasMany(e => e.ModOtdelSumFPs)
            //    .WithRequired(e => e.otdel)
            //    .HasForeignKey(e => e.mo_otdel_id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Mod>()
            //    .HasMany(e => e.ModOtdelSumFPs)
            //    .WithRequired(e => e.Mod)
            //    .HasForeignKey(e => e.mo_mod_id);


            modelBuilder.Entity<Category>()
                .HasMany(e => e.personals)
                .WithOptional(e => e.category)
                .HasForeignKey(e => e.p_cat_id);

            modelBuilder.Entity<Mod>()
                .HasMany(e => e.ModPersons)
                .WithOptional(e => e.Mod)
                .HasForeignKey(e => e.md_modId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Otdel>()
                .Property(e => e.ot_name)
                .IsUnicode(false);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.mods)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.m_otdelId)
                .WillCascadeOnDelete(false);

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
                .HasMany(e => e.Transports)
                .WithRequired(e => e.otdel)
                .HasForeignKey(e => e.tr_OtdelId);

            //modelBuilder.Entity<Otdel>()
            //    .HasMany(e => e.users)
            //    .WithOptional(e => e.otdel)
            //    .HasForeignKey(e => e.u_otdel_id);

            modelBuilder.Entity<Otdel>()
                .HasMany(e => e.users1)
                .WithMany(e => e.otdels)
                .Map(m => m.ToTable("UserOtdels").MapLeftKey("Otdel_ID").MapRightKey("User_ID"));

            modelBuilder.Entity<ModPerson>()
                .HasMany(e => e.ListAddWorks)
                .WithMany(e => e.ListModPerson)
                .Map(m => m.ToTable("ModPersonAddWorks").MapLeftKey("mp_Id").MapRightKey("aw_Id"));


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
                .HasMany(e => e.ModPersons)
                .WithRequired(e => e.person)
                .HasForeignKey(e => e.md_personalId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.tabelPersons)
                .WithRequired(e => e.person)
                .HasForeignKey(e => e.tp_person_id);

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.SmenaPersons)
                .WithRequired(e => e.personal)
                .HasForeignKey(e => e.sp_PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personal>()
                .HasMany(e => e.TransPersons)
                .WithRequired(e => e.person)
                .HasForeignKey(e => e.tp_PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Smena>()
                .Property(e => e.sm_Number)
                .IsUnicode(false);

            modelBuilder.Entity<Smena>()
                .HasMany(e => e.SmenaPerson)
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
                .HasForeignKey(e => e.tp_tabel_id);

            modelBuilder.Entity<TabelDay>()
                .Property(e => e.td_Hours)
                .HasPrecision(18, 1);

            modelBuilder.Entity<TabelPerson>()
                .HasMany(e => e.TabelDays)
                .WithRequired(e => e.TabelPerson)
                .HasForeignKey(e => e.td_TabelPersonId);

            modelBuilder.Entity<TransPerson>()
                .HasMany(e => e.TransDays)
                .WithRequired(e => e.TransPerson)
                .HasForeignKey(e => e.td_TransPersonId);

            modelBuilder.Entity<Transport>()
                .Property(e => e.tr_Number)
                .IsUnicode(false);

            modelBuilder.Entity<Transport>()
                .HasMany(e => e.TransportPerson)
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
                .HasMany(e => e.mods)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.m_author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.smenas)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.sm_UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.tabels)
                .WithOptional(e => e.Author)
                .HasForeignKey(e => e.t_author_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.transport)
                .WithRequired(e => e.user)
                .HasForeignKey(e => e.tr_UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
