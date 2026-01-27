namespace TIKSN.Configuration;

public interface IPartialConfiguration<out T>
{
    public T GetConfiguration();
}
