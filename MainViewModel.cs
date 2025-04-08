using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DefectViewProgram
{
    // 두 개의 뷰모델을 포함하는 MainViewModel 생성
    public class MainViewModel : BaseViewModel
    {
        public TiffImageLoaderViewModel tiffLoaderViewModel { get; set; }
        public FileOpenViewModel fileOpenViewModel { get; set; }
        public DefectControlViewModel defectControlViewModel { get; set; }
        public WaferMapViewModel waferMapViewModel { get; set; }


        public MainViewModel()
        {
            tiffLoaderViewModel = new TiffImageLoaderViewModel();
            fileOpenViewModel = new FileOpenViewModel();
            fileOpenViewModel.MainViewModel = this;
            defectControlViewModel = new DefectControlViewModel(this);
            waferMapViewModel = new WaferMapViewModel(this);

            fileOpenViewModel.FileSelected += folderPath => {
                tiffLoaderViewModel.LoadTiffImage(folderPath);
            };    
        }
    }
}