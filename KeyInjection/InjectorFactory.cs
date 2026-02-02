using System;

namespace GsproMissionControl.KeyInjection;

public static class InjectorFactory
{
    public static IKeyInjector Create()
    {
        if (OperatingSystem.IsWindows())
            return new WindowsSendInputKeyInjector();

        return new NoopKeyInjector();
    }
}