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
        public static bool TryConvertToBitMask(Image<Rgba32> image, Color maskColor)
        {
            if (image.DangerousTryGetSinglePixelMemory(out var memory))
                return false;
            var bitmask = new Mat(image.Width, image.Height, DepthType.Cv8U, 1);
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
                    if (span[i] == maskPixel)
                        bitmask.
                }
            }


            //var contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            //var hierarchy = new Mat();
            //CvInvoke.FindContours(outputImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
        }
    }
}
