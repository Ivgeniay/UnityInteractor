using System.Collections.Generic;
using System;
using static InteractionSystem.Repositories.Repository<int>;
using System.Linq;

namespace InteractionSystem.Repositories
{
    internal class Repository<T>
    {
        public event EventHandler<EventArgs> Changed;

        private readonly List<T> Items = new();
        public Repository() { }
        public Repository(IEnumerable<T> items) { Items = items.ToList(); }

        public void Register(T instance)
        {
            if (!Items.Contains(instance))
            {
                Items.Add(instance);
                Changed?.Invoke(this, new EventArgs() { Registered = instance });
            }
        }

        public void Unregister(T instance)
        {
            if (Items.Contains(instance))
            {
                Items.Remove(instance);
                Changed?.Invoke(this, new EventArgs() { Unregister = instance });
            }
        }

        public class EventArgs
        {
            public T Registered;
            public T Unregister;
        }

    }

}
