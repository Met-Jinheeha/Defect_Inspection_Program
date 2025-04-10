using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using WPFForms = System.Windows.Forms;
using System.Windows.Input;

namespace DefectViewProgram
{
    public class FileOpenViewModel : BaseViewModel
    {

        private string currentFolderPath;
        public string CurrentFolderPath
        {
            get => currentFolderPath;
            set => currentFolderPath = value;
        }
        public string Name { get; set; }
        public string FullPath { get; set; }


        private string selectedFileName;
        public string SelectedFileName
        {
            get => selectedFileName;
            set => selectedFileName = value;
        }

        private bool isShowAllList;
        public bool IsShowAllList
        {
            get => isShowAllList;
            set => isShowAllList = value;
        }

        // 뷰에서 참조할 UI 요소에 대한 속성들
        private System.Windows.Controls.TreeView folderTreeView;
        public System.Windows.Controls.TreeView FolderTreeView
        {
            get => folderTreeView;
            set => SetProperty(ref folderTreeView, value);
        }

        private System.Windows.Controls.ListBox fileListBox;
        public System.Windows.Controls.ListBox FileListBox
        {
            get => fileListBox;
            set => SetProperty(ref fileListBox, value);
        }

        private string waferInformation;
        public string WaferInformation
        {
            get => waferInformation;
            set => SetProperty(ref waferInformation, value);
        }


        // FileOpenViewModel
        private ObservableCollection<DefectInfo> defectList =  new ObservableCollection<DefectInfo>();
        public ObservableCollection<DefectInfo> DefectList
        {
            get => defectList;
            set => SetProperty(ref defectList, value);
        }

        private int selectedDefectIndex;
        public int SelectedDefectIndex
        {
            get => selectedDefectIndex;
            set => SetProperty(ref selectedDefectIndex, value);
        }

        private bool isSelectedKlarfFile;
        public bool IsSelectedKlarfFile
        {
            get => isSelectedKlarfFile;
            set => SetProperty(ref isSelectedKlarfFile, value);
        }

        private int selectedIndex;
        // 전체 칩 인덱스
        public int SelectedIndex
        {
            get => selectedIndex;
            set => SetProperty(ref selectedIndex, value);
        }


        private int selectedFileIndex;
        // 선택된 Klarf 파일 인덱스
        public int SelectedFileIndex
        {
            get => selectedFileIndex;
            set => SetProperty(ref selectedFileIndex, value);
        }


        public void OpenFolder()
        {
            var dialog = new WPFForms.FolderBrowserDialog();
            if (dialog.ShowDialog() == WPFForms.DialogResult.OK)
            {
                // 트리뷰에 폴더 추가
                LoadFolders(dialog.SelectedPath);
            }
        }

        private ICommand openFolderCommand;
        public ICommand OpenFolderCommand
        {
            get
            {
                if (openFolderCommand == null)
                {
                    openFolderCommand = new RelayCommand(OpenFolder);
                }
                return openFolderCommand;
            }
        }

        public void LoadFolders(string path) // 탐색기에서 폴더 선택했을때
        {

            if (FolderTreeView == null) return;

            FolderTreeView.Items.Clear();
            CurrentFolderPath = path;

            try
            {
                var rootDir = new DirectoryInfo(path);
                var rootItem = CreateTreeItem(rootDir);
                FolderTreeView.Items.Add(rootItem);
                rootItem.IsExpanded = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                System.Windows.MessageBox.Show($"폴더 로드 중 오류 발생: {ex.Message}");
            }
        }

        // 탐색기에서 폴더 선택했을때
        public TreeViewItem CreateTreeItem(DirectoryInfo dirInfo)
        {
            var item = new TreeViewItem { Header = dirInfo.Name, Tag = dirInfo.FullName };

            try
            {
                foreach (var subDir in dirInfo.GetDirectories())
                {
                    // 시스템/숨김 폴더 제외
                    if (!subDir.Attributes.HasFlag(FileAttributes.Hidden) &&
                        !subDir.Attributes.HasFlag(FileAttributes.System))
                    {
                        item.Items.Add(CreateTreeItem(subDir));
                    }
                }
            }
            catch { /* 접근 권한 없는 폴더 무시 */ }

            return item;
        }


        // Command 속성
        public ICommand SelectionChangedCommand { get; private set; }

        // 생성자에서 Command 초기화
        public FileOpenViewModel()
        {
            SelectionChangedCommand = new RelayCommand<object>(HandleSelectionChanged);
        }

        // Command 실행 메소드
        private void HandleSelectionChanged(object parameter)
        {
            if (parameter is TreeViewItem item)
            {
                LoadFiles(item.Tag.ToString());
            }
        }

        public void LoadFiles(string path)
        {

            if (FileListBox == null) return;
            FileListBox.Items.Clear();

            CurrentFolderPath = path;
            foreach (var file in Directory.GetFiles(path))
            {
                if (!IsKlarfInfoFileCheck(file))
                {
                    continue;
                }
                FileListBox.Items.Add(Path.GetFileName(file));
            }
        }

