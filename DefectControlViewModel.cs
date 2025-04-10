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

        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get => selectedIndex;
            set => selectedIndex = value;
        }

        private int chipSelectedIndex = 0;
        public int ChipSelectedIndex
        {
            get => chipSelectedIndex; set => chipSelectedIndex = value;
        }


        public void MoveToNextDefectOnWafer()
        {
            if (mainViewModel.waferMapViewModel.IsChipSelect) return;

            if (mainViewModel.fileOpenViewModel.IsSelectedKlarfFile)
            {
                SelectedIndex = 0;
                mainViewModel.fileOpenViewModel.IsSelectedKlarfFile = false;
                mainViewModel.fileOpenViewModel.SelectedDefectIndex = 0;
            }
          
            if (mainViewModel.fileOpenViewModel.DefectList.Count == 0) return;

            SelectedIndex++;

            if (SelectedIndex >= mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                SelectedIndex = 0;
                return;
            }

            mainViewModel.fileOpenViewModel.SelectedDefectIndex = SelectedIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnWafer = $"Total Defect: {SelectedIndex + 1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

            // 메인 뷰모델을 통해 TiffImageLoaderViewModel 접근
            if (mainViewModel != null && mainViewModel.tiffLoaderViewModel != null)
            {
                // 디펙트 ID 기반으로 이미지 로드
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(SelectedIndex);
                UpdateWaferMapWithSelectedDefect(SelectedIndex);
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
            if (mainViewModel.waferMapViewModel.IsChipSelect) return;

            if (mainViewModel.fileOpenViewModel.IsSelectedKlarfFile)
            {
                SelectedIndex = 0;
                mainViewModel.fileOpenViewModel.IsSelectedKlarfFile = false;
                mainViewModel.fileOpenViewModel.SelectedDefectIndex = 0;
            }

            if (mainViewModel.fileOpenViewModel.DefectList.Count == 0) return;

            SelectedIndex--;

            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
                return;
            }

            mainViewModel.fileOpenViewModel.SelectedDefectIndex = SelectedIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnWafer = $"Total Defect: {SelectedIndex+1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

            // 메인 뷰모델을 통해 TiffImageLoaderViewModel 접근
            if (mainViewModel != null && mainViewModel.tiffLoaderViewModel != null)
            {
                // 디펙트 ID 기반으로 이미지 로드
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(SelectedIndex);
                UpdateWaferMapWithSelectedDefect(SelectedIndex);
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

        // 리스트 버튼을 클릭 -> 칩 선택시에는 선택되면 안됨
        public void OnDefectListSelectionChangedWafer()
        {
            if (mainViewModel.waferMapViewModel.IsChipSelect)
            {
                OnDefectListSelectionChangedChip();
                return;
            }

            if (SelectedIndex >= 0 && SelectedIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                mainViewModel.fileOpenViewModel.TextDefectOnWafer =
                    $"Total Defect: {SelectedIndex + 1}/{mainViewModel.fileOpenViewModel.DefectList.Count}";

                SelectedIndex= mainViewModel.fileOpenViewModel.SelectedDefectIndex;

                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(mainViewModel.fileOpenViewModel.DefectList[SelectedIndex].DefectId - 1);
                UpdateWaferMapWithSelectedDefect(SelectedIndex);
            }
        }


        private ICommand onDefectListSelectionChangedWaferCommand;
        public ICommand OnDefectListSelectionChangedWaferCommand
        {
            get
            {
                if (onDefectListSelectionChangedWaferCommand == null)
                {
                    onDefectListSelectionChangedWaferCommand = new RelayCommand(OnDefectListSelectionChangedWafer);
                }
                return onDefectListSelectionChangedWaferCommand;
            }
        }


        /// <summary>
        /// 리스트에서 디펙 정보를 눌렀을 때
        /// </summary>
        public void OnDefectListSelectionChangedChip()
        {
            SelectedIndex = mainViewModel.fileOpenViewModel.SelectedDefectIndex;

            if (SelectedIndex >= 0 && SelectedIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                mainViewModel.fileOpenViewModel.TextDefectOnChip =
                            $"Cell ({mainViewModel.waferMapViewModel.XSelectedIndex}, {mainViewModel.waferMapViewModel.YSelectedIndex}) Defect: {mainViewModel.fileOpenViewModel.SelectedDefectIndex + 1}/{mainViewModel.waferMapViewModel.CellDefect.Count}";

                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(mainViewModel.fileOpenViewModel.DefectList[SelectedIndex].DefectId - 1);
                UpdateWaferMapWithSelectedDefect(SelectedIndex);
            }
        }


        // Defect 선택 시 웨이퍼 맵 업데이트
        private void UpdateWaferMapWithSelectedDefect(int defectIndex)
        {
            if (defectIndex >= 0 && defectIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
            {
                var defect = mainViewModel.fileOpenViewModel.DefectList[defectIndex];

                // DefectInfo 객체의 XIndex와 YIndex 속성 사용
                if (defect != null)
                {
                    // DefectInfo 클래스의 XIndex, YIndex가 int 타입이라면
                    var coord = (defect.XIndex, defect.YIndex);

                    // 웨이퍼 맵 뷰모델에 선택된 좌표 전달
                    mainViewModel.waferMapViewModel.SetSelectedDefect(coord);
                }
            }
        }

        public void MoveToNextDefectOnChip()
        {
            // 칩이 선택되지 않았으면 실행하지 않음
            if (!mainViewModel.waferMapViewModel.IsChipSelect) return;

            // 유효한 결함 목록 확인
            int defectCount = mainViewModel.waferMapViewModel.CellDefect.Count;
            if (defectCount <= 0) return;

            // 다음 결함으로 이동
            SelectedIndex = (SelectedIndex == -1) ? 0 : SelectedIndex + 1;

            // 인덱스 범위 넘어가면 첫 번째 결함으로 순환
            if (SelectedIndex >= defectCount)
                SelectedIndex = 0;

            // 결함 정보 업데이트
            mainViewModel.fileOpenViewModel.SelectedDefectIndex = SelectedIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnChip =
                $"Cell ({mainViewModel.waferMapViewModel.XSelectedIndex}, {mainViewModel.waferMapViewModel.YSelectedIndex}) " +
                $"Defect: {SelectedIndex + 1}/{defectCount}";

            if (SelectedIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(
                    mainViewModel.fileOpenViewModel.DefectList[SelectedIndex].DefectId - 1);
        }


        private ICommand moveToNextDefectOnChipCommand;
        public ICommand MoveToNextDefectOnChipCommand
        {
            get
            {
                if (moveToNextDefectOnChipCommand == null)
                {
                    moveToNextDefectOnChipCommand = new RelayCommand(MoveToNextDefectOnChip);
                }
                return moveToNextDefectOnChipCommand;
            }
        }

        public void MoveToPreviousDefectOnChip()
        {
            // 칩이 선택되지 않았으면 실행하지 않음
            if (!mainViewModel.waferMapViewModel.IsChipSelect) return;

            // 유효한 결함 목록 확인
            int defectCount = mainViewModel.waferMapViewModel.CellDefect.Count;
            if (defectCount <= 0) return;

            if (SelectedIndex <= 0)
                SelectedIndex = defectCount - 1;  // 첫 번째에서 마지막으로 순환
            else
                SelectedIndex--;

            // 인덱스가 범위를 넘어가면 첫 번째 결함으로 순환
            if (SelectedIndex <= 0)
                SelectedIndex = 0;

            // 결함 정보 업데이트
            mainViewModel.fileOpenViewModel.SelectedDefectIndex = SelectedIndex;

            mainViewModel.fileOpenViewModel.TextDefectOnChip =
                $"Cell ({mainViewModel.waferMapViewModel.XSelectedIndex}, {mainViewModel.waferMapViewModel.YSelectedIndex}) " +
                $"Defect: {SelectedIndex+1}/{defectCount}";

            if (SelectedIndex < mainViewModel.fileOpenViewModel.DefectList.Count)
                mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(
                    mainViewModel.fileOpenViewModel.DefectList[SelectedIndex].DefectId - 1);
        }


        private ICommand moveToPreviousDefectOnChipCommand;
        public ICommand MoveToPreviousDefectOnChipCommand
        {
            get
            {
                if (moveToPreviousDefectOnChipCommand == null)
                {
                    moveToPreviousDefectOnChipCommand = new RelayCommand(MoveToPreviousDefectOnChip);
                }
                return moveToPreviousDefectOnChipCommand;
            }
        }
    }
}
