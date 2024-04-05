namespace TIKSN.Configuration;

public interface IPartialConfiguration<out T>
{
    T GetConfiguration();
}
