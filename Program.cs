﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelectorDemo;
using SelectorDemo.Core;
using SelectorDemo.Enums;
using SelectorDemo.Models;
using SelectorDemo.Options;
using SelectorDemo.Services;

IConfiguration? configuration = null;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json");
        configuration = config.Build();
    })
    .ConfigureServices(services =>
    {
        services.Configure<TelcosPrefixAppSetting>(configuration!.GetSection(TelcosPrefixAppSetting.KEY));
        services.AddTransient<ITelcoResolver, TelcoResolver>();
        services.AddMapper(Assembly.GetExecutingAssembly());
    })
    .Build();

var mapper = host.Services.GetRequiredService<IMapper>();
var telcoResolver = host.Services.GetRequiredService<ITelcoResolver>();


var apiRequest = new ApiRequest { PhoneNumber = "2348058412218", CountryCode = CountryCode.NG };

var telco = telcoResolver.Resolve(apiRequest.CountryCode, apiRequest.PhoneNumber);

var selector = new SimSwapSelector(apiRequest.CountryCode, telco);
var omniCoreRequest = mapper.Map(selector, apiRequest);

Console.WriteLine(omniCoreRequest.GetType().Name);

await host.RunAsync();
