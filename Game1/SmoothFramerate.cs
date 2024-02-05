class SmoothFramerate
{
    int samples;
    int currentFrame;
    double[] frametimes;
    double currentFrametimes;

    public double framerate
    {
        get
        {
            return (samples / currentFrametimes);
        }
    }

    public SmoothFramerate(int Samples)
    {
        samples = Samples;
        currentFrame = 0;
        frametimes = new double[samples];
    }

    public void Update(double timeSinceLastFrame)
    {
        currentFrame++;
        if (currentFrame >= frametimes.Length) { currentFrame = 0; }

        currentFrametimes -= frametimes[currentFrame];
        frametimes[currentFrame] = timeSinceLastFrame;
        currentFrametimes += frametimes[currentFrame];
    }
}