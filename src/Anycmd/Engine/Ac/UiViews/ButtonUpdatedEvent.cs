﻿
namespace Anycmd.Engine.Ac.UiViews
{
    using UiViews;
    using Events;
    using InOuts;

    /// <summary>
    /// 
    /// </summary>
    public class ButtonUpdatedEvent : DomainEvent
    {
        public ButtonUpdatedEvent(IAcSession acSession, ButtonBase source, IButtonUpdateIo input)
            : base(acSession, source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IButtonUpdateIo Input { get; private set; }
    }
}