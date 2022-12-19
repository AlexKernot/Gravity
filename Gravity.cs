using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace Gravity;

public static class Globals
{
    public const double gravitationalConstant = 0.00000667408;

    public const int numberOfPlanets = 4;

    public const double timeStep = 1;
    public const int repeats = 650;

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
    private const long massOfEarth = 59720000000000;

    static void Main()
    {
        PlanetClass[] planet = new PlanetClass[Globals.numberOfPlanets];

        for (int i = 0; i < Globals.numberOfPlanets; i++)
        {
            planet[i] = new PlanetClass();
        }

        planet[0].SetMass(massOfEarth);
        planet[1].SetMass(massOfEarth);


        planet[0].SetPositionX(32000);
        planet[1].SetPositionX(-32000);
        planet[0].SetPositionY(32000);
        planet[1].SetPositionY(-32000);
        planet[2].SetMass(massOfEarth);
        planet[2].SetPositionX(-32000);
        planet[2].SetPositionY(32000);
        planet[3].SetMass(massOfEarth);
        planet[3].SetPositionX(32000);
        planet[3].SetPositionY(-32000);


        for (int i = 0; i < Globals.repeats; i++)
        {
            planet = CalculateStep(planet);
            Globals.IncrementTime();
        }

        var plt = new ScottPlot.Plot(1920, 1920);

        for (int i = 0; i < Globals.numberOfPlanets; i++)
        {
            plt.AddScatter(planet[i].GetDataX(), planet[i].GetDataY());
        }

        plt.SaveFig("DisplacementGraph.png");

    }
    static double[] ToCart(double magnitude, double angle)
    {
        // Converts { r, theta } vectors and converts them into { x, y } vectors
        double[] xy = {
            magnitude * (Math.Cos(angle)),
            magnitude * (Math.Sin(angle)) };

        return xy;
    }
    static double AngleVector(double point1X, double point1Y, double point2X, double point2Y)
    {

        double vectorX = (point2X - point1X);
        double vectorY = (point2Y - point1Y);

        // double magnitudeVector = Magnitude(vectorX, vectorY);

        // double cosA = vectorY / magnitudeVector;
        double angle = Math.Atan2(vectorX, vectorY);

        angle = Math.PI / 2 - angle;

        return angle;
    }
    static double DistanceFormula(double x1, double y1, double x2, double y2)
    {
        // Calculated the distance in cartesian coordinates via the following formula: sqrt( (x2-x1)^2 + (y2-y1)^2 )
        double x = x2 - x1;
        double y = y2 - y1;

        return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    static double ForceDueToGravity(double distanceBetween, double planet1Mass, double planet2Mass)
    {
        double gravity = planet1Mass * planet2Mass;
        gravity /= Math.Pow(distanceBetween, 2);
        gravity *= Globals.gravitationalConstant;

        return gravity;
    }

    static PlanetClass[] CalculateStep(PlanetClass[] planet)
    {
        // number of planets - 1 because by the time the last planet is reached, its interaction has already been calculated with every planet
        for (int i = 0; i < Globals.numberOfPlanets - 1; i++)
        {
            PlanetClass planet1 = planet[i];

            double planet1X = planet1.GetPositionX();
            double planet1Y = planet1.GetPositionY();
            double planet1Mass = planet1.GetMass();

            // This is broken up into an unessecary amount of variables because I need to make sure all the math happens in a very specific way
            // j = i + 1 finds the next planet that hasn't been calculated already
            for (int j = i + 1; j < Globals.numberOfPlanets; j++)
            {
                PlanetClass planet2 = planet[j];

                double planet2X = planet2.GetPositionX();
                double planet2Y = planet2.GetPositionY();
                double planet2Mass = planet2.GetMass();

                double distanceBetween = DistanceFormula(planet1X, planet1Y, planet2X, planet2Y);
                double forceBetween = ForceDueToGravity(distanceBetween, planet1Mass, planet2Mass);
                double angleBetween = AngleVector(planet1X, planet1Y, planet2X, planet2Y);

                double changeAccelerationPlanet1 = forceBetween / planet1Mass;
                double changeAccelerationPlanet2 = forceBetween / planet2Mass;

                double changeMagnitudeOfVelocityPlanet1 = changeAccelerationPlanet1 / Globals.timeStep;
                double changeMagnitudeOfVelocityPlanet2 = changeAccelerationPlanet2/ Globals.timeStep;

                double[] changeVelocityVectorPlanet1 = ToCart(changeMagnitudeOfVelocityPlanet1, angleBetween);

                // Angle between planet 2 and planet 1 is the same as 1 and 2 but in the opposite direction
                double[] changeVelocityVectorPlanet2 = ToCart(changeMagnitudeOfVelocityPlanet2, angleBetween + Math.PI);

                planet1.AddVelocity(changeVelocityVectorPlanet1[0], changeVelocityVectorPlanet1[1]);
                planet2.AddVelocity(changeVelocityVectorPlanet2[0], changeVelocityVectorPlanet2[1]);

                // Once finished with the planet
                planet[j] = planet2;
            }
            planet[i] = planet1;
        }
        for (int i = 0; i < Globals.numberOfPlanets; i++)
        {
            planet[i].UpdatePosition();
        }
        return planet;
    }
}