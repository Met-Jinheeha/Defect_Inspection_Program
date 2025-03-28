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
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                string selectedFileName = ListBox.SelectedItem.ToString();
                FullPath = Path.Combine(currentFolderPath, selectedFileName);

                Console.WriteLine(FullPath);

                KlarfFileParser parser = new KlarfFileParser();

                ChipInfo chip = new ChipInfo();

                parser.ParseText(FullPath);

                string defectInfo = chip.GetAllDefects();

                ObservableCollection<object> items = new ObservableCollection<object>();

                string[] lines = defectInfo.Split('\n');

                //Console.WriteLine("라인" + lines[1]);


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

                //currentFrameIndex = 0;

                Console.WriteLine(items);
            }
        }

        // 이벤트 핸들러
        //private void NextDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (crtDefectIdxOnChip < chipInfo.DefectList.Count - 1)
        //    {
        //        crtDefectIdxOnChip++;
        //        UpdateUI();
        //    }
        //}

        //private void PreviousDefectOnChipButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (crtDefectIdxOnChip > 0)
        //    {
        //        crtDefectIdxOnChip--;
        //        UpdateUI();
        //    }
        //}

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
            
            for(int i = 0; i < defectInfo.Count; i++)
            {
                items.Add(new
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

            defectList.ItemsSource = items;

            Console.WriteLine(items);
        }

        int currentWaferIndex = 0;
       
        private void NextDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Next");

            currentWaferIndex++;
            int count = defectList.Items.Count;

            defectList.SelectedIndex = currentWaferIndex;

            txtDefectOnWafer.Text =  $"전체 디펙: {currentWaferIndex + 1}/{count}";

            LoadDefectImageFromSelected();
        }

        private void PreviousDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Previous");

            currentWaferIndex--;
            int count = defectList.Items.Count;

            defectList.SelectedIndex = currentWaferIndex;

            txtDefectOnWafer.Text = $"전체 디펙: {currentWaferIndex + 1}/{count}";

            LoadDefectImageFromSelected();
        }


        //// UI 업데이트 통합 메서드
        //private void UpdateUI()
        //{
        //    // 데이터그리드 업데이트
        //    defectList.ItemsSource = chipInfo.DefectList;
        //    defectList.SelectedIndex = crtDefectIdxOnChip;

        //    // 카운터 텍스트 업데이트
        //    txtDefectOnChip.Text = $"칩 내 디펙: {crtDefectIdxOnChip + 1}/{chipInfo.DefectList.Count}";
        //    txtDefectOnWafer.Text = $"웨이퍼 내 디펙: {crtDefectIdxOnWafer + 1}/{waferInfo.TotalDefects}";

        //    // 이미지 업데이트
        //    LoadDefectImage();
        //}



        //private void UpdateCurrentChipFromWaferDefect()
        //{
        //    // 웨이퍼 인덱스에 해당하는 칩으로 이동
        //    Point defectChipIndex = waferInfo.GetChipIndexFromDefectId(crtDefectIdxOnWafer);
        //    currentChipIndex = defectChipIndex;

        //    // 해당 칩에서의 결함 인덱스 찾기
        //    chipInfo = waferInfo.GetChipInfo(currentChipIndex);
        //    crtDefectIdxOnChip = chipInfo.GetLocalDefectIndex(crtDefectIdxOnWafer);
        //}

        private TiffBitmapDecoder currentDecoder;
        private List<BitmapSource> tiffFrames = new List<BitmapSource>();
        private int currentFrameIndex = 0;

        private void LoadDefectImageFromSelected()
        {
            if (defectList.SelectedItem != null)
            {
                // 동적 객체에서 id 속성 가져오기
                dynamic selectedItem = defectList.SelectedItem;
                string defectId = selectedItem.id;

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

                        // 프레임이 있으면 첫 번째 프레임 표시
                        if (tiffFrames.Count > 0)
                        {
                            currentFrameIndex++;
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