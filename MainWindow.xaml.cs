using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Diagnostics;

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
        }

        /// <summary>
        /// 폴더 패스를 이용해서 폴더 내부의 파일을 보여주게 하는 함수 호출하는 이벤트 핸들러
        /// </summary>
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedFolderPath = GetSelectedFolderPath();
            if (selectedFolderPath != null)
            {
                System.Windows.MessageBox.Show("선택한 폴더: " + selectedFolderPath, "폴더 선택 완료");
                DisplayFileList(selectedFolderPath);
            }
        }

        /// <summary>
        /// 폴더 Path 가져오는 함수
        /// </summary>
        private string GetSelectedFolderPath()
        {
            using (var folderBrowswerDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowswerDialog.ShowDialog();

                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    return folderBrowswerDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 폴더 내부 파일들의 이름을 보여주는 함수
        /// </summary>
        private void DisplayFileList(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            string[] directories = Directory.GetDirectories(folderPath);

            ListBox.Items.Clear();

            foreach (string directoryPath in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                ListBox.Items.Add($"폴더: {directoryInfo.Name}");
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string revisedName = fileInfo.Name;
                if (fileInfo.Name.StartsWith("~$"))
                {
                    revisedName = fileInfo.Name.Substring(2);
                }
                ListBox.Items.Add($"파일: {revisedName}");
            }
        }
    }
}
