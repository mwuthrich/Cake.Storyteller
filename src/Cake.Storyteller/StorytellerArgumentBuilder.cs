﻿using System.Collections.Generic;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Storyteller
{
    /// <summary>
    /// Contains functionality for building arguments for Storyteller
    /// </summary>
    public class StorytellerArgumentBuilder
    {
        private readonly List<string> _stArguments;
        public StorytellerArgumentBuilder()
        {
            _stArguments = new List<string>
            {
                 "results-path" ,
                 "workspace" ,
                 "exclude-tags" ,
                 "open" ,
                 "csv" ,
                 "json" ,
                 "dump" ,
                 "build" ,
                 "profile" ,
                 "timeout" ,
                 "lifecycle" ,
                 "teamcity" ,
                 "config" ,
                 "retries" 
            };
        }

        /// <summary>
        /// Builds arguments for Storyteller
        /// </summary>
        /// <param name="command">Storyteller Command (Open or Run)</param>
        /// <param name="projectPath">Storyteller project path</param>
        /// <param name="arguments">Arguments pass in by cake</param>
        /// <param name="settings"><see cref="StorytellerSettings"/> object</param>
        /// <returns></returns>
        public ProcessArgumentBuilder BuildArguments(StorytellerCommand command, 
                                                     string projectPath, 
                                                     ICakeArguments arguments, 
                                                     StorytellerSettings settings = null)
        {
            var dict = new Dictionary<string, string>();

            if (settings != null)
            {
                //settings
                var props = typeof(StorytellerSettings).GetProperties();
                foreach (var prop in props)
                {
                    var attrs = prop.GetCustomAttributes(false);
                    foreach (var attr in attrs)
                    {
                        var stFlagAttr = attr as StorytellerFlag;
                        if (stFlagAttr != null)
                        {
                            var value = prop.GetValue(settings, null);
                            if (value != null)
                            {
                                dict.Add(stFlagAttr.Flag, value.ToString());
                            }
                        }
                    }
                }
            }

            //arguments
            if (arguments != null)
            {
                _stArguments.ForEach(arg =>
                {
                    if (arguments.HasArgument(arg))
                    {
                        dict[arg] = arguments.GetArgument(arg);
                    }
                });
            }

            var builder = new ProcessArgumentBuilder();
            builder.Append(command.ToString().ToLower());
            builder.Append(projectPath);
            foreach (var pair in dict)
            {
                if (pair.Value == "True" || string.IsNullOrEmpty(pair.Value))
                {
                    builder.Append("--" + pair.Key);
                }
                else
                {
                    builder.Append("--" + pair.Key + " " + pair.Value);
                }
            }
            return builder;
        }
    }
}