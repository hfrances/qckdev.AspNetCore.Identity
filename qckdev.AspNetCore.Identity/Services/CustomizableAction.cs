
using System;

namespace qckdev.AspNetCore.Identity.Services
{
    public abstract class CustomizableAction<TParameter, TValue> :
        ICustomizableAction, ICustomizableAction<TParameter, TValue>
    {

        void ICustomizableAction.Customize(object parameter, object value)
        {
            this.Customize((TParameter)parameter, (TValue)value);
        }

        public abstract void Customize(TParameter parameter, TValue value);

    }
}
