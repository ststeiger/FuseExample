
namespace FuseExample 
{


    class Statistics
    {


        public static double Average(double[] vals)
        {
            double sum = 0;
            for (long i = 0; i < vals.LongLength; ++i)
            {
                sum += vals[i];
            }

            sum = sum / (double)vals.LongLength;
            return sum;
        }


        public static double StDev(double[] vals)
        {
            double dblAverage = Average(vals);

            double sum = 0;
            for (long i = 0; i < vals.LongLength; ++i)
            {
                sum += System.Math.Pow(vals[i] - dblAverage, 2.0);
            }

            sum = sum / (double)vals.LongLength;
            sum = System.Math.Sqrt(sum);
            return sum;
        }


        public static void tdist()
        {
            double[] vals = new double[] { 1.1, 1.2, 1.3, 1.4, 1.5 };
            double avg = Average(vals);
            System.Console.WriteLine(avg);
        }


    }


}
