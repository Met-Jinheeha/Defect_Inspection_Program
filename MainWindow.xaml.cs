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
        }

        private string currentFolderPath;
        public string FullPath { get; set; }

        public bool isChipDataView = false;



        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 트리뷰에 폴더 추가
                LoadFolders(dialog.SelectedPath);
            }
        }

        private void LoadFolders(string path)
        {
            FolderTreeView.Items.Clear();
            currentFolderPath = path;

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

            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    ListBox.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var item = FolderTreeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                LoadFiles(item.Tag.ToString());
            }
        }


        /// <summary>
        /// 폴더 패스를 이용해서 폴더 내부의 파일을 보여주게 하는 함수 호출하는 이벤트 핸들러
        /// </summary>
        /// 
        //private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string selectedFolderPath = GetSelectedFolderPath();
        //    if (selectedFolderPath != null)
        //    {
        //        System.Windows.MessageBox.Show("선택한 폴더: " + selectedFolderPath, "폴더 선택 완료");
        //        DisplayFileList(selectedFolderPath);
        //        currentFolderPath = selectedFolderPath;
        //    }
        //}

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
        //private void DisplayFileList(string folderPath)
        //{
        //    string[] files = Directory.GetFiles(folderPath);
        //    string[] directories = Directory.GetDirectories(folderPath);

        //    ListBox.Items.Clear();

        //    foreach (string directoryPath in directories)
        //    {
        //        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        //        ListBox.Items.Add(directoryInfo.Name);
        //    }

        //    foreach (string file in files)
        //    {
        //        FileInfo fileInfo = new FileInfo(file);
        //        string revisedName = fileInfo.Name;
        //        if (fileInfo.Name.StartsWith("~$"))
        //        {
        //            revisedName = fileInfo.Name.Substring(2);
        //        }
        //        ListBox.Items.Add(revisedName);
        //    }

        //    KlarfFileParser parser = new KlarfFileParser();
        //    parser.ParseText(FullPath); // 스트링
        //    //TextBlock.Text = parser.ParsedContent;
        //}


        private string selectedFileName;
        public string SelectedFileName
        {
            get => selectedFileName;
            set => selectedFileName = value;
        }


        public void ShowAllList(object sender, RoutedEventArgs e)
        {
            defectList.ItemsSource = allDefectsItems;
            currentWaferIndex = 0;
            currentChipDefectIndex = 0;
            txtDefectOnWafer.Text = $"전체 디펙: 1/{allDefectsItems.Count}";
            LoadTiffImage();
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
                SelectedFileName = ListBox.SelectedItem.ToString();
                FullPath = Path.Combine(currentFolderPath, SelectedFileName);

                Console.WriteLine(FullPath);

                KlarfFileParser parser = new KlarfFileParser();
                ChipInfo chip = new ChipInfo();

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

                defectList.ItemsSource = allDefectsItems;
                currentWaferIndex = 0;
                currentChipDefectIndex = 0;
                txtDefectOnWafer.Text = $"전체 디펙: 1/{allDefectsItems.Count}";
                LoadTiffImage();

                Console.WriteLine(items);
            }
        }

        private ObservableCollection<object> allDefectsItems = new ObservableCollection<object>();
        private ObservableCollection<object> chipDefectsItems = new ObservableCollection<object>();

        int id = 0;

        /// <summary>
        /// 버튼 눌렀을때 디펙 이동해주는 버튼. 전체 웨이퍼일때만 작동함.
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

            id = 0;

            chipDefectsItems.Clear();

            for (int i = 0; i < defectInfo.Count; i++)
            {
                chipDefectsItems.Add(new
                {
                    id = defectInfo[i].defectId,
                    xrel = defectInfo[i].xRel,
                    yrel = defectInfo[i].yRel,
                    xindex = defectInfo[i].xIndex,
                    yindex = defectInfo[i].yIndex,
                    xsize = defectInfo[i].xSize,
                    ysize = defectInfo[i].ySize
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
                id = selectedItem != null ? (int)selectedItem.id : -2;
            }
            isChipDataView = true;
        }

        // 전체 칩 인덱스
        int currentWaferIndex = 0;

        // 칩 내부 디펙 현재 인덱스
        int currentChipDefectIndex = 0;


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

            LoadDefectImageFromWholeSelected(currentWaferIndex);
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
                int count = defectList.Items.Count;

                defectList.SelectedIndex = currentWaferIndex;

                txtDefectOnWafer.Text = $"전체 디펙: {currentWaferIndex + 1}/{count}";

                LoadDefectImageFromWholeSelected(currentWaferIndex);
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
                id = selectedItem != null ? (int)selectedItem.id : -2;

                txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

                // 올바른 id로 이미지 로드
                LoadDefectImageFromChipOnSelected(id);
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
                id = selectedItem != null ? (int)selectedItem.id : -2;

                txtDefectOnChip.Text = $"칩 내 디펙: {currentChipDefectIndex + 1}/{chipDefectsItems.Count}";

                // 올바른 id로 이미지 로드
                LoadDefectImageFromChipOnSelected(id);
            }
        }

        /// <summary>
        ///  
        /// </summary>

        private TiffBitmapDecoder currentDecoder;
        private List<BitmapSource> tiffFrames = new List<BitmapSource>();
        private int currentFrameIndex = 0;

        private void LoadTiffImage()
        {

                // ID로 tif 파일 경로 생성
                string imagePath = Path.Combine(currentFolderPath, $"Klarf Format.tif");

                if (File.Exists(imagePath))
                {
                    try
                    {
                        // 기존 프레임 목록 초기화
                        tiffFrames.Clear();

                        // TiffBitmapDecoder를 사용하여 멀티페이지 TIFF 로드
                        currentDecoder = new TiffBitmapDecoder(
                            new Uri(imagePath, UriKind.Absolute),
                            BitmapCreateOptions.PreservePixelFormat,
                            BitmapCacheOption.OnLoad);

                        // 모든 프레임을 리스트에 저장
                        foreach (BitmapFrame frame in currentDecoder.Frames)
                        {
                            tiffFrames.Add(frame);
                        }

                        currentFrameIndex = 0;

                        if (tiffFrames.Count > 0) // 프레임이 있으면 첫 번째 프레임 표시
                            {
                            
                            defectImage.Source = tiffFrames[currentFrameIndex];
                            
                            // 프레임 개수에 따라 UI 업데이트 (페이지 표시 등)
                            UpdateFrameNavigationUI();
                        }
                        else
                        {
                            defectImage.Source = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"이미지 로드 오류: {ex.Message}");
                        defectImage.Source = null;
                    }
                }
                else
                {
                    Console.WriteLine($"이미지 파일이 존재하지 않음: {imagePath}");
                    defectImage.Source = null;
                    tiffFrames.Clear();
                    UpdateFrameNavigationUI();
                }
        }

        private void LoadDefectImageFromWholeSelected(int currentWholeWaferIndex)
        { 
            defectImage.Source = tiffFrames[currentWholeWaferIndex];
        }

        private void LoadDefectImageFromChipOnSelected(int defectId)
        {
           
            if (defectId >= 0 && defectId < tiffFrames.Count)
            {
                defectImage.Source = tiffFrames[defectId];
            }
            else
            {
                Console.WriteLine($"유효하지 않은 defectId: {defectId}");
            }
        }

        // 이미지 탐색 UI 업데이트 메서드
        private void UpdateFrameNavigationUI()
        {
            // 여기서 페이지 수 표시 등의 UI 업데이트
            // 예: pageInfoTextBlock.Text = $"페이지 {currentFrameIndex + 1}/{tiffFrames.Count}";

            // 페이지 네비게이션 버튼 활성화/비활성화 설정
            // 예: prevButton.IsEnabled = (currentFrameIndex > 0);
            // 예: nextButton.IsEnabled = (currentFrameIndex < tiffFrames.Count - 1);
        } 
    }
}