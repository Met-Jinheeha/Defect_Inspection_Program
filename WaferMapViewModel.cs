using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Input;

namespace DefectViewProgram
{
    public class WaferMapViewModel : BaseViewModel
    {
        private Canvas waferCanvas;

        private ChipInfo chipInfo;
        private MainViewModel mainViewModel;

        // 현재 WaferInfo 저장
        private WaferInfo currentWaferInfo = null;

        public void SetChipInfo(ChipInfo chipInfo)
        {
            this.chipInfo = chipInfo;
        }

        public WaferMapViewModel(MainViewModel mainVM)
        {
            this.mainViewModel = mainVM;
        }

        public void SetCanvas(Canvas canvas)
        {
            this.waferCanvas = canvas;
        }

        private int selectedDefectCount;
        public int SelectedDefectCount
        {
            get => selectedDefectCount;
            set => selectedDefectCount = value;

        }

        private bool isChipSelect;
        public bool IsChipSelect
        {
            get => isChipSelect;
            set => isChipSelect = value;

        }

        // 현재 선택된 디펙트 좌표
        private (int, int)? selectedDefectCoord = null;

        // 색상 정의
        private static readonly SolidColorBrush NormalBrush = Brushes.Green;
        private static readonly SolidColorBrush DefectBrush = Brushes.Red;
        private static readonly SolidColorBrush SelectedBrush = Brushes.Black;
        private static readonly SolidColorBrush BorderBrush = Brushes.Black;

        // 선택된 디펙트 좌표 설정하는 메서드
        public void SetSelectedDefect((int, int) coord)
        {
            selectedDefectCoord = coord;
            // 웨이퍼 맵 다시 그리기 (선택된 셀 반영)
            if (waferCanvas != null)
                DrawWaferMap(this.currentWaferInfo);
        }


        private List<DefectInfo> cellDefects;
        public  List<DefectInfo> CellDefect
        {
            get; set;
        }

        private int xSelectedIndex;
        public int XSelectedIndex
        {
            get => xSelectedIndex;
            set => xSelectedIndex = value;

        }

        private int ySelectedIndex;
        public int YSelectedIndex
        {
            get => ySelectedIndex;
            set => ySelectedIndex = value;

        }

        // 셀 클릭 커맨드
        private ICommand cellClickCommand;
        public ICommand CellClickCommand
        {
            get
            {
                if (cellClickCommand == null)
                {
                    cellClickCommand = new RelayCommand<object>(ExecuteCellClick);
                }
                return cellClickCommand;
            }
        }

        /// <summary>
        /// 웨이퍼 맵에서 그리드를 눌렀을 때 작동
        /// </summary>
        /// 
        public void ExecuteCellClick(object parameter)
        {
            IsChipSelect = true;

            if (parameter is ValueTuple<int, int> clickedCoord)
            {
                XSelectedIndex = clickedCoord.Item1;
                YSelectedIndex = clickedCoord.Item2;

                // 해당 셀의 디펙트 목록 가져오기
                if (chipInfo != null)
                {
                    CellDefect = chipInfo.GetDefects(XSelectedIndex, YSelectedIndex);

                    // FileOpenViewModel의 DefectList 업데이트
                    if (mainViewModel?.fileOpenViewModel != null)
                    {
                        mainViewModel.fileOpenViewModel.DefectList.Clear();
                        foreach (var defect in CellDefect)
                        {
                            mainViewModel.fileOpenViewModel.DefectList.Add(defect);
                        }

                        int SelectedDefectCount;

                        if (CellDefect.Count == 0)
                        {
                            SelectedDefectCount = -1;
                        }
                        else
                        {
                            SelectedDefectCount = 0;
                        }

                        mainViewModel.defectControlViewModel.ChipSelectedIndex = 0;
                        mainViewModel.defectControlViewModel.SelectedIndex = 0;
                        mainViewModel.fileOpenViewModel.SelectedDefectIndex = 0;

                        if (mainViewModel.fileOpenViewModel.DefectList.Count > 0)
                        {
                            mainViewModel.tiffLoaderViewModel.LoadDefectImageFromWholeSelected(
                                mainViewModel.fileOpenViewModel.DefectList[0].DefectId - 1);
                        }
                        else
                        {
                            mainViewModel.tiffLoaderViewModel.CurrentImage = null;
                        }

                        var coord = (XSelectedIndex, YSelectedIndex);
                        SetSelectedDefect(coord);

                        mainViewModel.fileOpenViewModel.TextDefectOnChip =
                            $"Cell ({XSelectedIndex}, {YSelectedIndex}) Defect: {SelectedDefectCount + 1}/{CellDefect.Count}";
                    }
                }
            }
        }

