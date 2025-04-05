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
        public ObservableCollection<FileOpenViewModel> Children { get; set; } = new ObservableCollection<FileOpenViewModel>();


        public bool isChipDataView = false;


        private string selectedFileName;
        public string SelectedFileName
        {
            get => selectedFileName;
            set => selectedFileName = value;
        }

        //private TreeView folderTreeView;
        //public TreeView FolderTreeView
        //{
        //    get => folderTreeView;
        //    set => SetProperty(ref folderTreeView, value);
        //}


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


        // ViewModel
        private ObservableCollection<DefectInfo> defectList =  new ObservableCollection<DefectInfo>();
        public ObservableCollection<DefectInfo> DefectList
        {
            get => defectList;
            set => SetProperty(ref defectList, value);
        }

        // 전체 디펙개수
        public int wholeWaferDefectCount = 0;

        // 전체 칩 인덱스
        public int currentWaferIndex = 0;

        // 칩 내부 디펙 현재 인덱스
        public int currentChipDefectIndex = 0;

        // 선택된 Tiff 이미지 인덱스
        public int currentImageIndex = 0;


        private ObservableCollection<object> allDefectsItems = new ObservableCollection<object>();
        private ObservableCollection<object> chipDefectsItems = new ObservableCollection<object>();


        private string buttonMessage = "폴더선택을로직에서";
        public string ButtonMessage
        {
            get { return buttonMessage; }
            set { SetProperty(ref buttonMessage, value); }

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


        //public void ShowAllList(object sender, RoutedEventArgs e)
        //{
        //    defectList.ItemsSource = allDefectsItems;
        //    currentWaferIndex = 0;
        //    currentChipDefectIndex = 0;
        //    txtDefectOnWafer.Text = $"전체 디펙: 1/{allDefectsItems.Count}";

        //    TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //    loader.LoadTiffImage(CurrentFolderPath);
        //    isChipDataView = false;
        //}


        private string textDefectOnWafer = "전체 디펙: 0/0";
        public string TextDefectOnWafer
        {
            get => textDefectOnWafer;
            set => SetProperty(ref textDefectOnWafer, value);
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
                ChipInfo chip = new ChipInfo();
                chip.ChipDefectClear();

                SelectedFileName = FileListBox.SelectedItem.ToString();
                FullPath = Path.Combine(CurrentFolderPath, SelectedFileName);

                Console.WriteLine(FullPath);

                KlarfFileParser parser = new KlarfFileParser();

                parser.ParseText(FullPath);

                string defectInfo = chip.GetAllDefects();

                //ObservableCollection<object> items = new ObservableCollection<object>();

                string[] lines = defectInfo.Split('\n');
                
                allDefectsItems.Clear();

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length < 7) continue;

                    DefectList.Add(new DefectInfo
                    {
                        DefectId = int.Parse(parts[2]),
                        XRel = double.Parse(parts[3]),
                        YRel = double.Parse(parts[4]),
                        XIndex = int.Parse(parts[5]),
                        YIndex = int.Parse(parts[6]),
                        XSize = int.Parse(parts[7]),
                        YSize = int.Parse(parts[8])
                    });
                }
            

                WaferInformation = parser.waferInfo;

                FileSelected?.Invoke(CurrentFolderPath);

                //DefectItems.ItemsSource = allDefectsItems;
                currentWaferIndex = 0;
                currentChipDefectIndex = 0;
                TextDefectOnWafer = $"전체 디펙: 1/{allDefectsItems.Count}";

                if (MainViewModel != null && MainViewModel.tiffLoaderViewModel != null)
                {
                    MainViewModel.tiffLoaderViewModel.LoadTiffImage(CurrentFolderPath);
                }

                //Console.WriteLine(items);
            }
        }



        /// <summary>
        /// 버튼 눌렀을때 특정 칩의 디펙을 보여주는 함수
        /// </summary>
        /// 
        //public void TransferCoordinateButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string temp = transferCoordinate1.Text;

        //    // 웨이퍼 그래픽에서 입력 됐을때 들어오는 속성추가해야됨 04.02

        //    string[] tempArr = temp.Split(',');

        //    for (int i = 0; i < tempArr.Length; i++)
        //    {
        //        tempArr[i] = tempArr[i].Trim();
        //    }

        //    int x = int.Parse(tempArr[0]);
        //    int y = int.Parse(tempArr[1]);

        //    ChipInfo chip = new ChipInfo();

        //    //ObservableCollection<object> items = new ObservableCollection<object>();

        //    List<DefectInfo> defectInfo = chip.GetDefects(x, y);

        //    if (defectInfo.Count == 0)
        //    {
        //        txtDefectOnChip.Text = $"칩 내 디펙: {0}/{0}";
        //        ClearDefectImage();
        //        return;
        //    }

        //    currentImageIndex = 0;

        //    chipDefectsItems.Clear();

        //    for (int i = 0; i < defectInfo.Count; i++)
        //    {
        //        chipDefectsItems.Add(new
        //        {
        //            id = defectInfo[i].DefectId,
        //            xrel = defectInfo[i].XRel,
        //            yrel = defectInfo[i].YRel,
        //            xindex = defectInfo[i].XIndex,
        //            yindex = defectInfo[i].YIndex,
        //            xsize = defectInfo[i].XSize,
        //            ysize = defectInfo[i].YSize
        //        });
        //    }

        //    // 칩 모드로 전환
        //    // 먼저 ItemsSource 설정
        //    defectList.ItemsSource = chipDefectsItems;

        //    // 아이템이 있는지 확인
        //    if (chipDefectsItems.Count > 0)
        //    {
        //        // 첫 아이템 선택
        //        defectList.SelectedIndex = 0;
        //        currentChipDefectIndex = 0;

        //        // 이제 SelectedItem 접근
        //        dynamic selectedItem = defectList.SelectedItem;
        //        currentImageIndex = selectedItem != null ? (int)selectedItem.id - 1 : -2;
        //    }
        //    isChipDataView = true;
        //    txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";
        //    //var vm = this.DataContext as TiffImageLoaderViewModel;
        //    //vm.LoadDefectImageFromChipOnSelected(currentImageIndex);
        //    TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //    loader.LoadTiffImage(CurrentFolderPath);
        //}



        //public void NextDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (isChipDataView)
        //    {
        //        return;
        //    }

        //    currentWaferIndex++;
        //    int count = defectList.Items.Count;

        //    defectList.SelectedIndex = currentWaferIndex;

        //    txtDefectOnWafer.Text = $"전체 디펙: {currentWaferIndex + 1}/{count}";
        //    //var vm = this.DataContext as TiffImageLoaderViewModel;
        //    //vm.LoadDefectImageFromWholeSelected(currentWaferIndex);
        //    TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //    loader.LoadTiffImage(CurrentFolderPath);
        //}


        //public void PreviousDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        //{

        //    if (isChipDataView)
        //    {
        //        return;
        //    }

        //    if (currentWaferIndex > 0)
        //    {
        //        currentWaferIndex--;
        //        wholeWaferDefectCount = defectList.Items.Count;

        //        defectList.SelectedIndex = currentWaferIndex;

        //        txtDefectOnWafer.Text = $"전체 디펙: {currentWaferIndex + 1}/{wholeWaferDefectCount}";
        //        //var vm = this.DataContext as TiffImageLoaderViewModel;
        //        //vm.LoadDefectImageFromWholeSelected(currentWaferIndex);
        //        TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //        loader.LoadTiffImage(CurrentFolderPath);
        //    }
        //}


        // 칩 내부 다음 결함
        //public void NextDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!isChipDataView)
        //    {
        //        return;
        //    }

        //    if (chipDefectsItems.Count == 0) return;

        //    if (currentChipDefectIndex < chipDefectsItems.Count - 1)
        //    {
        //        currentChipDefectIndex++; // 리스트 기준 인덱스 증가
        //        defectList.SelectedIndex = currentChipDefectIndex; // 리스트에서 해당 인덱스 선택

        //        // 선택된 아이템에서 defectId 가져오기
        //        dynamic selectedItem = defectList.SelectedItem;
        //        currentImageIndex = selectedItem != null ? (int)selectedItem.id - 1 : 0;

        //        txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

        //        // 올바른 id로 이미지 로드
        //        //var vm = this.DataContext as TiffImageLoaderViewModel;
        //        //vm.LoadDefectImageFromChipOnSelected(currentImageIndex);
        //        TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //        loader.LoadTiffImage(CurrentFolderPath);
        //    }
        //}


        // 칩 내부 이전 결함
        //public void PreviousDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!isChipDataView)
        //    {
        //        return;
        //    }

        //    if (chipDefectsItems.Count == 0) return;

        //    if (currentChipDefectIndex > 0)
        //    {
        //        currentChipDefectIndex--;
        //        defectList.SelectedIndex = currentChipDefectIndex;

        //        // 선택된 아이템에서 defectId 가져오기
        //        dynamic selectedItem = defectList.SelectedItem;
        //        currentImageIndex = selectedItem != null ? (int)selectedItem.id - 1 : 0;

        //        txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

        //        //var vm = this.DataContext as TiffImageLoaderViewModel;
        //        //vm.LoadDefectImageFromChipOnSelected(currentImageIndex);
        //        TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
        //        loader.LoadTiffImage(CurrentFolderPath);

        //    }
        //}
        //public void ClearDefectImage()
        //{
        //    CurrentImage.Source = null;
        //    NoImageText.Visibility = Visibility.Visible;
        //}

    }
}
