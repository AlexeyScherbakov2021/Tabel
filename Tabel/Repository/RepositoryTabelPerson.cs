using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models2;

namespace Tabel.Repository
{
    //internal class RepositoryTabelPerson : RepositoryMSSQL<TabelPerson>
    //{
    //    public bool ReamoveForPersonMonth(int IdTabel, int IdPerson, bool Autosave = false)
    //    {
    //        if (IdTabel < 1 || IdPerson < 1)
    //            return false;

    //        IQueryable<int> ListIdRemove = Items.Where(it => it.d_person_id == IdPerson 
    //                && it.d_tabel_id == IdTabel).Select(it => it.id);


    //        foreach(int n in ListIdRemove)
    //        {
    //            var item = _Set.Local.FirstOrDefault(i => i.id == n) ?? new TabelPerson { id = n };

    //            db.Entry(item).State = EntityState.Deleted;
    //        }

    //        if (Autosave)
    //            db.SaveChanges();

    //        return true;
    //    }
    //}
}
