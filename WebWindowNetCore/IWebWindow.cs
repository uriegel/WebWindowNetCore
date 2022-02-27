namespace WebWindowNetCore;

public interface IWebWindow
{
    void Initialize(Configuration configuration);
    void Execute();
}