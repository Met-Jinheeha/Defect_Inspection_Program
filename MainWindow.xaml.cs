using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DefectViewProgram
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // MainViewModel 인스턴스를 생성해 DataContext에 할당
            var mainViewModel = new MainViewModel();
            DataContext = mainViewModel;

            // FileOpenViewModel에 UI 요소 연결
            mainViewModel.fileOpenViewModel.FolderTreeView = this.FolderTreeView;
            mainViewModel.fileOpenViewModel.FileListBox = this.FileListBox;

            mainViewModel.waferMapViewModel.SetCanvas(this.WaferCanvas);
        }
    }
}