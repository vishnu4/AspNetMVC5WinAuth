using System;
using System.Web.Mvc;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using StructureMap.Graph.Scanning;
using System.Linq;
namespace AspNetMVC5WinAuth.DependencyResolution
{


    public class ControllerConvention : IRegistrationConvention
    {

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo<Controller>() && !type.IsAbstract)
            {
                registry.For(type).LifecycleIs(new UniquePerRequestLifecycle());
            }
        }

        public void ScanTypes(TypeSet types, Registry registry)
        {
            //https://structuremap.github.io/registration/auto-registration-and-conventions/
            // Only work on concrete types
            foreach (Type type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed))
            {
                // Register against all the interfaces implemented
                // by this concrete class
                foreach (Type @interface in type.GetInterfaces())
                {
                    registry.For(@interface).Use(type);
                }
            }
        }

    }
}