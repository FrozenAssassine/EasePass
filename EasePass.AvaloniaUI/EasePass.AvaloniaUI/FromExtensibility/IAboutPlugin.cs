/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using System;

namespace EasePassExtensibility
{
    /// <summary>
    /// Implement this interface to give Ease Pass some information about your plugin. It is highly recommended to implement this interface a single time in every plugin.
    /// </summary>
    public interface IAboutPlugin : IExtensionInterface
    {
        /// <summary>
        /// Name of your plugin
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// Description of your plugin
        /// </summary>
        string PluginDescription { get; }
        /// <summary>
        /// Author of your plugin
        /// </summary>
        string PluginAuthor { get; }
        /// <summary>
        /// URL to the authors webpage
        /// </summary>
        string PluginAuthorURL { get; }
        /// <summary>
        /// URI to the icon of the plugin
        /// </summary>
        Uri PluginIcon { get; }
    }
}
