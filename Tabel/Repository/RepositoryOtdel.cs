using System;
using System.Collections.Generic;
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
    }

}
