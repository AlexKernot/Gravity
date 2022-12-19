using System;

namespace Gravity;
public class PlanetClass
{
    private long mass = 0;
    private double positionX = 0;
    private double positionY = 0;

    private double velocityX = 0;
    private double velocityY = 0;

    private double[] dataX = new double[Globals.repeats];
    private double[] dataY = new double[Globals.repeats];

    public bool UpdatePosition()
    {
        positionX += velocityX * Globals.timeStep;
        positionY += velocityY * Globals.timeStep;

        if (Double.IsNaN(positionX) || Double.IsNaN(positionY))
        {
            return false;
        }

        dataX[Globals.GetTime()] = positionX;
        dataY[Globals.GetTime()] = positionY;

        return true;
    }

    public bool AddVelocity(double changeVelocityX, double changeVelocityY)
    {
        velocityX += changeVelocityX;
        velocityY += changeVelocityY;

        return true;
    }
    public long GetMass()
    {
        return mass;
    }

    public bool SetMass(long setMass)
    {
        mass = setMass;
        return true;
    }

    public double GetPositionX()
    {
        return positionX;
    }

    public bool SetPositionX(double setPositionX)
    {
        positionX = setPositionX;
        return true;
    }

    public double GetPositionY()
    {
        return positionY;
    }

    public bool SetPositionY(double setPositionY)
    {
        positionY = setPositionY;
        return true;
    }

    public double GetVelocityX()
    {
        return velocityX;
    }

    public bool SetVelocityX(double setVelocityX)
    {
        velocityX = setVelocityX;
        return true;
    }

    public double GetVelocityY()
    {
        return velocityY;
    }

    public bool SetVelocityY(double setVelocityY)
    {
        velocityY = setVelocityY;
        return true;
    }

    public double[] GetDataX()
    {
        return dataX;
    }

    // Automatically assumes it's at timestep i.
    public bool SetDataX(double value) {
        dataX[Globals.GetTime()] = value;
        return true;
    }
    public double[] GetDataY()
    {
        return dataY;
    }

    public bool SetDataY(double value)
    {
        dataY[Globals.GetTime()] = value;
        return true;
    }
}
