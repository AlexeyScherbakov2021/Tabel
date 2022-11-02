using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class AllRepo
    {
        private static RepositoryOtdel repoOtdel = null;
        private static RepositoryMSSQL<AddWorks> repoAddWorks = null;
        private static RepositoryMSSQL<ModPerson> repoModPerson = null;
        private static RepositoryMSSQL<Transport> repoTransport = null;
        private static RepositoryMSSQL<TransPerson> repoTransPerson = null;
        private static RepositoryMSSQL<Personal> repoPersonal = null;
        private static RepositoryMSSQL<WorkTabel> repoTabel = null;
        private static RepositoryMSSQL<TabelPerson> repoTabelPerson = null;
        private static RepositoryMSSQL<Mod> repoModel = null;
        private static RepositoryMSSQL<Smena> repoSmena = null;
        private static RepositoryMSSQL<SmenaPerson> repoSmenaPersonal = null;
        private static RepositoryCalendar repoCalendar = null;
        private static RepositoryMSSQL<typeDay> repoTypeDay = null;
        private static RepositoryMSSQL<User> repoUser = null;
        private static RepositoryMSSQL<Category> repoCategory = null;


        public static RepositoryOtdel GetRepoOtdel() => repoOtdel ?? new RepositoryOtdel();
        public static RepositoryMSSQL<AddWorks> GetRepoAddWorks() => repoAddWorks ?? new RepositoryMSSQL<AddWorks>();
        public static RepositoryMSSQL<ModPerson> GetRepoModPerson() => repoModPerson ?? new RepositoryMSSQL<ModPerson>();
        public static RepositoryMSSQL<Transport> GetRepoTransport() => repoTransport ?? new RepositoryMSSQL<Transport>();
        public static RepositoryMSSQL<TransPerson> GetRepoTransPerson() => repoTransPerson ?? new RepositoryMSSQL<TransPerson>();
        public static RepositoryMSSQL<Personal> GetRepoPersonal() => repoPersonal ?? new RepositoryMSSQL<Personal>();
        public static RepositoryMSSQL<WorkTabel> GetRepoTabel() => repoTabel ?? new RepositoryMSSQL<WorkTabel>();
        public static RepositoryMSSQL<TabelPerson> GetRepoTabelPerson() => repoTabelPerson ?? new RepositoryMSSQL<TabelPerson>();
        public static RepositoryMSSQL<Mod> GetRepoModel() => repoModel ?? new RepositoryMSSQL<Mod>();
        public static RepositoryMSSQL<Smena> GetRepoSmena() => repoSmena ?? new RepositoryMSSQL<Smena>();
        public static RepositoryMSSQL<SmenaPerson> GetRepoSmenaPerson() => repoSmenaPersonal ?? new RepositoryMSSQL<SmenaPerson>();
        public static RepositoryCalendar GetRepoCalendar() => repoCalendar ?? new RepositoryCalendar();
        public static RepositoryMSSQL<typeDay> GetRepoTypeDay() => repoTypeDay ?? new RepositoryMSSQL<typeDay>();
        public static RepositoryMSSQL<User> GetRepoUser() => repoUser ?? new RepositoryMSSQL<User>();
        public static RepositoryMSSQL<Category> GetRepoCategory() => repoCategory ?? new RepositoryMSSQL<Category>();




    }
}
