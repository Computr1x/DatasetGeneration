using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DataSetGen.Utils
{
    internal class BitMaskConverter
    {
        public static Mat ConvertToBitMask(Image<Rgba32> image, Color maskColor)
        {
            var mask = Mat.Zeros(image.Height, image.Width, DepthType.Cv8U, 1);

            var maskPixel = maskColor.ToPixel<Rgba32>();
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    // pixelRow.Length has the same value as accessor.Width,
                    // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgba32 pixel = ref pixelRow[x];
                        if (pixel == maskPixel)
                            mask.SetValue(y, x, byte.MaxValue);
                    }
                }
            });

            return mask;
        }

        public static System.Drawing.Point[][] FindContours(Image<Rgba32> image, Color maskColor)
        {
            var mask = ConvertToBitMask(image, maskColor);
            var contours = FindContours(mask);
            return contours.ToArrayOfArray();
        }

        private static VectorOfVectorOfPoint FindContours(Mat mask)
        {
            //mask.ToImage<Emgu.CV.Structure.Gray, byte>().Save($"./Results/mask{i}.jpeg");
            //File.WriteAllBytes(, maskContour);
            var contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(mask, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxNone);

            // filter
            var newContours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            for (int i = 0; i < contours.Size; i++)
            {
                if (contours[i].Size > 100)
                    newContours.Push(contours[i]);
            }

            Mat newMat = Mat.Zeros(mask.Rows, mask.Cols, DepthType.Cv8U, 1);
            CvInvoke.DrawContours(newMat, newContours, -1, new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            //CvInvoke.Imshow("test", newMat);
            //CvInvoke.WaitKey();

            //var imageContour = newMat.ToImage<Bgr, Byte>().ToJpegData();
            //File.WriteAllBytes($"./Results/contour{i++}.jpeg", imageContour);
            return newContours;
        }
    }
}
