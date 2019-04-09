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


namespace itk.simple.examples
{


    class IterationUpdate : Command
    {

        private ImageRegistrationMethod m_Method;

        public IterationUpdate(ImageRegistrationMethod m)
        {
            m_Method = m;
        }

        public override void Execute()
        {
            VectorDouble pos = m_Method.GetOptimizerPosition();
            Console.WriteLine("{0:3} = {1:10.5} : [{2}, {3}]",
                              m_Method.GetOptimizerIteration(),
                              m_Method.GetMetricValue(),
                              pos[0], pos[1]);
        }
    }

    class ImageRegistrationMethod3
    {

        static void Main(string[] args)
        {

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: %s <fixedImageFilter> <movingImageFile> <outputTransformFile>\n", "ImageRegistrationMethod2");
                return;
            }

            ImageFileReader reader = new ImageFileReader();
            reader.SetOutputPixelType(PixelIDValueEnum.sitkFloat32);

            reader.SetFileName(args[0]);
            Image fixedImage = reader.Execute();

            reader.SetFileName(args[1]);
            Image movingImage = reader.Execute();

            ImageRegistrationMethod R = new ImageRegistrationMethod();
            R.SetMetricAsCorrelation();

            double learningRate = 2.0;
            double  minStep = 1e-4;
            uint numberOfIterations = 500;
            double gradientMagnitudeTolerance = 1e-8;
            
            R.SetOptimizerAsRegularStepGradientDescent(learningRate,
                                                      minStep,
                                                      numberOfIterations,
                                                      gradientMagnitudeTolerance);
            R.SetOptimizerScalesFromIndexShift();

            tx = sitk.CenteredTransformInitializer(fixedImage, movingImage, sitk.Similarity2DTransform());
            R.SetInitialTransform(tx);
            R.SetInterpolator(InterpolatorEnum.sitkLinear);

            IterationUpdate cmd = new IterationUpdate(R);
            R.AddCommand(EventEnum.sitkIterationEvent, cmd);

            Transform outTx = R.Execute(fixedImage, movingImage);

            outTx.WriteTransform(args[2]);

        }

    }

}

