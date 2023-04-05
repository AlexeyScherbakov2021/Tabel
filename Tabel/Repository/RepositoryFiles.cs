using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryFiles
    {
#if DEBUG
        static string FileStorage = @"\\ngk-dc-07\FileStore$\TabelD\";
#else
        static string FileStorage = @"\\ngk-dc-07\FileStore$\Tabel\";
#endif


        //--------------------------------------------------------------------------------------------
        // получение нужного пути и созздание директории года
        //--------------------------------------------------------------------------------------------
        private string CurrentPath(int Year)
        {
            string NewPath = FileStorage + Year.ToString() + "\\";
            if (!Directory.Exists(NewPath))
                Directory.CreateDirectory(NewPath);

            return NewPath;
        }

        //--------------------------------------------------------------------------------------------
        // Добавление файлов
        //--------------------------------------------------------------------------------------------
        public void AddFiles(AttachFile FileName, int Year)
        {
            string NewName;
            string NewPath = CurrentPath(Year);

            if (FileName.FullName != null)
            {
                if(string.IsNullOrEmpty(FileName.FileName))
                    FileName.FileName = Path.GetFileName(FileName.FullName);

                NewName = NewPath + FileName.task_id.ToString() + "." + FileName.FileName;
                File.Copy(FileName.FullName, NewName, true);

                // записано, обнуляем
                FileName.FullName = null;
            }
        }

        public void AddFiles(ICollection<AttachFile> ListFiles, int Year)
        {
            string NewName;
            string NewPath = CurrentPath(Year);

            foreach (AttachFile file in ListFiles)
            {
                if (file.FullName != null)
                {
                    if (string.IsNullOrEmpty(file.FileName))
                        file.FileName = Path.GetFileName(file.FullName);

                    NewName = NewPath + file.mod_id.ToString() + "." + file.FileName;
                    File.Copy(file.FullName, NewName, true);

                    // записано, обнуляем
                    file.FullName = null;
                }
            }
        }

        //--------------------------------------------------------------------------------------------
        // Добавление файлов
        //--------------------------------------------------------------------------------------------
        public async void AddFilesAsync(AttachFile FilesName, int Year)
        {
            await Task.Run(() => CopyFiles(FilesName, Year));
        }
        //public async void AddFilesAsync(ICollection<AttachFile> ListFiles, int Year)
        //{
        //    await Task.Run(() => CopyFiles(ListFiles, Year));
        //}


        public Task<bool> CopyFiles(AttachFile FileName, int Year)
        {
            string NewName;
            string NewPath = CurrentPath(Year);
            if (FileName.FullName != null)
            {
                NewName = NewPath + FileName.mod_id.ToString() + "." + FileName.FileName;
                try
                {
                    File.Copy(FileName.FullName, NewName, true);
                }
                catch { };

                // записано, обнуляем
                FileName.FullName = null;
            }

            return Task.FromResult(true);
        }

        //public Task<bool> CopyFiles(ICollection<AttachFile> ListFiles, int Year)
        //{
        //    string NewName;
        //    string NewPath = CurrentPath(Year);
        //    foreach (AttachFile file in ListFiles)
        //    {
        //        if (file.FullName != null)
        //        {
        //            NewName = NewPath + file.task_id.ToString() + "." + file.FileName;
        //            try
        //            {
        //                File.Copy(file.FullName, NewName, true);
        //            }
        //            catch { };

        //            // записано, обнуляем
        //            file.FullName = null;
        //        }
        //    }

        //    return Task.FromResult(true);
        //}

        //--------------------------------------------------------------------------------------------
        // Удаление файлов
        //--------------------------------------------------------------------------------------------
        public void DeleteFiles(AttachFile FileName, int Year)
        {
            string NewPath = CurrentPath(Year);
            string NewName;

            NewName = NewPath + FileName.mod_id.ToString() + "." + FileName.FileName;
            try
            {
                File.Delete(NewName);
            }
            catch { }
        }
        //public void DeleteFiles(ICollection<AttachFile> ListFiles, int Year)
        //{
        //    string NewPath = CurrentPath(Year);
        //    string NewName;

        //    foreach (AttachFile file in ListFiles)
        //    {
        //        NewName = NewPath + file.task_id.ToString() + "." + file.FileName;
        //        try
        //        {
        //            File.Delete(NewName);
        //        }
        //        catch { }
        //    }
        //}

        //--------------------------------------------------------------------------------------------
        // Получение файла в TEMP папку
        //--------------------------------------------------------------------------------------------
        public string GetFile(AttachFile atFile, int Year)
        {
            string NewPath = CurrentPath(Year);

            //string NewPath = FileStorage + raFile.RouteOrder.Order.o_date_created.Year.ToString() + "\\";
            string NewName = NewPath + atFile.mod_id.ToString() + "." + atFile.FileName;

            string TempFileName = Path.GetTempPath() + atFile.FileName;

            try
            {
                File.Copy(NewName, TempFileName, true);
            }
            catch
            {
                return null;
            }
            return TempFileName;
        }


    }
}
