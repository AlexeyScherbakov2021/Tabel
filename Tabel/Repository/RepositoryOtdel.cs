using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryOtdel : RepositoryMSSQL<Otdel>
    {
        public override IQueryable<Otdel> Items => base.Items.Where(it => it.parent == null);//.Include(it => it.subOtdels);

        public Otdel AddOtdel(Otdel item, Otdel parent = null)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            db.Entry(item).State = EntityState.Added;
            db.SaveChanges();
            return item;
        }

        public ICollection<Otdel> GetTreeOtdels(ICollection<Otdel> ListUserOtdels)
        {
            List<Otdel> ListOtdels = new List<Otdel>();

            // получение корневых отделов
            ICollection<Otdel> ListRootOtdels = Items.AsNoTracking().ToList();

            // удаление ненужных отделов
            foreach(var item in ListRootOtdels)
            {
                if (ListUserOtdels.Any(it => it.id == item.id))
                    ListOtdels.Add(item);
            }

            // добавлеие подотделов, которых еще не добавлены
            RepositoryMSSQL<Otdel> repo1 = new RepositoryMSSQL<Otdel>();
            ICollection<Otdel> AllOtdels = repo1.Items.AsNoTracking().Where(it => it.parent != null).ToList();
            foreach(var item in AllOtdels)
            {

                if(ListUserOtdels.Any(it => it.id == item.id ))
                {
                    bool IsExist = false;

                    foreach (var sub in ListOtdels)
                    {
                        if(sub.subOtdels.Any(it => it.id == item.id))
                        {
                            IsExist = true;
                            break;
                        }
                    }
                    if(!IsExist)
                        ListOtdels.Add(item);
                }
            }

            return ListOtdels;
        }

    }

}
