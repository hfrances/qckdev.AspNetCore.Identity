
using System;

namespace qckdev.AspNetCore.Identity.Services
{
    public sealed class GenericCustomizableAction<TParameter, TValue> : CustomizableAction<TParameter, TValue>
    {

        private Action<TParameter, TValue> Action { get; }


        public GenericCustomizableAction(Action<TParameter, TValue> action)
        {
            this.Action = action;
        }

        public override void Customize(TParameter parameter, TValue value)
        {
            if (this.Action == null)
            {

                throw new NotImplementedException();
            }
            else
            {
                this.Action.Invoke(parameter, value);
            }
        }

    }
}
