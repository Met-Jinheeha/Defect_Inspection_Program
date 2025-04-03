using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace DefectViewProgram
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        private FileOpenViewModel fileOpenViewModel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TiffImageLoaderViewModel();
            DataContext = new FileOpenViewModel();

            // 뷰모델 인스턴스 가져오기
            fileOpenViewModel = DataContext as FileOpenViewModel;

            // UI 요소 연결
            if (fileOpenViewModel != null)
            {
                fileOpenViewModel.FolderTreeView = this.FolderTreeView;
                fileOpenViewModel.FileListBox = this.FileListBox;
            }
        }

        // 트리뷰 선택 변경 이벤트 핸들러
        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (fileOpenViewModel != null)
            {
                fileOpenViewModel.FolderTreeView_SelectedItemChanged(sender, e);
            }
        }
    }
}