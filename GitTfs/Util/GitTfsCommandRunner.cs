﻿using System.Collections.Generic;
using System.Linq;
using Sep.Git.Tfs.Commands;

namespace Sep.Git.Tfs.Util
{
    public class GitTfsCommandRunner
    {
        private readonly IHelpHelper _help;

        public GitTfsCommandRunner(IHelpHelper help)
        {
            _help = help;
        }

        public int Run(GitTfsCommand command, IList<string> args)
        {
            var runMethods = command.GetType().GetMethods().Where(m => m.Name == "Run" && m.ReturnType == typeof(int)).Select(m => new { Method = m, Parameters = m.GetParameters() });
            var splitRunMethods = runMethods.Where(m => m.Parameters.All(p => p.ParameterType == typeof (string)));
            var exactMatchingMethod = splitRunMethods.SingleOrDefault(m => m.Parameters.Length == args.Count);
            if (exactMatchingMethod != null)
                return (int) exactMatchingMethod.Method.Invoke(command, args.ToArray());
            var defaultRunMethod = runMethods.FirstOrDefault(m => m.Parameters.Length == 1 && m.Parameters[0].ParameterType.IsAssignableFrom(args.GetType()));
            if (defaultRunMethod != null)
                return (int) defaultRunMethod.Method.Invoke(command, new object[] {args});
            return _help.ShowHelpForInvalidArguments(command);
        }
    }
}