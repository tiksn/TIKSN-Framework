namespace TIKSN.Configuration
{
    public interface IPartialConfiguration<T>
    {
        T GetConfiguration();
    }
}