        public bool IsKlarfInfoFileCheck(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 처음 20줄만 확인
                    for (int i = 0; i < 20 && !reader.EndOfStream; i++)
                    {
                        string line = reader.ReadLine();
                        if (line.Contains("FileVersion") || line.Contains("WaferID") || line.Contains("LotID"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false; // 파일 읽기 실패시
            }
        }

        /// <summary>
        /// 파일 새로고침
        /// </summary>
        public void RefreshFiles()
        {
            var item = FolderTreeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                LoadFiles(item.Tag.ToString());
            }
            ChipInfo chip = new ChipInfo();
            chip.ChipDefectClear();
        }

        private ICommand refreshFilesCommand;
        public ICommand RefreshFilesCommand
        {
            get
            {
                if (refreshFilesCommand == null)
                {
                    refreshFilesCommand = new RelayCommand(RefreshFiles);
                }
                return refreshFilesCommand;
            }
        }


        /// <summary>
        /// 폴더 Path 가져오는 함수
        /// </summary>
        /// 
        public string GetSelectedFolderPath()
        {
            using (var folderBrowswerDialog = new WPFForms.FolderBrowserDialog())
            {
                WPFForms.DialogResult result = folderBrowswerDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return folderBrowswerDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

        private string textDefectOnWafer = "Total Defect: 0/0";
        public string TextDefectOnWafer
        {
            get => textDefectOnWafer;
            set => SetProperty(ref textDefectOnWafer, value);
        }

        private string textDefectOnChip = "Cell ( , ) Defect 0/0";
        public string TextDefectOnChip
        {
            get => textDefectOnChip;
            set => SetProperty(ref textDefectOnChip, value);
        }


        private string textKlarfFileNum = "Klarf File: ( / )";
        public string TextKlarfFileNum
        {
            get => textKlarfFileNum;
            set => SetProperty(ref textKlarfFileNum, value);
        }


        private ICommand fileListSelectionChangedCommand;
        public ICommand FileListSelectionChangedCommand
        {
            get
            {
                if (fileListSelectionChangedCommand == null)
                {
                    fileListSelectionChangedCommand = new RelayCommand(FileListSelectionChanged);
                }
                return fileListSelectionChangedCommand;
            }
        }

        public MainViewModel MainViewModel { get; set; }

        public event Action<string> FileSelected;

        /// <summary>
        /// 리스트 박스에 있는것(Klarf.txt) 클릭했을 때 파싱해주는 함수
        /// </summary>
        /// 
        public void FileListSelectionChanged()
        {
            if (FileListBox.SelectedItem != null)
            {
                MainViewModel.waferMapViewModel.IsChipSelect = false;
               
                SelectedFileName = FileListBox.SelectedItem.ToString();
                FullPath = Path.Combine(CurrentFolderPath, SelectedFileName);

                ChipInfo chip = new ChipInfo();
                chip.ChipDefectClear();

                KlarfFileParser parser = new KlarfFileParser(chip);

                parser.ParseText(FullPath);

                string defectInfo = chip.GetAllDefects();

                string[] lines = defectInfo.Split('\n');
                
                DefectList.Clear();

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    for (int i = 0; i < parts.Length - 6 ; i += 7)
                    {
                        try
                        {
                            DefectList.Add(new DefectInfo
                            {
                                DefectId = int.Parse(parts[i]),
                                XRel = double.Parse(parts[i + 1]),
                                YRel = double.Parse(parts[i + 2]),
                                XIndex = int.Parse(parts[i + 3]),
                                YIndex = int.Parse(parts[i + 4]),
                                XSize = int.Parse(parts[i + 5]),
                                YSize = int.Parse(parts[i + 6])
                            });
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                WaferInformation = parser.recipeText;

                FileSelected?.Invoke(CurrentFolderPath);

                SelectedIndex = 0;
                IsSelectedKlarfFile = true;
                SelectedDefectIndex = 0;
                TextDefectOnWafer = $"Total Defect: 1/{DefectList.Count}";
                TextKlarfFileNum = $"Klarf File: {SelectedFileName}";


                if (MainViewModel != null && MainViewModel.tiffLoaderViewModel != null)
                {
                    MainViewModel.tiffLoaderViewModel.LoadTiffImage(CurrentFolderPath);
                }
                if (MainViewModel != null && MainViewModel.waferMapViewModel != null)
                {
                    MainViewModel.waferMapViewModel.SetChipInfo(chip);
                    MainViewModel.waferMapViewModel.DrawWaferMap(parser.WaferInfo);
                }
            }
        }
        public void ShowAllList()
        {
            IsSelectedKlarfFile = false;

            MainViewModel.waferMapViewModel.IsChipSelect = false;

            SelectedFileName = FileListBox.SelectedItem.ToString();
            SelectedFileIndex = FileListBox.Items.IndexOf(FileListBox.SelectedItem);
            FullPath = Path.Combine(CurrentFolderPath, SelectedFileName);

            ChipInfo chip = new ChipInfo();
            chip.ChipDefectClear();

            KlarfFileParser parser = new KlarfFileParser(chip);

            parser.ParseText(FullPath);

            string defectInfo = chip.GetAllDefects();

            string[] lines = defectInfo.Split('\n');

            DefectList.Clear();

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                for (int i = 0; i < parts.Length - 6; i += 7)
                {
                    try
                    {
                        DefectList.Add(new DefectInfo
                        {
                            DefectId = int.Parse(parts[i]),
                            XRel = double.Parse(parts[i + 1]),
                            YRel = double.Parse(parts[i + 2]),
                            XIndex = int.Parse(parts[i + 3]),
                            YIndex = int.Parse(parts[i + 4]),
                            XSize = int.Parse(parts[i + 5]),
                            YSize = int.Parse(parts[i + 6])
                        });
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private ICommand showAllListCommand;
        public ICommand ShowAllListCommand
        {
            get
            {
                if (showAllListCommand == null)
                {
                    showAllListCommand = new RelayCommand(ShowAllList);
                }
                return showAllListCommand;
            }
        }



    }
}
