namespace Trakt_for_Windows_Phone_7.Framework
{
    using System;
    using Caliburn.Micro;

    public class PhoneContainer : SimpleContainer
    {
        readonly PhoneBootstrapper bootstrapper;

        public PhoneContainer(PhoneBootstrapper bootstrapper)
        {
            this.bootstrapper = bootstrapper;
            Activator = new InstanceActivator(bootstrapper, type => GetInstance(type, null));
        }

        public InstanceActivator Activator { get; private set; }

        protected override object ActivateInstance(Type type, object[] args)
        {
            return Activator.ActivateInstance(base.ActivateInstance(type, args));
        }
    }
}