using System;
using System.Collections.Generic;
using System.Linq;

namespace IoCContainerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Resolver resolver = new Resolver();
            resolver.Register<Shopper, Shopper>();
            //resolver.Register<ICreditCard, MasterCard>();
            resolver.Register<ICreditCard, Visa>();
            var shopper = resolver.Resolve<Shopper>();
            shopper.Charge();
            Console.Read();
        }
    }

    public class Resolver
    {
        private Dictionary<Type, Type> dependencyMap = new Dictionary<Type, Type>();

        public T Resolve<T>()
        {
           return (T)Resolve(typeof(T));
        }

        public void Register<TFrom, TTo>()
        {
            dependencyMap.Add(typeof(TFrom),typeof(TTo));
        }

        private object Resolve(Type typeToResolve)
        {
            Type resolvedType = null;
            try
            {
                resolvedType = dependencyMap[typeToResolve];
            }
            catch
            {
                throw new Exception(string.Format("Could not resolve type {0}", typeToResolve.FullName));
            }

            var firstConstructor = resolvedType.GetConstructors().First();
            var constructorParameters = firstConstructor.GetParameters();
            if (constructorParameters.Count() == 0)
                return Activator.CreateInstance(resolvedType);
     
            IList<object> parameters = new List<object>();
            foreach(var parameterToResolve in constructorParameters)
            {
                parameters.Add(Resolve(parameterToResolve.ParameterType));
            }
            return firstConstructor.Invoke(parameters.ToArray());
        }
    }

    public class MasterCard : ICreditCard
    {
        public string Charge()
        {
            return "Swiping the MasterCard!";
        }
    }

    public class Visa : ICreditCard
    {
        public string Charge()
        {
            return "Chaaarging with the Visa!";
        }
    }
    public class Shopper
    {
        private readonly ICreditCard creditCard;
        public Shopper(ICreditCard creditCard)
        {
            this.creditCard = creditCard;
        }
        public void Charge()
        {
            var chargeMessage = creditCard.Charge();
            Console.WriteLine(chargeMessage);
        }
    }

    public interface ICreditCard
    {
        string Charge();
    }
}
