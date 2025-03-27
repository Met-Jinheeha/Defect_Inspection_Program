using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        private string currentFolderPath;
        public string FullPath { get; set; }


        /// <summary>
        /// 폴더 패스를 이용해서 폴더 내부의 파일을 보여주게 하는 함수 호출하는 이벤트 핸들러
        /// </summary>
        /// 
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedFolderPath = GetSelectedFolderPath();
            if (selectedFolderPath != null)
            {
                System.Windows.MessageBox.Show("선택한 폴더: " + selectedFolderPath, "폴더 선택 완료");
                DisplayFileList(selectedFolderPath);
                currentFolderPath = selectedFolderPath;
            }
        }


        /// <summary>
        /// 폴더 Path 가져오는 함수
        /// </summary>
        /// 
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
        /// 이거 일단 보류
        /// </summary>
        /// 
        private void DisplayFileList(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            string[] directories = Directory.GetDirectories(folderPath);

            ListBox.Items.Clear();

            foreach (string directoryPath in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                ListBox.Items.Add(directoryInfo.Name);
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string revisedName = fileInfo.Name;
                if (fileInfo.Name.StartsWith("~$"))
                {
                    revisedName = fileInfo.Name.Substring(2);
                }
                ListBox.Items.Add(revisedName);
            }

            KlarfFileParser parser = new KlarfFileParser();
            parser.ParseText(FullPath); // 스트링
            //TextBlock.Text = parser.ParsedContent;
        }

        /// <summary>
        /// 리스트 박스에 있는걸 클릭했을 때 파싱해주는 함수
        /// </summary>
        /// 



        //public Dictionary<Point, List<DefectInfo>> GetAllDefects()
        //{
        //    return chipDefects;
        //}





        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                string selectedFileName = ListBox.SelectedItem.ToString();
                FullPath = Path.Combine(currentFolderPath, selectedFileName);

                Console.WriteLine(FullPath);

                KlarfFileParser parser = new KlarfFileParser();

                ChipInfo chip = new ChipInfo();
                DefectInfo defect = new DefectInfo();

                parser.ParseText(FullPath);

                string defectInfo = chip.GetAllDefects();

                ObservableCollection<object> items = new ObservableCollection<object>();

                string[] lines = defectInfo.Split('\n');

                Console.WriteLine("라인" + lines[1]);


                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    // 먼저 길이 확인 후 parts[1] 접근
                    if (parts.Length > 1)
                    {
                        Console.WriteLine("파트" + parts[1]);
                    }

                    if (parts.Length < 7) continue;

                    // 익명 객체 사용
                    items.Add(new
                    {
                        id = parts[2],
                        xrel = parts[3],
                        yrel = parts[4],
                        xindex = parts[5],
                        yindex = parts[6],
                        xsize = parts[7],
                        ysize = parts[8]
                    });
                }

                defectList.ItemsSource = items;

                Console.WriteLine(items);
            }
        }
    }
}
