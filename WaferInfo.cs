using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    class WaferInfo
    {
        private string dateTime;
        public string DateTime
        {
            get 
            {
                string[] tempArr = dateTime.Split(' ');
                dateTime = tempArr[1] + ' ' + tempArr[2];
                return RemoveLastChar(dateTime);
            }
            set => dateTime = value;
        }

        private string sampleType;
        public string SampleType
        {
            get
            {
                string[] tempArr = sampleType.Split(' ');
                sampleType = RemoveLastChar(tempArr[1]);
                return sampleType;
            }
            set => sampleType = value;
        }

        private string sampleSize;
        public string SampleSize
        {
            get
            {
                string[] tempArr = sampleSize.Split(' ');
                sampleSize = RemoveLastChar(tempArr[2]);
                return sampleSize;
            }
            set => sampleSize = value;
        }

        private string lotId;
        public string LotId
        {
            get
            {
                string[] tempArr = lotId.Split(' ');
                lotId = RemoveLastChar(tempArr[1]);
                return lotId;
            }
            set => lotId = value;
        }

        private string stepId;
        public string StepId
        {
            get
            {
                string[] tempArr = stepId.Split(' ');
                stepId = RemoveLastChar(tempArr[1]);
                return stepId;
            }
            set => stepId = value;
        }

        private string sampleOrientationMarkType;
        public string SampleOrientationMarkType
        {
            get
            {
                string[] tempArr = sampleOrientationMarkType.Split(' ');
                sampleOrientationMarkType = RemoveLastChar(tempArr[1]);
                return sampleOrientationMarkType;
            }
            set => sampleOrientationMarkType = value;
        }

        private string orientationMarkLocation;
        public string OrientationMarkLocation
        {
            get
            {
                string[] tempArr = orientationMarkLocation.Split(' ');
                orientationMarkLocation = RemoveLastChar(tempArr[1]);
                return orientationMarkLocation;
            }
            set => orientationMarkLocation = value;
        }

        private string waferId;
        public string WaferId
        {
            get
            {
                string[] tempArr = waferId.Split(' ');
                waferId = RemoveLastChar(tempArr[1]);
                return waferId;
            }
            set => waferId = value;
        }

        private string slotId;
        public string SlotId
        {
            get
            {
                string[] tempArr = slotId.Split(' ');
                slotId = RemoveLastChar(tempArr[1]);
                return slotId;
            }
            set => slotId = value;
        }

        private string Content
        {
            get; set;
        }


        public WaferInfo(string[] waferInfoContent)
        {
            DateTime = waferInfoContent[0];
            SampleType = waferInfoContent[1];
            LotId = waferInfoContent[2];
            SampleSize = waferInfoContent[3];
            StepId = waferInfoContent[4];
            SampleOrientationMarkType = waferInfoContent[5];
            OrientationMarkLocation = waferInfoContent[6];
            WaferId = waferInfoContent[7];
            SlotId = waferInfoContent[8];
        }

        public static string RemoveLastChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return str.Substring(0, str.Length - 1);
        }


        public override string ToString()
        {
            Content = $"Time: {DateTime}\n" +
                          $"SampleType: {SampleType}\n" +
                          $"SampleSize: {SampleSize}\n" +
                          $"LotId: {LotId}\n" +
                          $"StepId: {StepId}\n" +
                          $"WaferMarkType: {SampleOrientationMarkType}\n" +
                          $"MarkPosition: {OrientationMarkLocation}\n" +
                          $"WaferId: {WaferId}\n" +
                          $"SlotId: {SlotId}";
            return Content;
        }
    }
}
