namespace NamedPipeServer;

public interface ITestCommandHandler
{
    public bool Handle(string path);
}