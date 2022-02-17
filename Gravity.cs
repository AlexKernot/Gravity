using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Gravity
{
    class Gravity
    {
        // Object masses
        static double mass1 = 0f;
        static double mass2 = 0f;
        static double mass3 = 0f;

        // Gravitatonal constant
        static float G = (float)(6.67408 * Math.Pow(10, -11));

        static double timeStep = 0f;
        static int repeats = 0;

        // Position for object 1
        double x1 = 0;
        double y1 = 0;

        double velocityX = 0;
        double velocityY = 0;

        // Set position for object 2
        double x2 = 0;
        double y2 = 0;

        double velocityX2 = 0;
        double velocityY2 = 0;

        // set position for object 3
        double x3 = 0;
        double y3 = 0;

        double velocityX3 = 0;
        double velocityY3 = 0;

        // Variables for the graphs
        double[] dataX;
        double[] dataY;

        double[] dataX2;
        double[] dataY2;

        double[] dataX3;
        double[] dataY3;

        static double[] ToCart(double r, double theta)
        {
            // Converts { r, theta } vectors and converts them into { x, y } vectors
            double[] xy = {
                r * (Math.Cos(theta)),
                r * (Math.Sin(theta)) };

            return xy;
        }
        static double[] VectorAddition (double b, double c, double theta)
        {
            double a = Math.Sqrt((Math.Pow(b, 2) * Math.Pow(c, 2)) - (2 * b * c * Math.Cos(theta)));

            double A = Math.Acos((Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c));

            double[] magTheta = { a, A };

            return magTheta;
        }

        static double Magnitude (double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        static double DotProduct (double u1, double u2, double v1, double v2)
        {

            double dotProduct = ((u1 * v1) + (u2 * v2));

            return dotProduct;
        }

        static double AngleVector(double x1, double y1, double x2, double y2)
        {
            double dotProduct = DotProduct(x1, x2, y1, y2);

            double magU = Magnitude(x1, y1);
            double magV = Magnitude(x2, y2);

            double angle = dotProduct / (magU * magV);

            return angle;
        }
        static double DistanceFormula(double u1, double u2, double v1, double v2)
        {
            // Calculated the distance in cartesian coordinates via the following formula: sqrt( (x2-x1)^2 + (y2-y1)^2 )
            return Math.Sqrt(Math.Pow(u2 - u1, 2) + Math.Pow(v2 - v1, 2));
        }

        static double Force(double distance)
        {
            // Force due to gravity; F = G(m1m2/r^2)
            return G * mass1 * mass2 / Math.Pow(distance, 2);
        }
        static double[] Acceleration(double distance1, double distance2, double mass1, double mass2, double u1, double u2, double v1, double v2)
        {
            double acc1 = G * (mass1 / Math.Pow(distance1, 2));
            double acc2 = G * (mass2 / Math.Pow(distance2, 2));
            double angle = AngleVector(u1, u2, v1, v2);
            double[] acc = VectorAddition(acc1, acc2, angle);

            return acc;
        }

        static double AngleBetween(double y1, double y2, double x1, double x2)
        {
            // Returns the angle between the the two objects
            double angle = (Math.Atan2(y2 - y1, x2 - x1) + 2* Math.PI);


            return angle;
        }

        async void MainLoop()
        {
            for (int i = 0; i < repeats; i++)
            {
                Task<bool> calc1 = MainCalc1(i);
                Task<bool> calc2 = MainCalc2(i);
                Task<bool> calc3 = MainCalc3(i);

                await Task.WhenAll(calc1, calc2, calc3);
            }
        }

        Task<bool> MainCalc1(int i)
        {
            // Handles all required calculations to return a final { x1, y1 } displacement

            double distance1 = DistanceFormula(x1, x2, y1, y2);
            double distance2 = DistanceFormula(x1, x3, y1, y3);

            double[] acc = Acceleration(distance1, distance2, mass2, mass3, x2, x3, y2, y3);
            double angle = acc[1];
            double[] convVelocity = ToCart(acc[0], acc[1]);

            velocityX += convVelocity[0];
            velocityY += convVelocity[1];

            double newVectorX = x1 + (velocityX * timeStep);
            double newVectorY = y1 + (velocityY * timeStep);

            x1 = newVectorX;
            y1 = newVectorY;

            Console.WriteLine(String.Format("2) d = {0}, angle = {1}, velocityX = {2}, velocityY = {3}, velX = {4}, velY = {5}, x = {6}, y = {7}, ({8})\n", 
                Math.Round(distance1).ToString(), 
                (angle * (180 / Math.PI)).ToString(), 
                Math.Round(velocityX), 
                Math.Round(velocityY), 
                Math.Round(convVelocity[0]),
                Math.Round(convVelocity[1]), 
                Math.Round(x1), 
                Math.Round(y1), i));

            dataX[i] = x1;
            dataY[i] = y1;

            return Task.FromResult(true);
        }

        Task<bool> MainCalc2(int i)
        {
            // Handles all required calculations to return a final { x1, y1 } displacement

            double distance1 = DistanceFormula(x2, x1, y2, y1);
            double distance2 = DistanceFormula(x2, x3, y2, y3);

            double[] acc = Acceleration(distance1, distance2, mass1, mass3, x1, x3, y1, y3);
            double angle = acc[1];
            double[] convVelocity = ToCart(acc[0], acc[1]);

            velocityX2 += convVelocity[0];
            velocityY2 += convVelocity[1];

            double newVectorX = x2 + (velocityX2 * timeStep);
            double newVectorY = y2 + (velocityY2 * timeStep);

            x2 = newVectorX;
            y2 = newVectorY;

            Console.WriteLine(String.Format("3) d = {0}, angle = {1}, velocityX = {2}, velocityY = {3}, velX = {4}, velY = {5}, x = {6}, y = {7}, ({8})\n",
                Math.Round(distance1).ToString(),
                (angle * (180 / Math.PI)).ToString(),
                Math.Round(velocityX2),
                Math.Round(velocityY2),
                Math.Round(convVelocity[0]),
                Math.Round(convVelocity[1]),
                Math.Round(x2),
                Math.Round(y2), i));

            dataX2[i] = x2;
            dataY2[i] = y2;

            return Task.FromResult(true);
        }

        Task<bool> MainCalc3(int i)
        {
            // Handles all required calculations to return a final { x1, y1 } displacement

            double distance1 = DistanceFormula(x3, x1, y3, y1);
            double distance2 = DistanceFormula(x3, x2, y3, y2);

            double[] acc = Acceleration(distance1, distance2, mass1, mass2, x1, x2, y1, y2);
            double angle = acc[1];
            double[] convVelocity = ToCart(acc[0], acc[1]);

            velocityX3 += convVelocity[0];
            velocityY3 += convVelocity[1];

            double newVectorX = x3 + (velocityX3 * timeStep);
            double newVectorY = y3 + (velocityY3 * timeStep);

            x3 = newVectorX;
            y3 = newVectorY;

            Console.WriteLine(String.Format("1) d = {0}, angle = {1}, velocityX = {2}, velocityY = {3}, velX = {4}, velY = {5}, x = {6}, y = {7}, ({8})\n",
                Math.Round(distance1).ToString(),
                (angle * (180 / Math.PI)).ToString(),
                Math.Round(velocityX3),
                Math.Round(velocityY3),
                Math.Round(convVelocity[0]),
                Math.Round(convVelocity[1]),
                Math.Round(x3),
                Math.Round(y3), i));

            dataX3[i] = x3;
            dataY3[i] = y3;

            return Task.FromResult(true);
        }

        public void DeclareVariables()
        {
            // This grabs all the variables from the external config file and stores them as their corrosponding variables so the code doesn't need to be modified for number changes
            var config = ConfigurationManager.AppSettings;

            timeStep = Double.Parse(config.Get("TimeStep"));
            repeats = int.Parse(config.Get("Repeats"));

            mass1 = double.Parse(config.Get("Mass1"));
            mass2 = double.Parse(config.Get("Mass2"));
            mass3 = double.Parse(config.Get("Mass3"));

            x1 = double.Parse(config.Get("X1"));
            y1 = double.Parse(config.Get("Y1"));

            x2 = double.Parse(config.Get("X2"));
            y2 = double.Parse(config.Get("Y2"));

            x3 = double.Parse(config.Get("X3"));
            y3 = double.Parse(config.Get("Y3"));

            velocityX = double.Parse(config.Get("InitVelocityX"));
            velocityY = double.Parse(config.Get("InitVelocityY"));

            velocityX2 = double.Parse(config.Get("InitVelocityX2"));
            velocityY2 = double.Parse(config.Get("InitVelocityY2"));

            velocityX3 = double.Parse(config.Get("InitVelocityX3"));
            velocityY3 = double.Parse(config.Get("InitVelocityY3"));

            // Used for storing the entire session's x and y coordinates to draw a graph
            dataX = new double[repeats];
            dataY = new double[repeats];

            dataX2 = new double[repeats];
            dataY2 = new double[repeats];

            dataX3 = new double[repeats];
            dataY3 = new double[repeats];
        }

        public void Main2() 
        {

            MainLoop();

            var plt = new ScottPlot.Plot(1920, 1920);
            plt.AddScatter(dataX, dataY);
            plt.AddScatter(dataX2, dataY2);
            plt.AddScatter(dataX3, dataX3);
            plt.SaveFig("DisplacementGraph.png");

            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Process.Start(directory + @"\DisplacementGraph.png");
        }

        static void Main(string[] args)
        {
            Gravity grav = new Gravity();
            grav.DeclareVariables();
            grav.Main2();
        }
    }
}
