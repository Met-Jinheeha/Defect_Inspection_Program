using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    // 두 개의 뷰모델을 포함하는 MainViewModel 생성
    public class MainViewModel
    {
        public TiffImageLoaderViewModel TiffLoader { get; set; }
        public FileOpenViewModel fileOpenViewModel { get; set; }

        public MainViewModel()
        {
            TiffLoader = new TiffImageLoaderViewModel();
            fileOpenViewModel = new FileOpenViewModel();
        }
    }
}

