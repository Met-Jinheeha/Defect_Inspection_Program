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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TiffImageLoaderViewModel();
        }

        private string currentFolderPath;
        public string CurrentFolderPath
        {
            get => currentFolderPath;
            set => currentFolderPath = value;
        }
        public string FullPath { get; set; }

        public bool isChipDataView = false;


        private string selectedFileName;
        public string SelectedFileName
        {
            get => selectedFileName;
            set => selectedFileName = value;
        }

        // 전체 디펙개수
        int wholeWaferDefectCount = 0;

        // 전체 칩 인덱스
        int currentWaferIndex = 0;

        // 칩 내부 디펙 현재 인덱스
        int currentChipDefectIndex = 0;

        // 선택된 Tiff 이미지 인덱스
        int currentImageIndex = 0;

        private ObservableCollection<object> allDefectsItems = new ObservableCollection<object>();
        private ObservableCollection<object> chipDefectsItems = new ObservableCollection<object>();

        

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 트리뷰에 폴더 추가
                LoadFolders(dialog.SelectedPath);
            }
        }

        private void LoadFolders(string path) // 탐색기에서 폴더 선택했을때
        {
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
            }
        }

        // 탐색기에서 폴더 선택했을때
        private TreeViewItem CreateTreeItem(DirectoryInfo dirInfo)
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


        // 탐색기에서 폴더 선택했을때, 혹은 내부 트리뷰에서 폴더 선택했을때 
        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = FolderTreeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                LoadFiles(item.Tag.ToString());
            }
        }

        private void LoadFiles(string path)
        {
            ListBox.Items.Clear();
 
                CurrentFolderPath = path;
                foreach (var file in Directory.GetFiles(path))
                {
                    if (!IsKlarfInfoFileCheck(file))
                    {
                        continue;
                    }
                    ListBox.Items.Add(Path.GetFileName(file));
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


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var item = FolderTreeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                LoadFiles(item.Tag.ToString());        
            }
            ChipInfo chip = new ChipInfo();
            chip.ChipDefectClear();
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


        public void ShowAllList(object sender, RoutedEventArgs e)
        {
            defectList.ItemsSource = allDefectsItems;
            currentWaferIndex = 0;
            currentChipDefectIndex = 0;
            txtDefectOnWafer.Text = $"전체 디펙: 1/{allDefectsItems.Count}";

            //TiffImageLoaderViewModel loader = new TiffImageLoaderViewModel();
            var vm = this.DataContext as TiffImageLoaderViewModel;
            vm.LoadTiffImage(CurrentFolderPath);
            isChipDataView = false;
        }

        /// <summary>
        /// 리스트 박스에 있는것(Klarf.txt) 클릭했을 때 파싱해주는 함수
        /// </summary>
        /// 
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                ChipInfo chip = new ChipInfo();
                chip.ChipDefectClear();

                SelectedFileName = ListBox.SelectedItem.ToString();
                FullPath = Path.Combine(CurrentFolderPath, SelectedFileName);

                Console.WriteLine(FullPath);

                KlarfFileParser parser = new KlarfFileParser();
               
                parser.ParseText(FullPath);

                string defectInfo = chip.GetAllDefects();

                ObservableCollection<object> items = new ObservableCollection<object>();

                string[] lines = defectInfo.Split('\n');

                allDefectsItems.Clear();

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length < 7) continue;

                    // 익명 객체 사용
                    allDefectsItems.Add(new
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

                waferInfomationText.Text = parser.waferInfo;

                defectList.ItemsSource = allDefectsItems;
                currentWaferIndex = 0;
                currentChipDefectIndex = 0;
                txtDefectOnWafer.Text = $"전체 디펙: 1/{allDefectsItems.Count}";

                var vm = this.DataContext as TiffImageLoaderViewModel;
                vm.LoadTiffImage(CurrentFolderPath);

                Console.WriteLine(items);
            }
        }


        /// <summary>
        /// 버튼 눌렀을때 특정 칩의 디펙을 보여주는 함수
        /// </summary>
        /// 
        private void TransferCoordinateButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = transferCoordinate1.Text;
            string[] tempArr = temp.Split(',');

            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = tempArr[i].Trim();
            }

            int x = int.Parse(tempArr[0]);
            int y = int.Parse(tempArr[1]);
           
            ChipInfo chip = new ChipInfo();
           
            ObservableCollection<object> items = new ObservableCollection<object>();

            List<DefectInfo> defectInfo = chip.GetDefects(x, y);

            if(defectInfo.Count == 0)
            {
                txtDefectOnChip.Text = $"칩 내 디펙: {0}/{0}";
                ClearDefectImage();
                return;
            }

            currentImageIndex = 0;

            chipDefectsItems.Clear();

            for (int i = 0; i < defectInfo.Count; i++)
            {
                chipDefectsItems.Add(new
                {
                    id = defectInfo[i].DefectId,
                    xrel = defectInfo[i].XRel,
                    yrel = defectInfo[i].YRel,
                    xindex = defectInfo[i].XIndex,
                    yindex = defectInfo[i].YIndex,
                    xsize = defectInfo[i].XSize,
                    ysize = defectInfo[i].YSize
                });
            }

            // 칩 모드로 전환
            // 먼저 ItemsSource 설정
            defectList.ItemsSource = chipDefectsItems;

            // 아이템이 있는지 확인
            if (chipDefectsItems.Count > 0)
            {
                // 첫 아이템 선택
                defectList.SelectedIndex = 0;
                currentChipDefectIndex = 0;

                // 이제 SelectedItem 접근
                dynamic selectedItem = defectList.SelectedItem;
                currentImageIndex = selectedItem != null ? (int)selectedItem.id - 1 : -2;
            }
            isChipDataView = true;
            txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";
            var vm = this.DataContext as TiffImageLoaderViewModel;
            vm.LoadDefectImageFromChipOnSelected(currentImageIndex);
        }



        private void NextDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        {
            if (isChipDataView)
            {
                return;
            }

            currentWaferIndex++;
            int count = defectList.Items.Count;

            defectList.SelectedIndex = currentWaferIndex;

            txtDefectOnWafer.Text =  $"전체 디펙: {currentWaferIndex + 1}/{count}";
            var vm = this.DataContext as TiffImageLoaderViewModel;
            vm.LoadDefectImageFromWholeSelected(currentWaferIndex);
        }


        private void PreviousDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        {

            if (isChipDataView)
            {
                return;
            }

            if (currentWaferIndex > 0)
            {
                currentWaferIndex--;
                wholeWaferDefectCount = defectList.Items.Count;

                defectList.SelectedIndex = currentWaferIndex;

                txtDefectOnWafer.Text = $"전체 디펙: {currentWaferIndex + 1}/{wholeWaferDefectCount}";
                var vm = this.DataContext as TiffImageLoaderViewModel;
                vm.LoadDefectImageFromWholeSelected(currentWaferIndex);
            }
        }


        // 칩 내부 다음 결함
        private void NextDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isChipDataView)
            {
                return;
            }

            if (chipDefectsItems.Count == 0) return;

            if (currentChipDefectIndex < chipDefectsItems.Count - 1)
            {
                currentChipDefectIndex++; // 리스트 기준 인덱스 증가
                defectList.SelectedIndex = currentChipDefectIndex; // 리스트에서 해당 인덱스 선택

                // 선택된 아이템에서 defectId 가져오기
                dynamic selectedItem = defectList.SelectedItem;
                currentImageIndex = selectedItem != null ? (int)selectedItem.id -1: 0;

                txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

                // 올바른 id로 이미지 로드
                var vm = this.DataContext as TiffImageLoaderViewModel;
                vm.LoadDefectImageFromChipOnSelected(currentImageIndex);
            }
        }


        // 칩 내부 이전 결함
        private void PreviousDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isChipDataView)
            {
                return;
            }

            if (chipDefectsItems.Count == 0) return;

            if (currentChipDefectIndex > 0)
            {
                currentChipDefectIndex--;
                defectList.SelectedIndex = currentChipDefectIndex;

                // 선택된 아이템에서 defectId 가져오기
                dynamic selectedItem = defectList.SelectedItem;
                currentImageIndex = selectedItem != null ? (int)selectedItem.id - 1 : 0;

                txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

                var vm = this.DataContext as TiffImageLoaderViewModel;
                vm.LoadDefectImageFromChipOnSelected(currentImageIndex);

            }
        }
        private void ClearDefectImage()
        {
            CurrentImage.Source = null;
            NoImageText.Visibility = Visibility.Visible;
        }
    }
}