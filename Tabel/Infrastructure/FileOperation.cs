using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabel.Infrastructure
{
    internal class FileOperation
    {
        public static string GenerateTempFileNameWithDelete(string Name)
        {
            int num = 0;
            string TempFile;
            string ext = Path.GetExtension(Name);
            Name = Path.GetFileNameWithoutExtension(Name);

            do
            {
                if (num == 0)
                    TempFile = Path.GetTempPath() + $"{Name}.{ext}";
                else
                    TempFile = Path.GetTempPath() + $"{Name}({num}).{ext}";

                num++;

                try
                {
                    File.Delete(TempFile);
                }
                catch { }

            } while (File.Exists(TempFile));

            return TempFile;
        }

    }
}
