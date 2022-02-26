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


        static double Mod (double a, double b)
        {
            double aModB = a - (b * Math.Floor(a / b));

            return aModB;
        }

        static double ArcCos (double n)
        {
            if (n < -1 || n > 1)
            {
                n = Mod(n, 2);
                n -= 1;
            }

            double aCos = Math.Acos(n);

            return aCos;
        }

        static double ArcSin(double n)
        {
            if (n < -1 || n > 1)
            {
                n = Mod(n, 2);
                n -= 1;
            }

            double aSin = Math.Asin(n);

            return aSin;
        }
        static double[] ToCart(double r, double theta)
        {
            // Converts { r, theta } vectors and converts them into { x, y } vectors
            double[] xy = {
                r * (Math.Cos(theta)),
                r * (Math.Sin(theta)) };

            return xy;
        }
        static double VectorAddition (double b, double c, double theta)
        {
            double a = Math.Sqrt((Math.Pow(b, 2) * Math.Pow(c, 2)) - (2 * b * c * Math.Cos(theta)));

            return a;
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

        static double FinalAngle(double mag1, double mag2, double angle)
        {
            double finalAngle1 = (mag1 * Math.Sin(angle) / mag2);
            double finalAngle = ArcSin(finalAngle1);

            return finalAngle;
        }

        static double AngleVector(double x1, double y1, double x2, double y2)
        {
            double dotProduct = DotProduct(x1, y1, x2, y2);

            double magU = Magnitude(x1, y1);
            double magV = Magnitude(x2, y2);

            double dot = (dotProduct / (magU * magV));
            double angle = ArcCos(dot);

            return angle;
        }
        static double DistanceFormula(double x1, double y1, double x2, double y2)
        {
            // Calculated the distance in cartesian coordinates via the following formula: sqrt( (x2-x1)^2 + (y2-y1)^2 )
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        static double[] Acceleration(double distance1, double distance2, double mass1, double mass2, double u1, double u2, double v1, double v2, double w1, double w2)
        {
            double acc1 = G * (mass1 / Math.Pow(distance1, 2));
            double acc2 = G * (mass2 / Math.Pow(distance2, 2));

            double angle1 = AngleVector(u1, u2, w1, w2);
            double angle2 = AngleVector(v1, v2, w1, w2);

            double angle = AngleVector(u1, u2, v1, v2);
            double acc = VectorAddition(acc1, acc2, angle2 - angle1);

            // ERROR IS HERE
            double finalAngle = FinalAngle(acc2, acc, angle);

            double angleMinus = AngleVector(u1, u2, 1, 0);

            double finalAngle2 = Mod(angleMinus - finalAngle, (Math.PI * 2));

            double[] convVelocity = ToCart(acc, finalAngle2);

            double[] returns = { convVelocity[0], convVelocity[1], finalAngle2 };

            return returns;
        }

        void MainLoop()
        {
            for (int i = 0; i < repeats; i++)
            {
                MainCalc1(i);
            }
        }
        void MainCalc1(int i)
        {
            // Handles all required calculations to return a final { x1, y1 } displacement

            double distance1 = DistanceFormula(x1, y1, x2, y2);
            double distance2 = DistanceFormula(x1, y1, x3, y3);

            double[] acc = Acceleration(distance1, distance2, mass2, mass3, x2, y2, x3, y3, x1, y1);

            velocityX += acc[0];
            velocityY += acc[1];

            double newVectorX = x1 + (velocityX * timeStep);
            double newVectorY = y1 + (velocityY * timeStep);

            x1 = newVectorX;
            y1 = newVectorY;

            Console.WriteLine(String.Format("1) d = {0}, angle = {1}, velocityX = {2}, velocityY = {3}, velX = {4}, velY = {5}, x = {6}, y = {7}, ({8})\n",
                Math.Round(distance1).ToString(),
                (acc[2] * (180 / Math.PI)).ToString(),
                Math.Round(velocityX),
                Math.Round(velocityY),
                Math.Round(acc[0]),
                Math.Round(acc[1]),
                Math.Round(x1),
                Math.Round(y1), i));

            dataX[i] = x1;
            dataY[i] = y1;

        }

        public bool DeclareVariables()
        {
            // This grabs all the variables from the external config file and stores them as their corrosponding variables so the code doesn't need to be modified for number changes
            var config = ConfigurationManager.AppSettings;

            if (!double.TryParse(config.Get("TimeStep"), out timeStep)) {
                Console.WriteLine("Error with TimeStep");
                return false;
            };

            if (!int.TryParse(config.Get("Repeats"), out repeats))
            {
                Console.WriteLine("Error with Repeats");
                return false;
            };

            if (!double.TryParse(config.Get("Mass1"), out mass1))
            {
                Console.WriteLine("Error with Mass1");
                return false;
            };

            if (!double.TryParse(config.Get("Mass2"), out mass2))
            {
                Console.WriteLine("Error with Mass2");
                return false;
            };

            if (!double.TryParse(config.Get("Mass3"), out mass3))
            {
                Console.WriteLine("Error with Mass3");
                return false;
            };

            if (!double.TryParse(config.Get("X1"), out x1))
            {
                Console.WriteLine("Error with X1");
                return false;
            };

            if (!double.TryParse(config.Get("Y1"), out y1))
            {
                Console.WriteLine("Error with Y1");
                return false;
            };

            if (!double.TryParse(config.Get("X2"), out x2))
            {
                Console.WriteLine("Error with X2");
                return false;
            };

            if (!double.TryParse(config.Get("Y2"), out y2))
            {
                Console.WriteLine("Error with Y2");
                return false;
            };

            if (!double.TryParse(config.Get("X3"), out x3))
            {
                Console.WriteLine("Error with X3");
                return false;
            };

            if (!double.TryParse(config.Get("Y3"), out y3))
            {
                Console.WriteLine("Error with Y3");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityX"), out velocityX))
            {
                Console.WriteLine("Error with InitVelocityX");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityY"), out velocityY))
            {
                Console.WriteLine("Error with InitVelocityY");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityX2"), out velocityX2))
            {
                Console.WriteLine("Error with InitVelocityX2");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityY2"), out velocityY2))
            {
                Console.WriteLine("Error with InitVelocityY2");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityX3"), out velocityX3))
            {
                Console.WriteLine("Error with InitVelocityX3");
                return false;
            };

            if (!double.TryParse(config.Get("InitVelocityY3"), out velocityY3))
            {
                Console.WriteLine("Error with InitVelocityY3");
                return false;
            };

            // Used for storing the entire session's x and y coordinates to draw a graph
            dataX = new double[repeats];
            dataY = new double[repeats];

            //dataX2 = new double[repeats];
            //dataY2 = new double[repeats];

            //dataX3 = new double[repeats];
            //dataY3 = new double[repeats];

            return true;
        }

        public void Main2() 
        {

            MainLoop();

            double[] dataX2 = { x2 };
            double[] dataY2 = { y2 };

            double[] dataX3 = { x3 };
            double[] dataY3 = { y3 };

            var plt = new ScottPlot.Plot(1920, 1920);
            plt.AddScatter(dataX, dataY);
            plt.AddScatter(dataX2, dataY2);
            plt.AddScatter(dataX3, dataY3);
            plt.SaveFig("DisplacementGraph.png");

            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Process.Start(directory + @"\DisplacementGraph.png");
        }

        static void Main(string[] args)
        {
            Gravity grav = new Gravity();
            if (!grav.DeclareVariables())
            {
                throw new VariableInitialisationException();
            }
            grav.Main2();
        }
    }

    public class VariableInitialisationException : Exception
    {
        public VariableInitialisationException()
        { }

        public VariableInitialisationException(string message)
            : base(message) { }

        public VariableInitialisationException(string message, Exception inner)
            : base(message, inner) { }
    }
}
