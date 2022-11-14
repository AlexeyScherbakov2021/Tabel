using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Models;
using System.IO;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using System.Diagnostics;

namespace Tabel.Repository
{
    internal class RepositoryWord
    {
        private string _FileName;
        private Dictionary<string, string> _Words;


        public RepositoryWord(string fileName)
        {
            
            _FileName = fileName;
        }

        public void CreateWorkOffSZ(IEnumerable<Models.Personal> listPerson, DateTime dt)
        {
            string newFile = System.IO.Path.GetTempPath() + "СЗ на выходной день.docx";
            File.Copy(_FileName, newFile, true);

            try
            {
                using (var word = WordprocessingDocument.Open(newFile, true))
                {
                    var bookMarks = FindBookmarks(word.MainDocumentPart.Document);

                    foreach (var end in bookMarks)
                    {
                        if (end.Key == "ДАТА")
                        {
                            var textElement = new Text(dt.ToString("dd MMMM yyyy года"));
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не найден шаблон СЗ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Process.Start(newFile);

        }


        private static Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> outs = null, Dictionary<string, string> bStartWithNoEnds = null)
        {
            if (outs == null) { outs = new Dictionary<string, BookmarkEnd>(); }
            if (bStartWithNoEnds == null) { bStartWithNoEnds = new Dictionary<string, string>(); }

            // Проходимся по всем элементам на странице Word-документа
            foreach (var docElement in documentPart.Elements())
            {
                // BookmarkStart определяет начало закладки в рамках документа
                // маркер начала связан с маркером конца закладки
                if (docElement is BookmarkStart)
                {
                    var bookmarkStart = docElement as BookmarkStart;
                    // Записываем id и имя закладки
                    bStartWithNoEnds.Add(bookmarkStart.Id, bookmarkStart.Name);
                }

                // BookmarkEnd определяет конец закладки в рамках документа
                if (docElement is BookmarkEnd)
                {
                    var bookmarkEnd = docElement as BookmarkEnd;
                    foreach (var startName in bStartWithNoEnds)
                    {
                        // startName.Key как раз и содержит id закладки
                        // здесь проверяем, что есть связь между началом и концом закладки
                        if (bookmarkEnd.Id == startName.Key)
                            // В конечный массив добавляем то, что нам и нужно получить
                            outs.Add(startName.Value, bookmarkEnd);
                    }
                }
                // Рекурсивно вызываем данный метод, чтобы пройтись по всем элементам
                // word-документа
                FindBookmarks(docElement, outs, bStartWithNoEnds);
            }

            return outs;
        }


    }
}