        public void DrawWaferMap(WaferInfo waferInfo)
        {

            // null 체크 추가
            if (waferInfo == null || waferInfo.chipInfoList == null)
            {
                Console.WriteLine("waferInfo 또는 chipInfoList가 null입니다.");
                return;
            }
            // 현재 정보 저장
            this.currentWaferInfo = waferInfo;
            waferCanvas.Children.Clear();

            // 웨이퍼 크기 설정
            double waferDiameter = Math.Min(waferCanvas.ActualWidth, waferCanvas.ActualHeight) * 0.8;
            double waferRadius = waferDiameter / 2;

            // 캔버스 중앙 좌표
            double centerX = waferCanvas.ActualWidth / 2;
            double centerY = waferCanvas.ActualHeight / 2;


            // 좌표 범위 계산
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var key in waferInfo.chipInfoList.Keys)
            {
                minX = Math.Min(minX, key.Item1);
                minY = Math.Min(minY, key.Item2);
                maxX = Math.Max(maxX, key.Item1);
                maxY = Math.Max(maxY, key.Item2);
            }

            // 전체 범위 계산
            int rangeX = maxX - minX;
            int rangeY = maxY - minY;

            // 셀 크기 계산 (작게 설정)
            double maxRange = Math.Max(rangeX, rangeY);
            double cellSize = (waferDiameter * 0.9) / (maxRange + 4);
            double cellSizeX = (waferDiameter * 2.2) / (maxRange + 4);
            double cellSizeY = (waferDiameter * 0.9) / (maxRange + 4);

            // 오프셋 계산
            double offsetX = centerX - ((rangeX * cellSizeX) / 2);
            double offsetY = centerY - ((rangeY * cellSizeY) / 2);

            // 모든 칩 정보 그리기
            foreach (var kvp in waferInfo.chipInfoList)
            {
                var coord = kvp.Key;
                string status = kvp.Value;

                // 좌표를 캔버스 좌표로 변환
                double x = ((coord.Item1 - minX) * cellSizeX) + offsetX;
                double y = ((coord.Item2 - minY) * cellSizeY) + offsetY;

                // 셀의 중심 계산
                double cellCenterX = x + cellSizeX / 2;
                double cellCenterY = y + cellSizeY / 2;

                // 웨이퍼 중심에서의 거리 계산
                double dx = cellCenterX - centerX;
                double dy = cellCenterY - centerY;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // 원형 웨이퍼 안에 있는 칩만 그리기
                if (distance <= waferRadius - Math.Max(cellSizeX, cellSizeY) / 2)
                {
                    // 색상 결정: 선택된 디펙트인 경우 검정색, 아니면 원래 색상
                    SolidColorBrush fillBrush;

                    if (selectedDefectCoord.HasValue && selectedDefectCoord.Value.Equals(coord))
                    {
                        fillBrush = SelectedBrush; // 선택된 디펙트는 검정색
                    }
                    else
                    {
                        fillBrush = (status == "X") ? DefectBrush : NormalBrush; // X가 불량칩
                    }

                    var rect = new Rectangle
                    {
                        Width = cellSizeX,
                        Height = cellSizeY,
                        Fill = fillBrush,
                        Stroke = BorderBrush,
                        StrokeThickness = 0.5,
                        Tag = coord // 좌표 정보 저장
                    };

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                    waferCanvas.Children.Add(rect);

                    // 클릭 이벤트를 CommandBinding으로 처리
                    rect.InputBindings.Add(new MouseBinding(
                        CellClickCommand,
                        new MouseGesture(MouseAction.LeftClick))
                    {
                        CommandParameter = coord
                    });

                    // 툴팁 추가
                    ToolTip tooltip = new ToolTip();
                    tooltip.Content = $"좌표: ({coord.Item1}, {coord.Item2}), 상태: {status}";
                    rect.ToolTip = tooltip;
                }
            }
        }
    }
}


