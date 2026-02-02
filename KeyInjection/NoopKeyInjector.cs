using System;

namespace GsproMissionControl.KeyInjection;

public sealed class NoopKeyInjector : IKeyInjector
{
    public void SendKeyChar(char c)
    {
        Console.WriteLine("NoopKeyInjector: " + c);
    }
    public void SendArrowUp()
    {
        Console.WriteLine("NoopKeyInjector: SendArrowUp");
    }
    public void SendArrowDown()
    {
        Console.WriteLine("NoopKeyInjector: SendArrowDown");
    }
    public void SendArrowLeft()
    {
        Console.WriteLine("NoopKeyInjector: SendArrowLeft");
    }
    public void SendArrowRight()
    {
        Console.WriteLine("NoopKeyInjector: SendArrowRight");
    }
}