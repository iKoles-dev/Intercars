using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Homebrew;

namespace Intercars
{
    using System.Windows.Media;

    using Intercars.Controller;
    using Intercars.Controller.Khcode;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _khcode = "";
        #region Default settings
        public MainWindow()
        {
            InitializeComponent();
            Controls.DebugBox = DebugBox;
            Controls.WorkProgress = WorkProgress;
            Controls.WorkProgressLabel = WorkProgressLabel;
            BaseDropDown.Visibility = Visibility.Hidden;
            BaseDropDown_Copy.Visibility = Visibility.Hidden;
            DropLable.Visibility = Visibility.Hidden;
            ExcelImage.Visibility = Visibility.Hidden;
            SearchKhcode();
        }

        private void ProgramWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void TelegramButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/iKolesDev");
        }

        private void Developer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TelegramButton_MouseDown(sender, e);
        }
        #endregion
        #region DragNDrop
        private void BaseDropDown_Copy_DragEnter(object sender, DragEventArgs e)
        {
            DropLable.Set("Отпустите левую кнопку мыши.");
            BaseDropDown_Copy.Fill = new SolidColorBrush(Color.FromRgb(0, 111, 111));
        }

        private void BaseDropDown_Copy_DragLeave(object sender, DragEventArgs e)
        {
            DropLable.Set("Перетащите Excel-файл в данную область.");
            BaseDropDown_Copy.Fill = new SolidColorBrush(Color.FromRgb(99, 92, 92));
        }

        private void BaseDropDown_Copy_Drop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                if (files[0].ToLower().EndsWith(".xlsx"))
                {
                    BaseDropDown.Visibility = Visibility.Hidden;
                    BaseDropDown_Copy.Visibility = Visibility.Hidden;
                    DropLable.Visibility = Visibility.Hidden;
                    ExcelImage.Visibility = Visibility.Hidden;
                    Start(files[0]);
                }
                else
                {
                    BaseDropDown_Copy_DragLeave(sender, e);
                }
            }

        }

        private void DropLable_DragEnter(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_DragEnter(sender, e);
        }

        private void DropLable_DragLeave(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_DragLeave(sender, e);
        }
        private void DropLable_Drop(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_Drop(sender, e);
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_DragEnter(sender, e);
        }

        private void Image_DragLeave(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_DragLeave(sender, e);
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            BaseDropDown_Copy_Drop(sender, e);
        }
        #endregion
        #region SearchForKhcode

        private void SearchKhcode()
        {
            new Thread(
                () =>
                    {
                        KhcodeSearcher khcodeSearcher = new KhcodeSearcher();
                        Application.Current.Dispatcher.Invoke(new Action(
                            () =>
                                {
                                    _khcode = khcodeSearcher.Khcode;
                                    BaseDropDown.Visibility = Visibility.Visible;
                                    BaseDropDown_Copy.Visibility = Visibility.Visible;
                                    DropLable.Visibility = Visibility.Visible;
                                    ExcelImage.Visibility = Visibility.Visible;
                                }));
                    }).Start();
        }

        #endregion

        private void Start(string path)
        {
            Thread thread = new Thread(
                () =>
                    {

                        Controls.DebugBox.WriteLine("Excel-файл был успешно подгружен!");
                        Intercars intercars = new Intercars(path,_khcode);

                    });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
