# Bolt.App.Core.Extensions.Default

Provide some common requirments for application. E.g Easy strong type settings from configuration, A facility to run task during application startup and a static Log class to log without injecting ILogger in every single classes if you like.

## How to use this library

Add this nuget package Bolt.App.Core.Extensions.Default in your app and add following line in your startup configureservices method

    services.AddCoreFeatures();

## Easy settings from configuration

Say you building a class to download image from image server. And you like to put image server url and optimization level in your settings. Now say you define this settings in your appsettings as below:

    "App": {
      "ImageFeature": {
        "ServerUrl": "http://your-image-server.svc",
        "QualityLevel": "High"
      }
    }

Now you can define your settings as below and decorate your settings dto with configuration path as below:

    [ConfigSectionName("App:ImageFeature", isRequired: false)]
    public class ImageLoadSettings
    {
        public string ServerUrl { get; set; }
        public QualityLevelType QualityLevel { get; set; }
    }

Thats it. Now you can use the settings directly in your classes as below. No need to go back to startup and injection in

    public class ImageLoadService
    {
      private ImageLoadSettings config;

      public ImageLoadService(IConfig<ImageLoadSettings> config)
      {
        config = config.Value;
      }

      ....
    }

## How i can run some code in application startup

This lib provide an interface IBootstrapperTask and any implementation of that registered in your ioc will be run on application startup.

    public class SomethingToRunAtStartup : IBootstrapperTask
    {
      public Task RunAsync()
      {
        ... your code here. You can inject your dependeies in constructor also. But if you inject something that need to run in scope then inject IServiceProvider.
      }
    }

## Can i use static logger class

This lib provide static log class that initialized with ILogger<> during application startup using IBootstrapperTask. So you can directly use that static class in your classes.

    public class ImageLoader
    {
      public void Load()
      {
        Log.Info("Loading image...");
      }
    }

> Note: Log static class initialized using a bootstrapper task during application startup. So if you use Log static class in your custom BoostrapperTask then there is possiblity Log not initialized yet and nothing logged in system. So don't use Log class inside your customg impl of IBootrapperTask.
