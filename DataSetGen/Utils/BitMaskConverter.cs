using Emgu.CV;
using Emgu.CV.CvEnum;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGen.Utils
{
    internal class BitMaskConverter
    {
        public static bool TryConvertToBitMask(Image<Rgba32> image, Color maskColor, out Mat? mask)
        {
            mask = null;
            if (!image.DangerousTryGetSinglePixelMemory(out var memory))
                return false;

            mask = new Mat(image.Height, image.Width, DepthType.Cv8U, 1);
            var span = memory.Span;
            var maskPixel = maskColor.ToPixel<Rgba32>();

            int pos;
            int ypos;
            for(int i = 0; i < image.Height; i++)
            {
                ypos = i * image.Width;
                for (int j = 0; j < image.Width; j++)
                {
                    pos = ypos + j;
                    if (span[pos] == maskPixel)
                        mask.SetValue(i, j, byte.MaxValue);
                }
            }
            return true;
        }

        public static void FindContours(Image<Rgba32> image, Color maskColor)
        {
            if(TryConvertToBitMask(image, maskColor, out var mask))
            {
                FindContours(mask!);
            }
        }

        public static void FindContours(Mat mat)
        {
            //CvInvoke.Imshow("test", mat);
            //CvInvoke.WaitKey();
            var contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(mat, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

            // filter
            var newContours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i].Size > 100)
                    newContours.Push(contours[i]);
            }

            Mat newMat = new(mat.Rows, mat.Cols, DepthType.Cv8U, 1);
            CvInvoke.DrawContours(newMat, newContours, -1, new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            CvInvoke.Imshow("test", newMat);
            CvInvoke.WaitKey();
            //CvInvoke.DestroyAllWindows();
            Console.WriteLine();
        }
    }
}
