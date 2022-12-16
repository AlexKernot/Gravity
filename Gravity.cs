using System;
using System.Diagnostics;
using System.Reflection;

namespace Gravity;

public static class Globals
{
    public const double gravitationalConstant = 0.00000000000667408;

    public const int numberOfPlanets = 2;

    public const double timeStep = 0.5f;
    public const int repeats = 100;

    private static int currentRelativeTime = 0;

    public static int GetTime()
    {
        return currentRelativeTime;
    }

    public static bool IncrementTime()
    {
        currentRelativeTime++;
        return true;
    }
}

class Gravity
{
    static void Main()
    {
        PlanetClass[] planet = new PlanetClass[Globals.numberOfPlanets];
        for (int i = 0; i < Globals.repeats; i++)
        {
            CalculateStep(planet);
        }

        var plt = new ScottPlot.Plot(1920, 1920);

        for (int i = 0; i > Globals.numberOfPlanets; i++)
        {
            plt.AddScatter(planet[i].GetDataX(), planet[i].GetDataY());
        }

        plt.SaveFig("DisplacementGraph.png");

        // Opens the saved image
        //var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //Process.Start(directory + @"\DisplacementGraph.png");

    }

    static double Mod (double a, double b)
    {
        double aModB = a - (b * Math.Floor(a / b));

        return aModB;
    }

    static double ArcCos (double n)
    {
        // Using the formula ((n - 1) % 2) - 1, numbers for ArcCos and ArcSin can be calculated outside of the domain {n: -1 < n < 1 }
        if (n < -1 || n > 1)
        {
            n--;
            n = Mod(n, 2);
            n--;
        }

        double arcCos = Math.Acos(n);

        return arcCos;
    }

    static double ArcSin(double n)
    {
        if (n < -1 || n > 1)
        {
            n--;
            n = Mod(n, 2);
            n--;
        }

        double aSin = Math.Asin(n);

        return aSin;
    }
    static double[] ToCart(double magnitude, double angle)
    {
        // Converts { r, theta } vectors and converts them into { x, y } vectors
        double[] xy = {
            magnitude * (Math.Cos(angle)),
            magnitude * (Math.Sin(angle)) };

        return xy;
    }
    static double VectorAddition (double vector1, double vetor2, double angleBetween)
    {
        double a = Math.Pow(vector1, 2);

        a *= Math.Pow(vetor2, 2);

        a -= 2 * vector1 * vetor2 * Math.Cos(angleBetween);

        a = Math.Sqrt(a);

        return a;
    }

    static double Magnitude (double a, double b)
    {
        double magnitude = Math.Pow(a, 2);

        magnitude += Math.Pow(b, 2);

        magnitude = Math.Sqrt(magnitude);

        return magnitude;
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

    static bool CalculateStep(PlanetClass[] planet)
    {
        Console.WriteLine("This program doesn't do anything yet.");
        return true;
    }
}