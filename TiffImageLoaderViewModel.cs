using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DefectViewProgram
{
    public class TiffImageLoaderViewModel : BaseViewModel
    {

        private BitmapSource currentImage;
        public BitmapSource CurrentImage
        {
            get { return currentImage; }
            set { SetProperty(ref currentImage, value); } // 이미지 바뀔때 SetProperty 호출 
        }

        /// <summary>
        ///  Tiff 이미지 Loader
        /// </summary>

        private TiffBitmapDecoder currentDecoder;
        private List<BitmapSource> tiffFrames = new List<BitmapSource>();
        private int currentFrameIndex = 0;

        public void LoadTiffImage(string CurrentFolderPath)
        {

            // ID로 tif 파일 경로 생성
            string imagePath = Path.Combine(CurrentFolderPath, $"Klarf Format.tif");

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

                        CurrentImage = tiffFrames[currentFrameIndex];
                        Console.WriteLine($"이미지 크기: {tiffFrames[currentFrameIndex].PixelWidth}x{tiffFrames[currentFrameIndex].PixelHeight}");
                    }
                    else
                    {
                        CurrentImage = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"이미지 로드 오류: {ex.Message}");
                    CurrentImage = null;
                }
            }
            else
            {
                Console.WriteLine($"이미지 파일이 존재하지 않음: {imagePath}");
                CurrentImage = null;
                tiffFrames.Clear();
            }
            Console.WriteLine($"CurrentImage 설정됨: {CurrentImage != null}");
        }


        public void LoadDefectImageFromWholeSelected(int currentWholeWaferIndex) // 전체 디펙 이미지 보여주기
        {

            CurrentImage = tiffFrames[currentWholeWaferIndex];
        }

        public void LoadDefectImageFromChipOnSelected(int defectId) // 칩 위의 디펙이미지 보여주기
        {

            if (defectId >= 0 && defectId < tiffFrames.Count)
            {
                CurrentImage = tiffFrames[defectId];
            }
            else
            {
                Console.WriteLine($"유효하지 않은 defectId: {defectId}");
            }
        }
    }
}
