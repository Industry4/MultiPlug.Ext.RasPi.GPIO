﻿using System;
using System.Reflection;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.About
{
    [Route("about")]
    public class AboutController : SettingsApp
    {
        public Response Get()
        {
            Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

            return new Response
            {
                Template = Templates.About,
                Model = new AboutModel
                {
                    Title = ExecutingAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title,
                    Description = ExecutingAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                    Company = ExecutingAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                    Product = ExecutingAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product,
                    Copyright = ExecutingAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright,
                    Trademark = ExecutingAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark,
                    Version = ExecutingAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                    Log = string.Join("\r", Core.Instance.RaspberryPi.LoggingService.Read()),
                    WiringPiVersion = Core.Instance.RaspberryPi.GPIOVersion,
                    LoggingLevel = Core.Instance.RaspberryPi.LoggingLevel
                }
            };
        }

        public Response Post(AboutPostModel theModel)
        {
            Core.Instance.RaspberryPi.LoggingLevel = theModel.LoggingLevel;

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new Uri(Context.Request.AbsoluteUri)
            };
        }
    }
}
