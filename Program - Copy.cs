using System;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Gravity
{
    class Gravity
    {
        // Object masses
        static double mass1 = 0f;
        static double mass2 = 0f;

        // Gravitatonal constant
        static float G = (float)( 6.67408 * Math.Pow(10, -11) );

        static double timeStep = 0f;
        static int repeats = 0;

        // Position for object 1
        double x1 = 0;
        double y1 = 0;
        double velocityX = 0;
        double velocityY = 0;

        // Set position for object 2
        static double x2 = 0;
        static double y2 = 0;

        double[] dataX;
        double[] dataY;

        static double[] ToPolar(double x, double y)
        {
            // Converts { x, y } vectors into { r, theta } vectors
            double[] rtheta = { 
                Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)), 
                Math.Atan(y / x) };

            return rtheta;
        }

        static double[] ToCart(double r, double theta)
        {
            // Converts { r, theta } vectors and converts them into { x, y } vectors
            double[] xy = { 
                r * (Math.Sin(theta)), 
                r * (Math.Cos(theta)) };

            return xy;
        }

        static double VectorAddition(double a, double b, double C)
        {
            // Calculates Vector Addition via the Cosine rule sqrt((a^2 + b^2 - 2ac * Cos(C)).
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2) - (2 * a * b * Math.Cos(C)));
        }

        static double DistanceFormula(double x1, double x2, double y1, double y2)
        {
            // Calculated the distance in cartesian coordinates via the following formula: sqrt( (x2-x1)^2 + (y2-y1)^2 )
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        static double Velocity (double distance)
        {
            // Velocity due to gravity; v = G(m2 / r^2)t
            return (G * (mass2 / Math.Pow(distance, 2))) * timeStep;
        }

        static double AngleBetween (double y1, double y2, double x1, double x2)
        {
            // Returns the angle between the the two objects
            double angle = Math.Atan2((y2 - y1), (x2 - x1));


            return angle;
        }

        void MainCalc (int i)
        {
            // Handles all required calculations to return a final { x, y } displacement

            double distance = DistanceFormula(x1, x2, y1, y2);
            double velocity = Velocity(distance);
            double angle = AngleBetween(y1, y2, x1, x2);

            double[] convVelocity = ToCart(velocity, angle);

            velocityX += convVelocity[0];
            velocityY += convVelocity[1];
            double newVectorX = x1 + (velocityX * timeStep);
            double newVectorY = y1 + (velocityY * timeStep);

            x1 = newVectorX;
            y1 = newVectorY;

            Console.WriteLine(String.Format("d = {0}, angle = {1}, velocityX = {2}, velocityY = {3}, velX = {4}, velY = {5}, x = {6}, y = {7}", Math.Round(distance).ToString(), (angle * (180 / Math.PI)).ToString(), Math.Round(velocityX), Math.Round(velocityY), Math.Round(convVelocity[0]), Math.Round(convVelocity[1]), Math.Round(x1), Math.Round(y1)));

            dataX[i] = x1;
            dataY[i] = y1;
        }
        public void DeclareVariables()
        {
                var config = ConfigurationManager.AppSettings;

                timeStep = Double.Parse(config.Get("TimeStep"));
                repeats = int.Parse(config.Get("Repeats"));
                x1 = double.Parse(config.Get("X1"));
                y1 = double.Parse(config.Get("Y1"));
                x2 = double.Parse(config.Get("X2"));
                y2 = double.Parse(config.Get("Y2"));
                velocityX = double.Parse(config.Get("InitVelocityX"));
                velocityY = double.Parse(config.Get("InitVelocityY"));
                mass1 = double.Parse(config.Get("Mass1"));
                mass2 = double.Parse(config.Get("Mass2"));

            // Used for storing the entire session's x and y coordinates to draw a graph
            dataX = new double[repeats];
            dataY = new double[repeats];
        }

        public void MainLoop()
        {
            for (int i = 0; i < repeats; i++)
            {
                MainCalc(i);
            }

            string[] fileLines = new string[repeats];

            for (int i = 0; i < repeats; i++)
            {
                fileLines[i] = dataX[i].ToString() + " " + dataY[i].ToString();
            }

            File.WriteAllLines("gravity.txt", fileLines);

            var plt = new ScottPlot.Plot(1080, 1920);
            plt.AddScatter(dataX, dataY);
            plt.SaveFig("DisplacementGraph.png");

            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Process.Start(directory + @"\DisplacementGraph.png");
        }

        static void Main(string[] args)
        {
            Gravity grav = new Gravity();
            grav.DeclareVariables();
            grav.MainLoop();
        }
    }
}
