namespace TIKSN.Identity;

public interface IIdentityGenerator<out T>
{
    public T Generate();
}
