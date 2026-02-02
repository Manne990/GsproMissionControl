namespace GsproMissionControl.KeyInjection;

public interface IKeyInjector
{
    void SendKeyChar(char c);

    void SendArrowUp();
    void SendArrowDown();
    void SendArrowLeft();
    void SendArrowRight();
}