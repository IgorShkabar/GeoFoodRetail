/** */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace GeoFoodRetail.Tests.GIS.Tests
{
    [SetUpFixture]
    public class AssemblyResolver
    {
        [OneTimeSetUp]
        public void RegisterResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedName = new AssemblyName(args.Name);
                if (requestedName.Name == "System.Buffers")
                {
                    // return the System.Buffers assembly, which is already available in the execution context.
                    return typeof(System.Buffers.ArrayPool<>).Assembly;
                }
                return null;
            };
        }
    }
}
