using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DefectViewProgram
{
    public class DefectControlViewModel : BaseViewModel
    {

        // 메인 뷰모델 참조
        private MainViewModel mainViewModel;

        // 생성자에서 MainViewModel 참조 받기
        public DefectControlViewModel(MainViewModel mainVM)
        {
            this.mainViewModel = mainVM;
        }

        int currentWaferIndex = 0;

        public void MoveToNextDefectOnWafer()
        {

            if (mainViewModel.fileOpenViewModel.IsSelectedKlarfFile)
            {
                currentWaferIndex = 0;
                mainViewModel.fileOpenViewModel.IsSelectedKlarfFile = false;
                mainViewModel.fileOpenViewModel.SelectedDefectIndex = 0;
            }
          
            if (mainViewModel.fileOpenViewModel.DefectList.Count == 0) return;

            currentWaferIndex++;

            if (currentWaferIndex >= mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                currentWaferIndex = 0;
                return;
            }

            mainViewModel.fileOpenViewModel.SelectedDefectIndex = currentWaferIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnWafer = $"전체 디펙: {currentWaferIndex+1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

            // 메인 뷰모델을 통해 TiffImageLoaderViewModel 접근
            if (mainViewModel != null && mainViewModel.tiffLoaderViewModel != null)
            {
                // 디펙트 ID 기반으로 이미지 로드
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(currentWaferIndex);
            }
        }

        private ICommand moveToNextDefectOnWaferCommand;
        public ICommand MoveToNextDefectOnWaferCommand
        {
            get
            {
                if (moveToNextDefectOnWaferCommand == null)
                {
                    moveToNextDefectOnWaferCommand = new RelayCommand(MoveToNextDefectOnWafer);
                }
                return moveToNextDefectOnWaferCommand;
            }
        }

        public void MoveToPreviousDefectOnWafer()
        {

            if (mainViewModel.fileOpenViewModel.IsSelectedKlarfFile)
            {
                currentWaferIndex = 0;
                mainViewModel.fileOpenViewModel.IsSelectedKlarfFile = false;
                mainViewModel.fileOpenViewModel.SelectedDefectIndex = 0;
            }

            if (mainViewModel.fileOpenViewModel.DefectList.Count == 0) return;

            currentWaferIndex--;

            if (currentWaferIndex < 0)
            {
                currentWaferIndex = 0;
                return;
            }


            mainViewModel.fileOpenViewModel.SelectedDefectIndex = currentWaferIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnWafer = $"전체 디펙: {currentWaferIndex+1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

            // 메인 뷰모델을 통해 TiffImageLoaderViewModel 접근
            if (mainViewModel != null && mainViewModel.tiffLoaderViewModel != null)
            {
                // 디펙트 ID 기반으로 이미지 로드
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(currentWaferIndex);
            }
        }
       
        private ICommand moveToPreviousDefectOnWaferCommand;
        public ICommand MoveToPreviousDefectOnWaferCommand
        {
            get
            {
                if (moveToPreviousDefectOnWaferCommand == null)
                {
                    moveToPreviousDefectOnWaferCommand = new RelayCommand(MoveToPreviousDefectOnWafer);
                }
                return moveToPreviousDefectOnWaferCommand;
            }
        }

        // DefectControlViewModel에 추가
        public void OnDefectListSelectionChanged()
        {
            int selectedIndex = mainViewModel.fileOpenViewModel.SelectedDefectIndex;
            if (selectedIndex >= 0 && selectedIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                currentWaferIndex = selectedIndex;
                mainViewModel.fileOpenViewModel.TextDefectOnWafer =
                    $"전체 디펙: {selectedIndex + 1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(selectedIndex);
            }
        }

        private ICommand defectListSelectionChangedCommand;
        public ICommand DefectListSelectionChangedCommand
        {
            get
            {
                if (defectListSelectionChangedCommand == null)
                {
                    defectListSelectionChangedCommand = new RelayCommand(OnDefectListSelectionChanged);
                }
                return defectListSelectionChangedCommand;
            }
        }




















        //public void PreviousDefectOnWholeWaferButton_Click(object sender, RoutedEventArgs e)
        //{

        //    //if (isChipDataView)
        //    //{
        //    //    return;
        //    //}

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
