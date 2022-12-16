namespace Gravity;
public class PlanetClass
{
    private long mass = 0;
    private int positionX = 0;
    private int positionY = 0;

    private int velocityX = 0;
    private int velocityY = 0;

    public long GetMass()
    {
        return mass;
    }

    public bool SetMass(long setMass)
    {
        mass = setMass;
        return true;
    }

    public int GetPositionX()
    {
        return positionX;
    }

    public bool SetPositionX(int setPositionX)
    {
        positionX = setPositionX;
        return true;
    }

    public int GetPositionY()
    {
        return positionY;
    }

    public bool SetPositionY(int setPositionY)
    {
        positionY = setPositionY;
        return true;
    }

    public int GetVelocityX()
    {
        return velocityX;
    }

    public bool SetVelocityX(int setVelocityX)
    {
        velocityX = setVelocityX;
        return true;
    }

    public int GetVelocityY()
    {
        return velocityY;
    }

    public bool SetVelocityY(int setVelocityY)
    {
        velocityY = setVelocityY;
        return true;
    }
}
