using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    public class RepositoryOtdel : RepositoryMSSQL<Otdel>
    {
        public override IQueryable<Otdel> Items => base.Items.Where(it => it.parent == null);//.Include(it => it.subOtdels);

        public RepositoryOtdel(BaseModel db = null) : base(db)
        {

        }


        //public RepositoryOtdel(BaseModel Db = null) : base(Db)
        //{

        //}

        //public void RemoveSubOtdel(T item, bool Autosave = false)
        //{
        //    if (item is null || item.id <= 0)
        //        return;

        //    db.Entry(item).State = EntityState.Deleted;
        //    if (Autosave)
        //        Save();

        //}


        //public Otdel AddOtdel(Otdel item, Otdel parent = null)
        //{
        //    if (item is null) throw new ArgumentNullException(nameof(item));
        //    db.Entry(item).State = EntityState.Added;
        //    db.SaveChanges();
        //    return item;
        //}

        public ICollection<Otdel> GetTreeOtdelsNoTracking(ICollection<Otdel> ListUserOtdels, int level)
        {
            ICollection<Otdel> ListOtdels = new Collection<Otdel>();

            // получение корневых отделов
            IEnumerable<Otdel> ListRootOtdels = Items.AsNoTracking().Where(it => it.ot_itr <= level).OrderBy(o => o.ot_sort);

            // удаление ненужных отделов
            foreach (var item in ListRootOtdels)
            {
                if (ListUserOtdels.Any(it => it.id == item.id))
                    ListOtdels.Add(item);
            }

            // добавлеие подотделов, которых еще не добавлены
            RepositoryMSSQL<Otdel> repo1 = new RepositoryMSSQL<Otdel>(db); // AllRepo.GetRepoAllOtdels();
            IEnumerable<Otdel> AllOtdels = repo1.Items.AsNoTracking().Where(it => it.parent != null && it.ot_itr <= level).OrderBy(o => o.ot_sort);
            CombaineOtdels(ListUserOtdels, ListOtdels, AllOtdels);
            return ListOtdels;

        }



        public ICollection<Otdel> GetTreeOtdels(ICollection<Otdel> ListUserOtdels, int level)
        {
            ICollection<Otdel> ListOtdels = new Collection<Otdel>();

            // получение корневых отделов
            IEnumerable<Otdel> ListRootOtdels = Items.Where(it => it.ot_itr <= level).OrderBy(o => o.ot_sort);

            // удаление ненужных отделов
            foreach (var item in ListRootOtdels)
            {
                if (ListUserOtdels.Any(it => it.id == item.id))
                    ListOtdels.Add(item);
            }

            // добавлеие подотделов, которых еще не добавлены
            RepositoryMSSQL<Otdel> repo1 = new RepositoryMSSQL<Otdel>(db);
            IEnumerable<Otdel> AllOtdels = repo1.Items.Where(it => it.parent != null && it.ot_itr <= level).OrderBy(o => o.ot_sort);
            CombaineOtdels(ListUserOtdels, ListOtdels, AllOtdels);

            return ListOtdels;
        }


        private void CombaineOtdels(ICollection<Otdel> ListUserOtdels, ICollection<Otdel> ListOtdels, IEnumerable<Otdel> AllOtdels)
        {
            foreach (var item in AllOtdels)
            {

                if (ListUserOtdels.Any(it => it.id == item.id))
                {
                    bool IsExist = false;

                    foreach (var sub in ListOtdels)
                    {
                        if (sub.subOtdels.Any(it => it.id == item.id))
                        {
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)
                        ListOtdels.Add(item);
                }
            }
        }
    }

}
