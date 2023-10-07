namespace TIKSN.Identity;

public interface IIdentityGenerator<out T>
{
    T Generate();
}
