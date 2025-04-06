using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DefectViewProgram
{
    public class WaferMapViewModel : BaseViewModel
    {
        private Canvas waferCanvas;

        // 칩 셀의 크기
        private const int CellSize = 10;

        // 색상 정의
        private static readonly SolidColorBrush NormalBrush = Brushes.Green;
        private static readonly SolidColorBrush DefectBrush = Brushes.Red;
        private static readonly SolidColorBrush BorderBrush = Brushes.Black;

        public WaferMapViewModel()
        {
        }

        public void SetCanvas(Canvas canvas)
        {
            this.waferCanvas = canvas;
        }


        //public void DrawWaferMap(string waferInfo)
        //{
        //    if (waferCanvas == null) return;
        //    waferCanvas.Children.Clear();

        //    int cellSize = 20;
        //    int centerX = (int)(waferCanvas.Width / 2 / cellSize);
        //    int centerY = (int)(waferCanvas.Height / 2 / cellSize);
        //    int radius = Math.Min(centerX, centerY) - 1;

        //    // 웨이퍼 외곽 원 그리기
        //    Ellipse waferOutline = new Ellipse
        //    {
        //        Width = radius * 2 * cellSize,
        //        Height = radius * 2 * cellSize,
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 2,
        //        Fill = Brushes.Transparent
        //    };
        //    Canvas.SetLeft(waferOutline, (centerX - radius) * cellSize);
        //    Canvas.SetTop(waferOutline, (centerY - radius) * cellSize);
        //    waferCanvas.Children.Add(waferOutline);

        //    // 칩 그리기
        //    foreach (Point p in WaferInfo.wholeGridList)
        //    {
        //        // 원형 웨이퍼 내부에 있는지 확인
        //        double distFromCenter = Math.Sqrt(Math.Pow(p.x - centerX, 2) + Math.Pow(p.y - centerY, 2));
        //        if (distFromCenter > radius) continue;

        //        Rectangle rect = new Rectangle
        //        {
        //            Width = cellSize,
        //            Height = cellSize,
        //            Stroke = Brushes.Black,
        //            Fill = ChipInfo.chipDefects.ContainsKey(p) ? Brushes.Red : Brushes.LightGray
        //        };
        //        Canvas.SetLeft(rect, p.x * cellSize);
        //        Canvas.SetTop(rect, p.y * cellSize);
        //        waferCanvas.Children.Add(rect);
        //    }
        //}

        public void DrawWaferMap(string waferInfo)
        {
            waferCanvas.Children.Clear();

            double waferDiameter = 300;
            double waferRadius = waferDiameter / 2;
            double cellSize = 25;

            var wafer = new Ellipse
            {
                Width = waferDiameter,
                Height = waferDiameter,
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            double canvasWidth = waferCanvas.ActualWidth;
            double canvasHeight = waferCanvas.ActualHeight;
            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;

            Canvas.SetLeft(wafer, centerX - waferRadius);
            Canvas.SetTop(wafer, centerY - waferRadius);
            waferCanvas.Children.Add(wafer);

            int cols = (int)(waferDiameter / cellSize);
            int rows = (int)(waferDiameter / cellSize);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    double x = col * cellSize + (centerX - waferRadius);
                    double y = row * cellSize + (centerY - waferRadius);

                    // 셀 중심 좌표
                    double cellCenterX = x + cellSize / 2;
                    double cellCenterY = y + cellSize / 2;

                    // 중심에서 거리
                    double dx = cellCenterX - centerX;
                    double dy = cellCenterY - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= waferRadius)
                    {
                        int gridX = col;
                        int gridY = row;

                        bool isDefect = ChipInfo.chipDefects.ContainsKey(Tuple.Create(gridX, gridY));


                        var rect = new Rectangle
                        {
                            Width = cellSize,
                            Height = cellSize,
                            Fill = isDefect ? Brushes.Red : Brushes.Green,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        };

                        Canvas.SetLeft(rect, x);
                        Canvas.SetTop(rect, y);
                        waferCanvas.Children.Add(rect);
                    }
                }
            }
        }









        //public void DrawWaferMap()
        //{
        //    waferCanvas.Children.Clear();

        //    // 정상 및 비정상 포인트 분류
        //    List<Point> normalPoints = new List<Point>();
        //    List<Point> abnormalPoints = new List<Point>();

        //    foreach (Point point in WaferInfo.wholeGridList)
        //    {
        //        if (ChipInfo.chipDefects.ContainsKey(point))
        //        {
        //            abnormalPoints.Add(point);
        //        }
        //        else
        //        {
        //            normalPoints.Add(point);
        //        }
        //    }

        //    // 좌표 정규화 (캔버스에 맞게 조정)
        //    NormalizeAndDrawPoints(normalPoints, abnormalPoints);
        //}

        private void NormalizeAndDrawPoints(List<Point> normalPoints, List<Point> abnormalPoints)
        {
            // 모든 점을 하나의 리스트로 합치기 (정규화 계산용)
            List<Point> allPoints = new List<Point>();
            allPoints.AddRange(normalPoints);
            allPoints.AddRange(abnormalPoints);

            if (allPoints.Count == 0) return;

            // x, y 좌표의 최소, 최대값 찾기
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (Point p in allPoints)
            {
                minX = Math.Min(minX, p.x);
                minY = Math.Min(minY, p.y);
                maxX = Math.Max(maxX, p.x);
                maxY = Math.Max(maxY, p.y);
            }

            // 스케일 계산 (캔버스 여백 10% 추가)
            double canvasWidth = waferCanvas.Width * 0.8;
            double canvasHeight = waferCanvas.Height * 0.8;
            double xRange = maxX - minX;
            double yRange = maxY - minY;

            // 스케일 결정 (x축, y축 비율 유지)
            double scale = Math.Min(canvasWidth / xRange, canvasHeight / yRange);

            // 정상 포인트 그리기
            foreach (Point p in normalPoints)
            {
                DrawCell(p, minX, minY, scale, NormalBrush);
            }

            // 비정상 포인트 그리기
            foreach (Point p in abnormalPoints)
            {
                DrawCell(p, minX, minY, scale, DefectBrush);
            }
        }

        private void DrawCell(Point point, int minX, int minY, double scale, SolidColorBrush fillBrush)
        {
            // 캔버스 중앙에 위치하도록 오프셋 계산
            double xOffset = (waferCanvas.Width - (scale * (WaferInfo.wholeGridList.Count))) / 2;
            double yOffset = (waferCanvas.Height - (scale * (WaferInfo.wholeGridList.Count))) / 2;

            // 정규화된 위치 계산
            double x = ((point.x - minX) * scale) + xOffset;
            double y = ((point.y - minY) * scale) + yOffset;

            // 사각형 생성
            Rectangle rect = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = fillBrush,
                Stroke = BorderBrush,
                StrokeThickness = 1
            };

            // 캔버스에 추가
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            waferCanvas.Children.Add(rect);

            // 툴팁 추가 (선택 사항)
            ToolTip tooltip = new ToolTip();
            tooltip.Content = $"X: {point.x}, Y: {point.y}";
            rect.ToolTip = tooltip;
        }

    }
}