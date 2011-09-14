namespace Trakt_for_Windows_Phone_7.Framework
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using ViewModels;

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
            container.RegisterPerRequest(typeof(RecommendationsViewModel), "RecommendationsViewModel", typeof(RecommendationsViewModel));
            container.RegisterPerRequest(typeof(WatchListViewModel), "WatchListViewModel", typeof(WatchListViewModel));
            container.RegisterPerRequest(typeof(SearchViewModel), "SearchViewModel", typeof(SearchViewModel));


            container.RegisterPerRequest(typeof(ShoutViewModel), null, typeof(ShoutViewModel));
            container.RegisterPerRequest(typeof(LogInViewModel), null, typeof(LogInViewModel));
            container.RegisterInstance(typeof(INavigationService), null, new FrameAdapter(RootFrame));
            container.RegisterInstance(typeof(IWindowManager), null, new WindowManager());

            container.RegisterInstance(typeof(PhoneContainer), null, container);
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