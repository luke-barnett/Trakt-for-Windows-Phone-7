namespace Trakt_for_Windows_Phone_7.Framework
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Phone.Tasks;
    using Caliburn.Micro;
    using Trakt_for_Windows_Phone_7.ViewModels;

    public class Trakt_for_Windows_Phone_7Bootstrapper : PhoneBootstrapper
    {
        PhoneContainer container;

        protected override void Configure()
        {
            container = new PhoneContainer(this);

            container.RegisterSingleton(typeof(MainPageViewModel), "MainPageViewModel", typeof(MainPageViewModel));
            container.RegisterPerRequest(typeof(MovieViewModel), "MovieViewModel", typeof(MovieViewModel));
            container.RegisterPerRequest(typeof(ShowViewModel), "ShowViewModel", typeof(ShowViewModel));
            container.RegisterPerRequest(typeof(SeasonViewModel), "SeasonViewModel", typeof(SeasonViewModel));
            container.RegisterPerRequest(typeof(EpisodeViewModel), "EpisodeViewModel", typeof(EpisodeViewModel));

            container.RegisterInstance(typeof(INavigationService), null, new FrameAdapter(RootFrame));
            container.RegisterInstance(typeof(IPhoneService), null, new PhoneApplicationServiceAdapter(PhoneService));
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}