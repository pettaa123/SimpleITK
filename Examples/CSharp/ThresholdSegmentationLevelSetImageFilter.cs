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


//    INPUTS:  {BrainProtonDensitySlice.png}
//    OUTPUTS: {ThresholdSegmentationLevelSetImageFilterVentricle.png}
//    ARGUMENTS:    81 112 210 250


// The ThresholdSegmentationLevelSetImageFilter is an extension of
// the threshold connected-component segmentation to the level set framework.
// The goal is to define a range of intensity values that classify the tissue
// type of interest and then base the propagation term on the level set
// equation for that intensity range.  Using the level set approach, the
// smoothness of the evolving surface can be constrained to prevent some of
// the "leaking" that is common in connected-component schemes.
//
// The threshold segmentation filter expects two inputs.  The first is an
// initial level set in the form of an Image. The second input is
// the feature image g.  For many applications, this filter requires little
// or no preprocessing of its input.  Smoothing the input image is not
// usually required to produce reasonable solutions, though it may still be
// warranted in some cases.
//
// The initial surface is generated using the fast marching filter.
// The output of the segmentation filter is passed to a
// BinaryThresholdImageFilter to create a binary representation of the
// segmented object.


using System;

using itk.simple;
using PixelId = itk.simple.PixelIDValueEnum;
using SitkImage = itk.simple.Image;

namespace itk.simple.examples {

    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("Missing Parameters ");
                Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName +
                                  "inputImage outputImage seedX seedY" +
                                  " lowerThreshold upperThreshold curvatureScaling == 1.0");
                return;
            }

            // Read input image

            SitkImage inputImage = SimpleITK.ReadImage(args[0], PixelId.sitkFloat32);

            //  The FastMarching will be used to generate the initial level set in the form of a distance
            //  FastMarching requires the user to provide a seed
            //  point from which the level set will be generated. The user can actually
            //  pass not only one seed point but a set of them. Note that the
            //  FastMarching is used here only as a helper in the
            //  determination of an initial Level Set. We could have used the
            //  DanielssonDistanceMapImageFilter in the same way.

            //  The seeds are passed stored in a list.  
           
            VectorUIntList seeds = new VectorUIntList();
            VectorUInt32 seedPosition = new VectorUInt32(2);
            seedPosition.Add(Convert.ToUInt32(args[2]));
            seedPosition.Add(Convert.ToUInt32(args[3]));
            seeds.Add(seedPosition);
            
            double initialDistance = Convert.ToDouble(args[6]);
            double seedValue = -initialDistance;

            SitkImage fastMarchImage = SimpleITK.FastMarching(inputImage, seeds);

            //  For the ThresholdSegmentationLevelSetImageFilter, scaling
            //  parameters are used to balance the influence of the propagation
            //  (inflation) and the curvature (surface smoothing) terms from
            //  LevelSetEquation. The advection term is not used in
            //  this filter. Set the terms with methods \SetPropagationScaling()
            //  and SetCurvatureScaling(). Both terms are set to 1.0 in this
            //  example.

            ThresholdSegmentationLevelSetImageFilter thresholdSegmentation = new ThresholdSegmentationLevelSetImageFilter();
            
            thresholdSegmentation.SetPropagationScaling(1.0);
            if (args.Length > 6)
            {
                thresholdSegmentation.SetCurvatureScaling(Convert.ToDouble(args[6]));
            }
            else
            {
                thresholdSegmentation.SetCurvatureScaling(1.0);
            }
            //  The level set solver will stop if the convergence criteria has been
            //  reached or if the maximum number of iterations has elasped.  The
            //  convergence criteria is defined in terms of the root mean squared (RMS)
            //  change in the level set function. When RMS change for an iteration is
            //  below a user-specified threshold, the solution is considered to have
            //  converged.
            thresholdSegmentation.SetMaximumRMSError(0.02);
            thresholdSegmentation.SetNumberOfIterations(1200);

            // The convergence criteria MaximumRMSError and
            // MaximumIterations are set as in previous examples.  We now set
            // the upper and lower threshold values U and L, and the isosurface
            // value to use in the initial model.

            thresholdSegmentation.SetUpperThreshold( Convert.ToDouble(args[5]));
            thresholdSegmentation.SetLowerThreshold( Convert.ToDouble(args[4]));

            SitkImage threshSegImage=thresholdSegmentation.Execute(fastMarchImage, inputImage);

            SitkImage threshSegResult = SimpleITK.BinaryThreshold(threshSegImage, -1000.0, 0.0, 255, 0);


            // Print out some useful information
            Console.WriteLine("");
            Console.WriteLine("Max. no. iterations: {0}", thresholdSegmentation.GetNumberOfIterations());
            Console.WriteLine("Max. RMS error: {0}", thresholdSegmentation.GetMaximumRMSError());
            Console.WriteLine("");
            Console.WriteLine("No. elapsed iterations: {0}", thresholdSegmentation.GetElapsedIterations());
            Console.WriteLine("RMS change: {0}", thresholdSegmentation.GetRMSChange());

            //needs imageJ installed and in %path%
            SimpleITK.Show(threshSegResult);

            SimpleITK.WriteImage(threshSegResult, args[1]);

        }
    }
}
