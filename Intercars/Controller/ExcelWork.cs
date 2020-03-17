namespace Intercars.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Homebrew;
    using Microsoft.Office.Interop.Excel;

    public class ExcelWork
    {
        private string _path = "";
        public List<IntercarsProfile> ExcelRead(string fileName)
        {
            _path = fileName;
            List<IntercarsProfile> intercarsProfiles = new List<IntercarsProfile>();
            Controls.DebugBox.WriteLine($"Приступаем к чтению {fileName}");
            Application ObjExcel = new Application();
            //Открываем книгу.                                                                                                                                                        
            Workbook ObjWorkBook = ObjExcel.Workbooks.Open(fileName, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Выбираем таблицу(лист).
            Worksheet ObjWorkSheet;
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];

            // Указываем номер столбца (таблицы Excel) из которого будут считываться данные.
            int numCol = 1;

            Range usedColumn = ObjWorkSheet.UsedRange.Columns[numCol];
            Array myvalues = (Array)usedColumn.Cells.Value2;
            List<string> allValues = new List<string>(myvalues.OfType<object>().Select(o => o.ToString()).ToArray());
            for (int i = 1; allValues.Count > i; i++)
            {
                intercarsProfiles.Add(new IntercarsProfile(allValues[i]));
                Controls.WorkProgress.SetValue(i*1d/allValues.Count*100);
                Controls.WorkProgressLabel.Set($"{(i * 1d / allValues.Count * 100).ToString("##.##")}%");
            }
            Controls.WorkProgress.SetValue(100);
            Controls.WorkProgressLabel.Set("100%");
            Controls.DebugBox.WriteLine($"Чтение файла завершено, найдено {intercarsProfiles.Count} строк.");

            // Выходим из программы Excel.
            ObjWorkBook.Close(true);
            ObjExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ObjWorkBook);
            ObjExcel = null;
            ObjWorkBook = null;
            ObjWorkSheet = null;
            System.GC.Collect();
            return intercarsProfiles;
        }

        public void ExcelWrite(List<IntercarsProfile> profiles)
        {
            Controls.DebugBox.WriteLine($"Приступаем к записи информации в {Path.GetFileName(_path)}");
            Application ObjExcel = new Application();
            //Открываем книгу.     

            Workbook ObjWorkBook = null;
            try
            {
                ObjWorkBook = ObjExcel.Workbooks.Open(_path, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            }
            catch (Exception)
            {
                Controls.DebugBox.WriteLine("Ошибка открытия файла для последующей записи.");
            }
            //Выбираем таблицу(лист).
            Worksheet ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            ObjWorkSheet.Cells[1, 3] = "Онлайн доступність:";
            ObjWorkSheet.Cells[1, 4] = "Online availability in your branch group:";
            ObjWorkSheet.Cells[1, 5] = "Oнлайн доступність у вашому відділенні : ";
            ObjWorkSheet.Cells[1, 6] = "Ціна:";
            ObjWorkSheet.Cells[1, 7] = "Роздрібна ціна:";
            ObjWorkSheet.Cells[1, 8] = "Оптова ціна:";
            ObjWorkSheet.Cells[1, 9] = "Cсылка на фото:";
            ObjWorkSheet.Cells[1, 10] = "Застосування:";
            ObjWorkSheet.Cells[1, 11] = "Модель:";
            ObjWorkSheet.Cells[1, 12] = "Замінники  Індекс:";
            ObjWorkSheet.Cells[1, 13] = "Оригінальні номери OE:";
            ObjWorkSheet.Cells[1, 14] = "Додаткова інформація з каталогу:";
            for (int i = 0; i < profiles.Count; i++)
            {
                try
                {
                    ObjWorkSheet.Cells[i + 2, 2] = profiles[i].Description;
                    ObjWorkSheet.Cells[i + 2, 3] = profiles[i].OnlineAvailability;
                    ObjWorkSheet.Cells[i + 2, 4] = profiles[i].AvailabilityInBranchGroup;
                    ObjWorkSheet.Cells[i + 2, 5] = profiles[i].AvailabilityInViddelenni;
                    ObjWorkSheet.Cells[i + 2, 6] = profiles[i].PriceType;
                    ObjWorkSheet.Cells[i + 2, 7] = profiles[i].PriceOpt;
                    ObjWorkSheet.Cells[i + 2, 8] = profiles[i].PriceRozdrib;
                    ObjWorkSheet.Cells[i + 2, 9] = profiles[i].Image;
                    ObjWorkSheet.Cells[i + 2, 10] = profiles[i].Mark;
                    ObjWorkSheet.Cells[i + 2, 11] = profiles[i].Model;
                    ObjWorkSheet.Cells[i + 2, 12] = profiles[i].Zaminniki;
                    ObjWorkSheet.Cells[i + 2, 13] = profiles[i].OriginalNumbers;
                    ObjWorkSheet.Cells[i + 2, 14] = profiles[i].AdditionalInformation;
                    Controls.WorkProgress.SetValue(1d*i/profiles.Count*100);
                    Controls.WorkProgressLabel.Set($"{(1d * i / profiles.Count * 100).ToString("##.##")}%");
                }
                catch (Exception)
                {
                    Controls.DebugBox.WriteLine("Ошибка при попытке записи новых данных.");
                }
            }
            try
            {
                ObjWorkSheet.SaveAs(_path);
            }
            catch (Exception)
            {
                Controls.DebugBox.WriteLine("Ошибка сохранения данных.");
            }
            for (int i = 1; i < 15; i++)
            {
                ObjWorkSheet.Columns[i].AutoFit();
            }
            Controls.WorkProgress.SetValue(100);
            Controls.WorkProgressLabel.Set($"100%");
            ObjExcel.Visible = true;
            Controls.DebugBox.WriteLine("Запись завершена!");
        }
    }
}