using System;

namespace VirusC
{
    public class NelderMead
    {
        public static int num_of_functioncalls = 0;

        public static double[] xVal;
        public static double[] yVal;

        public static double HimmelBlau(double[] x)
        {
            num_of_functioncalls++;
            return Math.Pow(Math.Pow(x[0], 2) + x[1] - 11, 2) + Math.Pow(x[0] + Math.Pow(x[1], 2) - 7, 2);
        }

        public static double RosenBrock(double[] x)
        {
            num_of_functioncalls++;
            return Math.Pow(1 - x[0], 2) + 100 * Math.Pow(x[1] - Math.Pow(x[0], 2), 2);
        }

        public static double LogisticCurve(double[] x)
        {
            double sum = 0;
            num_of_functioncalls++;
            for (int i=0;i<xVal.Length;i++)sum += Math.Pow(yVal[i] - x[2] / (1 + Math.Exp(-(xVal[i] - x[1]) / x[0])),2);
            return sum;// x[3] / (1 + Math.Exp(-(x[0] - x[2]) / x[1]));
        }


        public static double[][] optimizer(Func<double[], double> function, int N, double[] xReal, double[] yReal)
        {
            xVal = xReal;
            yVal = yReal;
            Random rnd = new Random();
            double[][] simplex = new double[N + 1][];

            // Generate N + 1 initial arrays.
            for (int array = 0; array <= N; array++){
                simplex[array] = new double[N];
                //for (int index = 0; index < N; index++)
                //{
                //    simplex[array][index] = rnd.NextDouble() * 20 - 10;
                //}
                simplex[array][0] = 2 + rnd.NextDouble() * 2;
                simplex[array][1] = 40 + rnd.NextDouble() * 40;
                simplex[array][2] = 100 + rnd.NextDouble() * 100 - 50;
            }
            const double alpha = 1;
            const double gamma = 2;
            const double rho = 0.5;
            const double sigma = 0.5;

            // Infinite loop until convergence
            int counter = 0;
            while (true){
                counter++;
                // Console.WriteLine("turn {0}: loop {1} Evaluation", num_of_functioncalls, counter);
                // Evaluation
                double[] functionValues = new double[N + 1];
                int[] indices = new int[N + 1];
                for (int vertex_of_simplex = 0; vertex_of_simplex <= N; vertex_of_simplex++){
                    functionValues[vertex_of_simplex] = function(simplex[vertex_of_simplex]);
                    indices[vertex_of_simplex] = vertex_of_simplex;
                }

                // Order
                Array.Sort(functionValues, indices);

                // Console.WriteLine("turn {0}: loop {1} Check covergence", num_of_functioncalls, counter);
                // Check for convergence
                if (Math.Abs(functionValues[N] - functionValues[0]) < 1e-5 || counter>1000){
                    break;
                }

                // Find centroid of the simplex excluding the vertex with highest functionvalue.
                // Console.WriteLine("turn {0}: loop {1} Centroid", num_of_functioncalls, counter);
                double[] centroid = new double[N];

                for (int index = 0; index < N; index++){
                    centroid[index] = 0;
                    for (int vertex_of_simplex = 0; vertex_of_simplex <= N; vertex_of_simplex++){
                        if (vertex_of_simplex != indices[N]){
                            centroid[index] += simplex[vertex_of_simplex][index] / N;
                        }
                    }
                }

                // Console.WriteLine("turn {0}: loop {1} Reflection", num_of_functioncalls, counter);
                // Reflection
                double[] reflection_point = new double[N];
                for (int index = 0; index < N; index++){
                    reflection_point[index] = centroid[index] + alpha * (centroid[index] - simplex[indices[N]][index]);
                }

                double reflection_value = function(reflection_point);

                if (reflection_value >= functionValues[0] & reflection_value < functionValues[N - 1]){
                    simplex[indices[N]] = reflection_point;
                    continue;
                }

                // Console.WriteLine("turn {0}: loop {1} Expansi0n", num_of_functioncalls, counter);
                // Expansion
                if (reflection_value < functionValues[0]){
                    double[] expansion_point = new double[N];
                    for (int index = 0; index < N; index++){
                        expansion_point[index] = centroid[index] + gamma * (reflection_point[index] - centroid[index]);
                    }
                    double expansion_value = function(expansion_point);

                    if (expansion_value < reflection_value){
                        simplex[indices[N]] = expansion_point;
                    }else{
                        simplex[indices[N]] = reflection_point;
                    }
                    continue;
                }

                // Console.WriteLine("turn {0}: loop {1} Contraction", num_of_functioncalls, counter);
                // Contraction
                double[] contraction_point = new double[N];
                for (int index = 0; index < N; index++){
                    contraction_point[index] = centroid[index] + rho * (simplex[indices[N]][index] - centroid[index]);
                }

                double contraction_value = function(contraction_point);

                if (contraction_value < functionValues[N]){
                    simplex[indices[N]] = contraction_point;
                    continue;
                }

                // Console.WriteLine("turn {0}: loop {1} Shrink", num_of_functioncalls, counter);
                // Shrink
                double[] best_point = simplex[indices[0]];
                for (int vertex_of_simplex = 0; vertex_of_simplex <= N; vertex_of_simplex++){
                    for (int index = 0; index < N; index++){
                        simplex[vertex_of_simplex][index] = best_point[index] + sigma * (simplex[vertex_of_simplex][index] - best_point[index]);
                    }
                }
            }
            //Console.WriteLine("turn {0}: a={1} b={2} c={3}", num_of_functioncalls, simplex[0][0], simplex[0][1], simplex[0][2]);
            return simplex;//[0];
        }
    }
}
