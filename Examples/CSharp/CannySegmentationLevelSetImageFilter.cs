/*=========================================================================
*
*  Copyright Insight Software Consortium
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*         http://www.apache.org/licenses/LICENSE-2.0.txt
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*=========================================================================*/

using System;
using System.Runtime.InteropServices;

using itk.simple;
using PixelId = itk.simple.PixelIDValueEnum;
using SitkImage = itk.simple.Image;

namespace itk.simple.examples
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 8)
            {
                Console.WriteLine("Missing Parameters ");
                Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + 
                                  "inputImage initialModel outputImage cannyThreshold " +
                                  "cannyVariance advectionWeight initialModelIsovalue maximumIterations ");
                return;
            }

            // Read input image

            SitkImage inputImage = SimpleITK.ReadImage(args[0], PixelId.sitkFloat32);
            SitkImage initialModel = SimpleITK.ReadImage(args[1], PixelId.sitkFloat32);

            //  The input image will be processed with a few iterations of
            //  feature-preserving diffusion.  We create a filter and set the
            //  appropriate parameters.

            GradientAnisotropicDiffusionImageFilter diffusion=new GradientAnisotropicDiffusionImageFilter(); 
            diffusion.SetConductanceParameter(1.0);
            diffusion.SetTimeStep(0.125);
            diffusion.SetNumberOfIterations(5);
            SitkImage diffusedImage=diffusion.Execute(inputImage);
               
            // As with the other ITK level set segmentation filters, the terms of the
            // CannySegmentationLevelSetImageFilter level set equation can be
            // weighted by scalars.  For this application we will modify the relative
            // weight of the advection term.  The propagation and curvature term weights
            // are set to their defaults of 0 and 1, respectively.

            CannySegmentationLevelSetImageFilter cannySegmentation = new CannySegmentationLevelSetImageFilter();
            cannySegmentation.SetAdvectionScaling(Convert.ToDouble(args[5]));
            cannySegmentation.SetCurvatureScaling(1.0);
            cannySegmentation.SetPropagationScaling(1.0);

            //  The maximum number of iterations is specified from the command line.
            //  It may not be desirable in some applications to run the filter to
            //  convergence.  Only a few iterations may be required.
           
            cannySegmentation.SetMaximumRMSError(0.01);
            cannySegmentation.SetNumberOfIterations(Convert.ToUInt32(args[7]));

            //  There are two important parameters in the
            //  CannySegmentationLevelSetImageFilter to control the behavior of the
            //  Canny edge detection.  The variance parameter controls the
            //  amount of Gaussian smoothing on the input image.  The threshold
            //  parameter indicates the lowest allowed value in the output image.
            //  Thresholding is used to suppress Canny edges whose gradient magnitudes
            //  fall below a certain value.

            cannySegmentation.SetThreshold(Convert.ToUInt32(args[3]));
            cannySegmentation.SetVariance(Convert.ToDouble(args[4]));

            // Finally, it is very important to specify the isovalue of the surface in
            // the initial model input image. In a binary image, for example, the
            // isosurface is found midway between the foreground and background values.

            cannySegmentation.SetIsoSurfaceValue(Convert.ToDouble(args[6]));

         
            SitkImage segResult=cannySegmentation.Execute(diffusedImage, initialModel);

            SitkImage threshSegResult=SimpleITK.BinaryThreshold(segResult, 0.0, 10.0, 255, 0);

            // Print out some useful information
            Console.WriteLine("");
            Console.WriteLine("Max. no. iterations: {0}", cannySegmentation.GetNumberOfIterations());
            Console.WriteLine("Max. RMS error: {0}", cannySegmentation.GetMaximumRMSError());
            Console.WriteLine("");
            Console.WriteLine("No. elpased iterations: {0}", cannySegmentation.GetElapsedIterations());
            Console.WriteLine("RMS change: {0}", cannySegmentation.GetRMSChange());

            //needs imageJ installed and in %path%
            SimpleITK.Show(threshSegResult);
			
			SimpleITK.WriteImage(cannySegResult, args[2]);
            
        }
    }
}
