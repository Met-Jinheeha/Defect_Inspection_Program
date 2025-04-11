using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

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
            this.WindowState = WindowState.Maximized;

            // MainViewModel 인스턴스를 생성해 DataContext에 할당
            var mainViewModel = new MainViewModel();
            DataContext = mainViewModel;

            // FileOpenViewModel에 UI 요소 연결
            mainViewModel.fileOpenViewModel.FolderTreeView = this.FolderTreeView;
            mainViewModel.fileOpenViewModel.FileListBox = this.FileListBox;

            mainViewModel.waferMapViewModel.SetCanvas(this.WaferCanvas);
        }

        // Define reasonable zoom limits
        private const double MinScale = 0.5; // Example: Minimum 50% zoom
        private const double MaxScale = 5.0; // Example: Maximum 500% zoom

        private void MouseWheelZoomInOut(object sender, MouseWheelEventArgs e)
        {
            var image = CurrentImage; // XAML의 Image 이름
            var container = ImageContainer; // XAML의 Border 이름

            // 필요한 요소들이 모두 준비되었는지 확인
            if (image == null || container == null || myScaleTransform == null || myTranslateTransform == null || image.Source == null)
                return;

            // --- 1. 스케일 계산 ---
            double currentScale = myScaleTransform.ScaleX; // X, Y 스케일이 같다고 가정
            double zoomFactor = e.Delta > 0 ? 1.15 : 1 / 1.15; // 확대/축소 비율 (조정 가능)
            double newScale = currentScale * zoomFactor;

            // 스케일 한계 적용
            newScale = Math.Max(MinScale, Math.Min(MaxScale, newScale));

            // 스케일 변화가 거의 없으면 중단
            if (Math.Abs(newScale - currentScale) < 0.001)
                return;

            // --- 2. 마우스 위치 (컨테이너 기준) ---
            Point mousePosInContainer = e.GetPosition(container);

            // --- 3. 이전 Translate 값 ---
            double currentTranslateX = myTranslateTransform.X;
            double currentTranslateY = myTranslateTransform.Y;

            // --- 4. 새 Translate 값 계산 (마우스 위치 고정) ---
            // 공식: T_new = P_container - ((P_container - T_old) * (S_new / S_old))
            // 이 공식은 (0,0) 기준 스케일링 시 마우스 포인터 위치(P_container)를
            // 화면상에 고정시키기 위한 새로운 Translate 값(T_new)을 계산합니다.
            double newTranslateX = mousePosInContainer.X - ((mousePosInContainer.X - currentTranslateX) * (newScale / currentScale));
            double newTranslateY = mousePosInContainer.Y - ((mousePosInContainer.Y - currentTranslateY) * (newScale / currentScale));


            // --- 6. 변환 적용 ---
            myScaleTransform.ScaleX = newScale;
            myScaleTransform.ScaleY = newScale;
            myTranslateTransform.X = newTranslateX;
            myTranslateTransform.Y = newTranslateY;

            // 이벤트 처리 완료
            e.Handled = true;
        }
    }
}