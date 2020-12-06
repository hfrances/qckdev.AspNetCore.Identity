
namespace qckdev.AspNetCore.Identity.Services
{
    public interface ICustomizableAction<TParameter, TValue> : ICustomizableAction
    {

        void Customize(TParameter parameter, TValue value);

    }
}
